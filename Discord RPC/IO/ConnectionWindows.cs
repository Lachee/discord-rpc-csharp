using System;
using System.IO;
using System.IO.Pipes;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	internal class ConnectionWindows : IConnection
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
		
		public void Dispose()
		{
			this.Close();
		}

		public int Read(byte[] buff, int length)
		{
			if (!IsOpen) return 0;
			return client.Read(buff, 0, length);
		}

		public bool Write(byte[] data)
		{
			if (!IsOpen) return false;
			client.Write(data, 0, data.Length);
			return true;
		}
	}
}
