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

		public ILogger Logger { get; set; }

		public bool IsConnected { get { return NativePipe.IsConnected(); } }

		public int ConnectedPipe { get { return _connectedPipe; } }
		private int _connectedPipe;
		
		private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];
		
		public bool Connect(int pipe)
		{
			if (IsConnected)
				throw new InvalidPipeException("Cannot connect as the pipe is already connected");

			if (pipe > 9)
				throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");

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
				throw new InvalidPipeException("Cannot connect as the pipe is already connected");

			try
			{
				//Prepare the pipe name
				string pipename = string.Format(PIPE_NAME, pipe);
				Logger.Info("Attempting to connect to " + pipename);

				byte[] bytes = Encoding.ASCII.GetBytes(pipename);
				uint err = NativePipe.Open(bytes);
				if (err == 0 && IsConnected)
				{
					_connectedPipe = pipe;
					return true;
				}
				else
				{
					Logger.Error("Failed to connect to native pipe. Err: {0}", err);
					return false;
				}

			}
			catch (Exception e)
			{
				Logger.Error("Error occured while connecting to native pipe. {0}", e.Message);
			}


			return false;
		}
		
		public bool ReadFrame(out PipeFrame frame)
		{
			//Make sure we are connected
			if (!IsConnected)
				throw new InvalidPipeException("Cannot read Native Stream as pipe is not connected");

			//Try and read the frame from the native pipe
			int bytesRead = NativePipe.ReadFrame(_buffer, _buffer.Length);
			if (bytesRead <= 0)
			{
				//A error actively occured. If it is 0 we just read no bytes.
				if (bytesRead < 0)
					Logger.Error("Native pipe failed to read. Err: {0}", bytesRead);

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

		public void Close()
		{
			NativePipe.Close();
		}

		public void Dispose()
		{
			//Close the stream (disposing of it too)
			Close();
		}
	}

	internal static class NativePipe
	{
		[DllImport("DiscordRPC.Native.dll", EntryPoint = "isConnected", CallingConvention =  CallingConvention.Cdecl)]
		public static extern bool IsConnected();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "readFrame", CallingConvention = CallingConvention.Cdecl)]
		public static extern int ReadFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "writeFrame", CallingConvention = CallingConvention.Cdecl)]
		public static extern bool WriteFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "close", CallingConvention = CallingConvention.Cdecl)]
		public static extern void Close();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "open", CallingConvention = CallingConvention.Cdecl)]
		public static extern UInt32 Open(byte[] pipename);
	}
}
