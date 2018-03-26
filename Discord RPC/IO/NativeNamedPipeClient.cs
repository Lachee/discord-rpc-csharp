using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
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
		
		private int[] _buffer = new int[PipeFrame.MAX_SIZE];
		
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

			throw new NotImplementedException();
		}
		
		public bool ReadFrame(out PipeFrame frame)
		{
			if (!IsConnected)
			{
				frame = default(PipeFrame);
				return false;
			}

			throw new NotImplementedException();
		}

		public bool WriteFrame(PipeFrame frame)
		{
			if (!IsConnected)
			{
				frame = default(PipeFrame);
				return false;
			}

			throw new NotImplementedException();
		}

		public void Close()
		{
			throw new NotImplementedException();
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
		public static extern bool ReadFrame(int[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "writeFrame")]
		public static extern bool WriteFrame(int[] buffer, int length);

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "close")]
		public static extern bool Close();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "open")]
		public static extern bool Open(char[] pipename);
	}
}
