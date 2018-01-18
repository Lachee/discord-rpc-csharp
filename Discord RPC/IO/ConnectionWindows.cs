using System;
using System.IO;
using System.IO.Pipes;

namespace DiscordRPC.IO
{
	public class ConnectionWindows : IConnection
	{
		const string PIPE_NAME = "discord-ipc-{0}";

		public bool IsOpen {  get { return client != null && client.IsConnected; } }

		public int PipeNumber { get { return _pipeno; } }
		private int _pipeno;

		private NamedPipeClientStream client;

		public bool Open()
		{
			for (int i = 0; i <= 9; i++)
			{
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, i);

					//Create the client
					client = new NamedPipeClientStream(PIPE_NAME);
					_pipeno = i;
					break;
				}
				catch (Exception e)
				{
					//Something happened, try again
					Console.WriteLine("Exception: {0}", e.Message);
					client = null;
				}
			}

			//Check if we succeded
			return client != null;
		}

		public bool Close()
		{
			client.Dispose();
			client = null;
			return true;
		}

		public void Destroy() { this.Close(); }

		public void Dispose()
		{
			throw new NotImplementedException();
		}

		public bool Read(out byte[] data, int length)
		{
			throw new NotImplementedException();
		}

		public bool Write(byte[] data)
		{
			throw new NotImplementedException();
		}
	}
}
