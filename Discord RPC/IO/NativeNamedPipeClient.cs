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
		const string PIPE_NAME = @"discord-ipc-{0}";

		public ILogger Logger { get; set; }

		public bool IsConnected { get { return e_isconnected; } }
		private static bool e_isconnected = false;

		public int ConnectedPipe { get { return _connectedPipe; } }
		private int _connectedPipe;
		
		private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];
		
		public bool Connect(int pipe)
		{
			if (pipe > 9)
				throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");

			//Attempt to connect to the specific pipe
			if (pipe >= 0 && AttemptConnection(pipe))
			{
				BeginRead();
				return true;
			}

			//Iterate until we connect to a pipe
			for (int i = 0; i < 9; i++)
			{
				if (AttemptConnection(i))
				{
					BeginRead();
					return true;
				}
			}

			//We failed to connect
			return false;
		}
		private bool AttemptConnection(int pipe)
		{
			//Prepare the pipe name
			string pipename = string.Format(PIPE_NAME, pipe);
			Logger.Info("Attempting to connect to " + pipename);

			return NativePipe.Open(pipename.ToCharArray());
		}
		
		public bool ReadFrame(out PipeFrame frame)
		{
			//Make sure we are connected
			if (!IsConnected)
			{
				frame = default(PipeFrame);
				return false;
			}
			
			//Try and read the frame from the native pipe
			if (!NativePipe.ReadFrame(_buffer, _buffer.Length))
			{
				frame = default(PipeFrame);
				return false;
			}

			//Parse the pipe
			using (MemoryStream stream = new MemoryStream(_buffer, 0, _buffer.Length))
			{
				//Try to parse the stream
				frame = new PipeFrame();
				if (frame.ReadStream(stream))
					return true;
				
				//We failed
				Logger.Error("Pipe failed to read from the data received by the stream.");
				return false;
			}
		}

		public bool WriteFrame(PipeFrame frame)
		{
			if (!IsConnected)
			{
				frame = default(PipeFrame);
				return false;
			}

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
		[DllImport("DiscordRPC.Native.dll", EntryPoint = "isConnected")]
		public static extern bool IsConnected();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "readFrame")]
		public static extern bool ReadFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "writeFrame")]
		public static extern bool WriteFrame(byte[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "close")]
		public static extern void Close();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "open")]
		public static extern bool Open(char[] pipename);
	}
}
