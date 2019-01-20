using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordRPC.IO
{
	/// <summary>
	/// Pipe Client used to communicate with Discord.
	/// </summary>
	public class NativeNamedPipeClient : INamedPipeClient
	{
		const string PIPE_NAME = @"\\?\pipe\discord-ipc-{0}";

		/// <summary>
		/// The logger of this pipe
		/// </summary>
		public ILogger Logger { get; set; }
		
		/// <summary>
		/// Is the pipe currently connected?
		/// </summary>
		public bool IsConnected { get { return NativePipe.IsConnected(); } }

		/// <summary>
		/// The pipe number we are connected too
		/// </summary>
		public int ConnectedPipe { get { return _connectedPipe; } }
		private int _connectedPipe;
		
		private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

		/// <summary>
		/// The last pipe error read.
		/// </summary>
		internal NativePipe.PipeReadError LastError { get { return _lasterr; } }
		private NativePipe.PipeReadError _lasterr = NativePipe.PipeReadError.None;

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
				throw new InvalidPipeException("Cannot connect as the pipe is already connected");
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
				throw new InvalidPipeException("Cannot connect as the pipe is already connected");
			}

			//Prepare the pipe name
			string pipename = string.Format(PIPE_NAME, pipe);
			Logger.Info("Attempting to connect to " + pipename);
			
			uint err = NativePipe.Open(pipename);
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
				throw new InvalidPipeException("Cannot read Native Stream as pipe is not connected");

			//Try and read the frame from the native pipe
			int bytesRead = NativePipe.ReadFrame(_buffer, _buffer.Length);
			if (bytesRead <= 0)
			{
				//Update the error message
				_lasterr = NativePipe.PipeReadError.ReadEmptyMessage;

				//A error actively occured. If it is 0 we just read no bytes.
				if (bytesRead < 0)
				{
					//We have a pretty bad error, we will log it for prosperity. 
					_lasterr = (NativePipe.PipeReadError)bytesRead;
					Logger.Error("Native pipe failed to read: {0}", _lasterr.ToString());

					//Close this pipe
					this.Close();
				}

				//Return a empty frame and return false (read failure).
				frame = default(PipeFrame);
				return false;
			}

			//Parse the pipe
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
				throw new InvalidPipeException("Cannot write Native Stream as pipe is not connected");

			//Create a memory stream so we can write it to the pipe
			using (MemoryStream stream = new MemoryStream())
			{
				//Write the stream and the send it to the pipe
				frame.WriteStream(stream);

				//Get the bytes and send it
				byte[] bytes = stream.ToArray();
				return NativePipe.WriteFrame(bytes, bytes.Length);
			}
		}

		/// <summary>
		/// Closes the pipe
		/// </summary>
		public void Close()
		{
			NativePipe.Close();
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

	internal static class NativePipe
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
		
		[DllImport("DiscordRPC.Native.dll", EntryPoint = "isConnected", CallingConvention =  CallingConvention.Cdecl)]
		public static extern bool IsConnected();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "readFrame", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ReadFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "writeFrame", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WriteFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "close", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Close();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "open", CallingConvention = CallingConvention.Cdecl)]
		public static extern UInt32 Open(string pipename);
	}
}
