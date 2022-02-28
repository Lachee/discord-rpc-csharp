#if (UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE) && !DISABLE_DISCORD
using DiscordRPC.IO;
using DiscordRPC.Logging;
using System;
using Lachee.IO;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

namespace DiscordRPC.Unity
{
    /// <summary>
    /// Pipe Client used to communicate with Discord.
    /// </summary>
    public class UnityNamedPipe : INamedPipeClient
    {
        const string PIPE_NAME = @"discord-ipc-{0}";

        private NamedPipeClientStream _stream;
        private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

        public ILogger Logger { get; set; }
        public bool IsConnected {  get { return _stream != null && _stream.IsConnected; } }
        public int ConnectedPipe { get; private set; }

        private volatile bool _isDisposed = false;

        public bool Connect(int pipe)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("NamedPipe");
            
            if (pipe > 9)
                throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");
            
            if (pipe < 0)
            {
                //If we have -1,  then we need to iterate over every single pipe until we get it
                //Iterate until we connect to a pipe
                for (int i = 0; i < 10; i++)
                {
                    if (AttemptConnection(i) || AttemptConnection(i, true))
                        return true;
                }

                //We failed everythign else
                return false;
            }
            else
            {
                //We have a set one so we should just straight up try to connect to it
                return AttemptConnection(pipe) || AttemptConnection(pipe, true);
            }
        }

        private bool AttemptConnection(int pipe, bool doSandbox = false)
        { 
            //Make sure the stream is null
            if (_stream != null)
            {
                Logger.Error("Attempted to create a new stream while one already exists!");
                return false;
            }

            //Make sure we are disconnected
            if (IsConnected)
            {
                Logger.Error("Attempted to create a new connection while one already exists!");
                return false;
            }

            try
            {
                //Prepare the sandbox
                string sandbox = doSandbox ? GetPipeSandbox() : "";
                if (doSandbox && sandbox == null)
                {
                    Logger.Trace("Skipping sandbox because this platform does not support it.");
                    return false;
                }

                //Prepare the name
                string pipename = GetPipeName(pipe);

                //Attempt to connect
                Logger.Info("Connecting to " + pipename + " (" + sandbox +")");
                ConnectedPipe = pipe;
                _stream = new NamedPipeClientStream(".", pipename);
                _stream.Connect();

                Logger.Info("Connected");
                return true;
            }
            catch(Exception e)
            {
                Logger.Error("Failed: " + e.GetType().FullName + ", " + e.Message);
                ConnectedPipe = -1;
                Close();
                return false;
            }
        }

        public void Close()
        {
            if (_stream != null)
            {
                Logger.Trace("Closing stream");
                _stream.Dispose();
                _stream = null;
            }
        }

        public void Dispose()
        {
            if (_isDisposed) return;
            Logger.Trace("Disposing Stream");
            _isDisposed = true;
            Close();
        }

        public bool ReadFrame(out PipeFrame frame)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("_stream");

            //We are not connected so we cannot read!
            if (!IsConnected)
            {
                frame = default(PipeFrame);
                return false;
            }

            //Try and read a frame
            int length = _stream.Read(_buffer, 0, _buffer.Length);
            Logger.Trace("Read {0} bytes", length);

            if (length == 0)
            {
                frame = default(PipeFrame);
                return false;
            }

            //Read the stream now
            using (MemoryStream memory = new MemoryStream(_buffer, 0, length))
            {
                frame = new PipeFrame();
                if (!frame.ReadStream(memory))
                {
                    Logger.Error("Failed to read a frame! {0}", frame.Opcode);
                    return false;
                }
                else
                {
                    Logger.Trace("Read pipe frame!");
                    return true;
                }
            }
        }

        public bool WriteFrame(PipeFrame frame)
        {
            if (_isDisposed)
                throw new ObjectDisposedException("_stream");

            //Write the frame. We are assuming proper duplex connection here
            if (!IsConnected)
            {
                Logger.Error("Failed to write frame because the stream is closed");
                return false;
            }

            try
            {
                //Write the pipe
                //This can only happen on the main thread so it should be fine.
                Logger.Trace("Writing frame");
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

        private string GetPipeName(int pipe, string sandbox = "")
        {
            switch (Environment.OSVersion.Platform)
            {
                default:

#if !UNITY_EDITOR_OSX
                case PlatformID.Win32NT:
                case PlatformID.Win32S:
                case PlatformID.Win32Windows:
                case PlatformID.WinCE:
                    Logger.Trace("PIPE WIN");
                    return sandbox + string.Format(PIPE_NAME, pipe);
#endif

                case PlatformID.Unix:
                case PlatformID.MacOSX:
                    Logger.Trace("PIPE UNIX / MACOSX");
                    return Path.Combine(GetEnviromentTemp(), sandbox + string.Format(PIPE_NAME, pipe));
            }
        }

        private string GetPipeSandbox()
        {
            switch (Environment.OSVersion.Platform)
            {
                default:
                    return null;
                case PlatformID.Unix:
                    return "snap.discord/";
            }
        }

        private string GetEnviromentTemp()
        {
            string temp = null;
            temp = temp ?? Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
            temp = temp ?? Environment.GetEnvironmentVariable("TMPDIR");
            temp = temp ?? Environment.GetEnvironmentVariable("TMP");
            temp = temp ?? Environment.GetEnvironmentVariable("TEMP");
            temp = temp ?? "/tmp";
            return temp;
        }
    }
}
#endif