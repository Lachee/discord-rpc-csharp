#if (UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE_WIN) && !DISABLE_DISCORD
using DiscordRPC.IO;
using DiscordRPC.Logging;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace DiscordRPC.Unity
{
    /// <summary>
    /// Pipe Client used to communicate with Discord.
    /// </summary>
    public class DiscordNativeNamedPipe : INamedPipeClient
    {
        const string PIPE_NAME = @"\\?\pipe\discord-ipc-{0}";

        /// <summary>
        /// The logger of this pipe
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        /// Is the pipe currently connected?
        /// </summary>
        public bool IsConnected { get { return NativePipeExtern.IsConnected(); } }

        /// <summary>
        /// The pipe number we are connected too
        /// </summary>
        public int ConnectedPipe { get { return _connectedPipe; } }
        private int _connectedPipe;

        private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

        /// <summary>
        /// The last pipe error read.
        /// </summary>
        internal NativePipeExtern.PipeReadError LastError { get { return _lasterr; } }
        private NativePipeExtern.PipeReadError _lasterr = NativePipeExtern.PipeReadError.None;

        /// <summary>
        /// Attempts to establish a connection to the pipe
        /// </summary>
        /// <param name="pipe">The pipe to connect too</param>
        /// <returns></returns>
        public bool Connect(int pipe)
        {
            if (IsConnected)
            {
                Logger.Error("Cannot connect as the pipe is already connected");
                throw new Exception("Cannot connect as the pipe is already connected");
            }

            if (pipe > 9)
            {
                Logger.Error("Argument cannot be greater than 9");
                throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");
            }


            //Attempt to connect to the specific pipe
            if (pipe >= 0 && AttemptConnection(pipe))
                return true;

            //Iterate until we connect to a pipe
            for (int i = 0; i < 10; i++)
            {
                if (AttemptConnection(i))
                    return true;
            }

            //We failed to connect
            return false;
        }
        private bool AttemptConnection(int pipe)
        {
            if (IsConnected)
            {
                Logger.Error("Cannot connect as the pipe is already connected");
                throw new Exception("Cannot connect as the pipe is already connected");
            }

            //Prepare the pipe name
            string pipename = string.Format(PIPE_NAME, pipe);
            Logger.Info("Attempting to connect to " + pipename);

            uint err = NativePipeExtern.Open(pipename);
            if (err == 0 && IsConnected)
            {
                Logger.Info("Succesfully connected to " + pipename);
                _connectedPipe = pipe;
                return true;
            }
            else
            {
                Logger.Error("Failed to connect to native pipe. Err: {0}", err);
                return false;
            }
        }

        /// <summary>
        /// Attempts to read a frame
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool ReadFrame(out PipeFrame frame)
        {
            //Make sure we are connected
            if (!IsConnected)
                throw new Exception("Cannot read Native Stream as pipe is not connected");

            //Prepare bytes read and a buffer of memory to read into
            int bytesRead = 0;
            int size = Marshal.SizeOf(_buffer[0]) * _buffer.Length;
            IntPtr pnt = Marshal.AllocHGlobal(size);

            //Try and read the frame from the native pipe
            try
            {
                bytesRead = NativePipeExtern.ReadFrame(pnt, _buffer.Length);
                if (bytesRead <= 0)
                {
                    //We have not read a valid amount of bytes, so return a error message.
                    //Update the error message
                    _lasterr = NativePipeExtern.PipeReadError.ReadEmptyMessage;

                    //A error actively occured. If it is 0 we just read no bytes.
                    if (bytesRead < 0)
                    {
                        //We have a pretty bad error, we will log it for prosperity. 
                        _lasterr = (NativePipeExtern.PipeReadError)bytesRead;
                        Logger.Error("Native pipe failed to read: {0}", _lasterr.ToString());

                        //Close this pipe
                        this.Close();
                    }

                    //Return a empty frame and return false (read failure).
                    frame = default(PipeFrame);
                    return false;
                }
                else
                {
                    //WE have read a valid amount of bytes, so copy the marshaled bytes over to the buffer
                    Marshal.Copy(pnt, _buffer, 0, bytesRead);
                }
            }
            finally
            {
                //Finally, before we exit this try block, free the pointer we allocated.
                Marshal.FreeHGlobal(pnt);
            }

            //Parse the message by reading the contents into a memory stream.
            using (MemoryStream stream = new MemoryStream(_buffer, 0, bytesRead))
            {
                //Try to parse the stream
                frame = new PipeFrame();
                if (frame.ReadStream(stream) && frame.Length != 0)
                    return true;

                //We failed
                Logger.Error("Pipe failed to read from the data received by the stream.");
                return false;
            }
        }

        /// <summary>
        /// Attempts to write a frame
        /// </summary>
        /// <param name="frame"></param>
        /// <returns></returns>
        public bool WriteFrame(PipeFrame frame)
        {
            if (!IsConnected)
                throw new Exception("Cannot write Native Stream as pipe is not connected");

            //Create a memory stream so we can write it to the pipe
            using (MemoryStream stream = new MemoryStream())
            {
                //Write the stream and the send it to the pipe
                frame.WriteStream(stream);

                //Get the bytes and send it
                byte[] bytes = stream.ToArray();

                //Copy the bytes into a new marshaled block
                int size = Marshal.SizeOf(bytes[0]) * bytes.Length;
                IntPtr pnt = Marshal.AllocHGlobal(size);
                try
                {
                    //Send the marshaled block
                    Marshal.Copy(bytes, 0, pnt, bytes.Length);
                    return NativePipeExtern.WriteFrame(pnt, bytes.Length);
                }
                finally
                {
                    //Finally, before exiting the try catch, free the memory we assigned.
                    Marshal.FreeHGlobal(pnt);
                }
            }
        }

        /// <summary>
        /// Closes the pipe
        /// </summary>
        public void Close()
        {
            NativePipeExtern.Close();
        }

        /// <summary>
        /// Closes the pipe
        /// </summary>
        public void Dispose()
        {
            //Close the stream (disposing of it too)
            Close();
        }
    }

    internal static class NativePipeExtern
    {
        public enum PipeReadError
        {
            None = 0,
            BufferZeroSized = -1,
            PipeNotConnected = -2,
            FailedToRead = -3,
            ReadEmptyMessage = -4,
            FailedToPeek = -5,
        }

        [DllImport("DiscordRPC.Native", EntryPoint = "isConnected", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool IsConnected();

        [DllImport("DiscordRPC.Native", EntryPoint = "readFrame", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern int ReadFrame(IntPtr buffer, int length);

        [DllImport("DiscordRPC.Native", EntryPoint = "writeFrame", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern bool WriteFrame(IntPtr buffer, int length);

        [DllImport("DiscordRPC.Native", EntryPoint = "close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern void Close();

        [DllImport("DiscordRPC.Native", EntryPoint = "open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
        public static extern UInt32 Open([MarshalAs(UnmanagedType.LPStr)] string pipename);
    }
}
#endif