using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.Logging;
using System.IO.Pipes;
using System.Threading;
using System.IO;

namespace DiscordRPC.IO
{
    /// <summary>
    /// A named pipe client using the .NET framework <see cref="NamedPipeClientStream"/>
    /// </summary>
    public sealed class ManagedNamedPipeClient : INamedPipeClient
    {
        /// <summary>
        /// Name format of the pipe
        /// </summary>
		const string PIPE_NAME = @"discord-ipc-{0}";

        /// <summary>
        /// The logger for the Pipe client to use
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Checks if the client is connected
        /// </summary>
        public bool IsConnected
        {
            get
            {
                //This will trigger if the stream is disabled. This should prevent the lock check
                if (_isClosed) return false;
                lock (l_stream)
                {
                    //We cannot be sure its still connected, so lets double check
                    return _stream != null && _stream.IsConnected;
                }
            }
        }

        /// <summary>
        /// The pipe we are currently connected too.
        /// </summary>
        public int ConnectedPipe { get { return _connectedPipe; } }

        private int _connectedPipe;
        private NamedPipeClientStream _stream;

        private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

        private Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();
        private object _framequeuelock = new object();

        private volatile bool _isDisposed = false;
        private volatile bool _isClosed = true;

        private object l_stream = new object();

        /// <summary>
        /// Creates a new instance of a Managed NamedPipe client. Doesn't connect to anything yet, just setups the values.
        /// </summary>
        public ManagedNamedPipeClient()
        {
            _buffer = new byte[PipeFrame.MAX_SIZE];
            Logger = new NullLogger();
            _stream = null;
        }

        /// <summary>
        /// Connects to the pipe
        /// </summary>
        /// <param name="pipe"></param>
        /// <returns></returns>
        public bool Connect(int pipe)
        {
            Logger.Trace("ManagedNamedPipeClient.Connection({0})", pipe);

            if (_isDisposed)
                throw new ObjectDisposedException("NamedPipe");

            if (pipe > 9)
                throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");

            if (pipe < 0)
            {
                //Iterate until we connect to a pipe
                for (int i = 0; i < 10; i++)
                {
                    if (AttemptConnection(i) || AttemptConnection(i, true))
                    {
                        BeginReadStream();
                        return true;
                    }
                }
            }
            else
            {
                //Attempt to connect to a specific pipe
                if (AttemptConnection(pipe) || AttemptConnection(pipe, true))
                {
                    BeginReadStream();
                    return true;
                }
            }

            //We failed to connect
            return false;
        }

        /// <summary>
        /// Attempts a new connection
        /// </summary>
        /// <param name="pipe">The pipe number to connect too.</param>
        /// <param name="isSandbox">Should the connection to a sandbox be attempted?</param>
        /// <returns></returns>
        private bool AttemptConnection(int pipe, bool isSandbox = false)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("_stream");

            //If we are sandbox but we dont support sandbox, then skip
            string sandbox = isSandbox ? GetPipeSandbox() : "";
            if (isSandbox && sandbox == null)
            {
                Logger.Trace("Skipping sandbox connection.");
                return false;
            }

            //Prepare the pipename
            Logger.Trace("Connection Attempt {0} ({1})", pipe, sandbox);
            string pipename = GetPipeName(pipe, sandbox);

            try
            {
                //Create the client
                lock (l_stream)
                {
                    Logger.Info("Attempting to connect to '{0}'", pipename);
                    _stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut, PipeOptions.Asynchronous);

                    // Intentionally use a timeout of 0 here to avoid spinlock overhead.
                    // We are already performing local retry logic, so this is not required.
                    _stream.Connect(0);

                    //Spin for a bit while we wait for it to finish connecting
                    Logger.Trace("Waiting for connection...");
                    do { Thread.Sleep(10); } while (!_stream.IsConnected);
                }

                //Store the value
                Logger.Info("Connected to '{0}'", pipename);
                _connectedPipe = pipe;
                _isClosed = false;
            }
            catch (Exception e)
            {
                //Something happened, try again
                //TODO: Log the failure condition
                Logger.Error("Failed connection to {0}. {1}", pipename, e.Message);
                Close();
            }

            Logger.Trace("Done. Result: {0}", _isClosed);
            return !_isClosed;
        }

        /// <summary>
        /// Starts a read. Can be executed in another thread.
        /// </summary>
        private void BeginReadStream()
        {
            if (_isClosed) return;
            try
            {
                lock (l_stream)
                {
                    //Make sure the stream is valid
                    if (_stream == null || !_stream.IsConnected) return;

                    Logger.Trace("Begining Read of {0} bytes", _buffer.Length);
                    _stream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(EndReadStream), _stream.IsConnected);
                }
            }
            catch (ObjectDisposedException)
            {
                Logger.Warning("Attempted to start reading from a disposed pipe");
                return;
            }
            catch (InvalidOperationException)
            {
                //The pipe has been closed
                Logger.Warning("Attempted to start reading from a closed pipe");
                return;
            }
            catch (Exception e)
            {
                Logger.Error("An exception occured while starting to read a stream: {0}", e.Message);
                Logger.Error(e.StackTrace);
            }
        }

        /// <summary>
        /// Ends a read. Can be executed in another thread.
        /// </summary>
        /// <param name="callback"></param>
        private void EndReadStream(IAsyncResult callback)
        {
            Logger.Trace("Ending Read");
            int bytes = 0;

            try
            {
                //Attempt to read the bytes, catching for IO exceptions or dispose exceptions
                lock (l_stream)
                {
                    //Make sure the stream is still valid
                    if (_stream == null || !_stream.IsConnected) return;

                    //Read our btyes
                    bytes = _stream.EndRead(callback);
                }
            }
            catch (IOException)
            {
                Logger.Warning("Attempted to end reading from a closed pipe");
                return;
            }
            catch (NullReferenceException)
            {
                Logger.Warning("Attempted to read from a null pipe");
                return;
            }
            catch (ObjectDisposedException)
            {
                Logger.Warning("Attemped to end reading from a disposed pipe");
                return;
            }
            catch (Exception e)
            {
                Logger.Error("An exception occured while ending a read of a stream: {0}", e.Message);
                Logger.Error(e.StackTrace);
                return;
            }

            //How much did we read?
            Logger.Trace("Read {0} bytes", bytes);

            //Did we read anything? If we did we should enqueue it.
            if (bytes > 0)
            {
                //Load it into a memory stream and read the frame
                using (MemoryStream memory = new MemoryStream(_buffer, 0, bytes))
                {
                    try
                    {
                        PipeFrame frame = new PipeFrame();
                        if (frame.ReadStream(memory))
                        {
                            Logger.Trace("Read a frame: {0}", frame.Opcode);

                            //Enqueue the stream
                            lock (_framequeuelock)
                                _framequeue.Enqueue(frame);
                        }
                        else
                        {
                            //TODO: Enqueue a pipe close event here as we failed to read something.
                            Logger.Error("Pipe failed to read from the data received by the stream.");
                            Close();
                        }
                    }
                    catch (Exception e)
                    {
                        Logger.Error("A exception has occured while trying to parse the pipe data: {0}", e.Message);
                        Close();
                    }
                }
            }
            else
            {
                //If we read 0 bytes, its probably a broken pipe. However, I have only confirmed this is the case for MacOSX.
                // I have added this check here just so the Windows builds are not effected and continue to work as expected.
                if (IsUnix())
                {
                    Logger.Error("Empty frame was read on {0}, aborting.", Environment.OSVersion);
                    Close();
                }
                else
                {
                    Logger.Warning("Empty frame was read. Please send report to Lachee.");
                }
            }

            //We are still connected, so continue to read
            if (!_isClosed && IsConnected)
            {
                Logger.Trace("Starting another read");
                BeginReadStream();
            }
        }

        /// <summary>
        /// Reads a frame, returning false if none are available
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool ReadFrame(out PipeFrame frame)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("_stream");

            //Check the queue, returning the pipe if we have anything available. Otherwise null.
            lock (_framequeuelock)
            {
                if (_framequeue.Count == 0)
                {
                    //We found nothing, so just default and return null
                    frame = default(PipeFrame);
                    return false;
                }

                //Return the dequed frame
                frame = _framequeue.Dequeue();
                return true;
            }
        }

        /// <summary>
        /// Writes a frame to the pipe
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool WriteFrame(PipeFrame frame)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("_stream");

            //Write the frame. We are assuming proper duplex connection here
            if (_isClosed || !IsConnected)
            {
                Logger.Error("Failed to write frame because the stream is closed");
                return false;
            }

            try
            {
                //Write the pipe
                //This can only happen on the main thread so it should be fine.
                frame.WriteStream(_stream);
                return true;
            }
            catch (IOException io)
            {
                Logger.Error("Failed to write frame because of a IO Exception: {0}", io.Message);
            }
            catch (ObjectDisposedException)
            {
                Logger.Warning("Failed to write frame as the stream was already disposed");
            }
            catch (InvalidOperationException)
            {
                Logger.Warning("Failed to write frame because of a invalid operation");
            }

            //We must have failed the try catch
            return false;
        }

        /// <summary>
        /// Closes the pipe
        /// </summary>
        public void Close()
        {
            //If we are already closed, jsut exit
            if (_isClosed)
            {
                Logger.Warning("Tried to close a already closed pipe.");
                return;
            }

            //flush and dispose
            try
            {
                //Wait for the stream object to become available.
                lock (l_stream)
                {
                    if (_stream != null)
                    {
                        try
                        {
                            //Stream isn't null, so flush it and then dispose of it.\
                            // We are doing a catch here because it may throw an error during this process and we dont care if it fails.
                            _stream.Flush();
                            _stream.Dispose();
                        }
                        catch (Exception)
                        {
                            //We caught an error, but we dont care anyways because we are disposing of the stream.
                        }

                        //Make the stream null and set our flag.
                        _stream = null;
                        _isClosed = true;
                    }
                    else
                    {
                        //The stream is already null?
                        Logger.Warning("Stream was closed, but no stream was available to begin with!");
                    }
                }
            }
            catch (ObjectDisposedException)
            {
                //ITs already been disposed
                Logger.Warning("Tried to dispose already disposed stream");
            }
            finally
            {
                //For good measures, we will mark the pipe as closed anyways
                _isClosed = true;
                _connectedPipe = -1;
            }
        }

        /// <summary>
        /// Disposes of the stream
        /// </summary>
        public void Dispose()
        {
            //Prevent double disposing
            if (_isDisposed) return;

            //Close the stream (disposing of it too)
            if (!_isClosed) Close();

            //Dispose of the stream if it hasnt been destroyed already.
            lock (l_stream)
            {
                if (_stream != null)
                {
                    _stream.Dispose();
                    _stream = null;
                }
            }

            //Set our dispose flag
            _isDisposed = true;
        }

        /// <summary>
        /// Returns a platform specific path that Discord is hosting the IPC on.
        /// </summary>
        /// <param name="pipe">The pipe number.</param>
        /// <param name="sandbox">The sandbox environment the pipe is in</param>
        /// <returns></returns>
        public static string GetPipeName(int pipe, string sandbox)
        {
            if (!IsUnix()) return sandbox + string.Format(PIPE_NAME, pipe);
            return Path.Combine(GetTemporaryDirectory(), sandbox + string.Format(PIPE_NAME, pipe));
        }

        /// <summary>
        /// returns a platform specific path that Discord is hosting the IPC on.
        /// </summary>
        /// <param name="pipe">The pipe number</param>
        /// <returns></returns>
        public static string GetPipeName(int pipe)
            => GetPipeName(pipe, "");

        /// <summary>
        /// Gets the name of the possible sandbox environment the pipe might be located within. If the platform doesn't support sandboxed Discord, then it will return null.
        /// </summary>
        /// <returns></returns>
        public static string GetPipeSandbox()
        {
            switch (Environment.OSVersion.Platform)
            {
                default:
                    return null;
                case PlatformID.Unix:
                    return "snap.discord/";
            }
        }

        /// <summary>
        /// Gets the temporary path for the current environment. Only applicable for UNIX based systems.
        /// </summary>
        /// <returns></returns>
        private static string GetTemporaryDirectory()
        {
            string temp = null;
            temp = temp ?? Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
            temp = temp ?? Environment.GetEnvironmentVariable("TMPDIR");
            temp = temp ?? Environment.GetEnvironmentVariable("TMP");
            temp = temp ?? Environment.GetEnvironmentVariable("TEMP");
            temp = temp ?? "/tmp";
            return temp;
        }

        /// <summary>
        /// Returns true if the current OS platform is Unix based (Unix or MacOSX).
        /// </summary>
        /// <returns></returns>
        public static bool IsUnix()
        {
            switch (Environment.OSVersion.Platform)
            {
                default:
                    return false;

                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    return true;
            }
        }
    }
}
