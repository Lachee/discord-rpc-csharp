using Lachee.IO.Exceptions;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace Lachee.IO
{
    public class NamedPipeClientStream : System.IO.Stream
    {
        private IntPtr ptr;
        private bool _isDisposed;
        
        /// <summary>
        /// Can the stream read? Always returns true.
        /// </summary>
        public override bool CanRead { get { return true; } }

        /// <summary>
        /// Can the stream seek? Always returns false.
        /// </summary>
        public override bool CanSeek { get { return false; } }

        /// <summary>
        /// Can the stream write? Always returns true.
        /// </summary>
        public override bool CanWrite { get { return true; } }

        /// <summary>
        /// The length of the stream. Always 0.
        /// </summary>
        public override long Length { get { return 0; } }

        /// <summary>
        /// The current position of the stream. Always 0.
        /// </summary>
        public override long Position { get { return 0; } set { } }

        /// <summary>
        /// Checks if the current pipe is connected and running.
        /// </summary>
        public bool IsConnected { get { return Native.IsConnected(ptr); } }

        /// <summary>
        /// The pipe name for this client.
        /// </summary>
        public string PipeName { get; private set; }
        
        #region Constructors
        /// <summary>
        /// Creates a new instance of a NamedPipeClient
        /// </summary>
        /// <param name="server">The remote to connect too</param>
        /// <param name="pipeName">The name of the pipe that will be connected too.</param>
        public NamedPipeClientStream(string server, string pipeName)
        {
            ptr = Native.CreateClient();
            PipeName = FormatPipe(server, pipeName);
            Console.WriteLine("Created new NamedPipeClientStream '{0}' => '{1}'", pipeName, PipeName);
        }
        
        ~NamedPipeClientStream()
        {
            Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            if (!_isDisposed)
            {
                Disconnect();
                Native.DestroyClient(ptr);
                _isDisposed = true;
            }
        }

        private static string FormatPipe(string server, string pipeName)
        {
            switch (Environment.OSVersion.Platform)
            {
                default:
                case PlatformID.Win32NT:
                    return string.Format(@"\\{0}\pipe\{1}", server, pipeName);

                case PlatformID.Unix:
                    if (server != ".")
                        throw new PlatformNotSupportedException("Remote pipes are not supported on this platform.");                    
                    return pipeName;
            }
        }
               
        #endregion

        #region Open Close
        /// <summary>
        /// Attempts to open a named pipe.
        /// </summary>
        /// <param name="name">The name of the pipe</param>
        public void Connect()
        {
            int code = Native.Open(ptr, PipeName);
            if (!IsConnected)  throw new NamedPipeOpenException(code);
            
        }

        /// <summary>
        /// Closes the named pipe already opened.
        /// </summary>
        public void Disconnect()
        {
            Native.Close(ptr);
        }
        #endregion

        /// <summary>
        /// Reads a block of bytes from a stream and writes the data to a specified buffer. Will not block if there is no data available to read.
        /// </summary>
        /// <param name="buffer">When this method returns, contains the specified byte array with the values between offset and (offset + count - 1) replaced by the bytes read from the current source.</param>
        /// <param name="offset">The byte offset in the buffer array at which the bytes that are read will be placed.</param>
        /// <param name="count">The maximum number of bytes to read.</param>
        /// <returns>The total number of bytes that are read into buffer. This might be less than the number of bytes requested if that number of bytes is not currently available. If the value is less than 0, then a error has occured.</returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            //Make sure we are connected
            if (!IsConnected)
                throw new NamedPipeConnectionException("Cannot read stream as pipe is not connected");

            if (offset + count > buffer.Length)
                throw new ArgumentOutOfRangeException("count", "Cannot read as the count exceeds the buffer size");

            //Prepare bytes read and a buffer of memory to read into
            int bytesRead = 0;
            int size = Marshal.SizeOf(buffer[0]) * count;
            IntPtr buffptr = Marshal.AllocHGlobal(size);

            try
            {
                //Read the bytes
                bytesRead = Native.ReadFrame(ptr, buffptr, count);
                if (bytesRead <= 0)
                {
                    //A error actively occured. If it is 0 we just read no bytes.
                    if (bytesRead < 0)
                    {
                        //We have a pretty bad error, we will log it for prosperity. 
                        throw new NamedPipeReadException(bytesRead);
                    }

                    //Return a empty frame and return false (read failure).
                    return 0;
                }
                else
                {
                    //WE have read a valid amount of bytes, so copy the marshaled bytes over to the buffer
                    Marshal.Copy(buffptr, buffer, offset, bytesRead);
                    return bytesRead;
                }
            }
            finally
            {
                //Finally, before we exit this try block, free the pointer we allocated.
                Marshal.FreeHGlobal(buffptr);
            }
        }

        /// <summary>
        /// Writes a block of bytes to the current stream using data from a buffer
        /// </summary>
        /// <param name="buffer">The buffer that contains data to write to the pipe.</param>
        /// <param name="offset">The zero-based byte offset in buffer at which to begin copying bytes to the current stream.</param>
        /// <param name="count">The maximum number of bytes to write to the current stream.</param>
        public override void Write(byte[] buffer, int offset, int count)
        {
            //Make sure we are connected
            if (!IsConnected)
                throw new NamedPipeConnectionException("Cannot write stream as pipe is not connected");

            //Copy the bytes into a new marshaled block
            int size = Marshal.SizeOf(buffer[0]) * count;
            IntPtr buffptr = Marshal.AllocHGlobal(size);

            try
            {
                //Copy the block of memory over
                Marshal.Copy(buffer, offset, buffptr, count);

                //Send the block
                int result = Native.WriteFrame(ptr, buffptr, count);
                if (result < 0) throw new NamedPipeWriteException(result);                
            }
            finally
            {
                //Finally, before exiting the try catch, free the memory we assigned.
                Marshal.FreeHGlobal(buffptr);
            }
        }

        #region unsupported

        /// <summary>
        /// Flushes the stream. Not supported by NamedPipeClient.
        /// </summary>
        public override void Flush()
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Seeks to the given posisiton. Not supported by NamedPipeClient
        /// </summary>
        /// <param name="offset"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException();
        }

        /// <summary>
        /// Sets the length of the stream. Not supported by NamedPipeClient
        /// </summary>
        /// <param name="value"></param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        #endregion

        private static class Native
        {
            const string LIBRARY_NAME = "NativeNamedPipe";

            #region Creation and Destruction
            [DllImport(LIBRARY_NAME, EntryPoint = "createClient", CallingConvention = CallingConvention.Cdecl, ExactSpelling = false)]
            public static extern IntPtr CreateClient();

            [DllImport(LIBRARY_NAME, EntryPoint = "destroyClient", CallingConvention = CallingConvention.Cdecl)]
            public static extern void DestroyClient(IntPtr client);
            #endregion

            #region State Control

            [DllImport(LIBRARY_NAME, EntryPoint = "isConnected", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern bool IsConnected([MarshalAs(UnmanagedType.SysInt)] IntPtr client);

            [DllImport(LIBRARY_NAME, EntryPoint = "open", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern int Open(IntPtr client, [MarshalAs(UnmanagedType.LPStr)] string pipename);

            [DllImport(LIBRARY_NAME, EntryPoint = "close", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl)]
            public static extern void Close(IntPtr client);

            #endregion

            #region IO
            
            [DllImport(LIBRARY_NAME, EntryPoint = "readFrame", CallingConvention = CallingConvention.Cdecl)]
            public static extern int ReadFrame(IntPtr client, IntPtr buffer, int length);

            [DllImport(LIBRARY_NAME, EntryPoint = "writeFrame", CallingConvention = CallingConvention.Cdecl)]
            public static extern int WriteFrame(IntPtr client, IntPtr buffer, int length);

            #endregion
        }
    }
}