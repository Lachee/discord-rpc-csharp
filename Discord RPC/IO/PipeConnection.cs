using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	internal class PipeConnection
	{
		const string PIPE_NAME = @"discord-ipc-{0}";

		public bool IsOpen {  get { return client != null && client.IsConnected; } }

		public int PipeNumber { get { return _pipeno; } }
		private int _pipeno;
		private NamedPipeClientStream client;

		public bool Open()
		{
			int pipeDigit = 0;

			while (true)
			{ 
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, pipeDigit);
					Console.WriteLine("Attempting {0}", pipename);
					
					//Create the client
					client = new NamedPipeClientStream(".", pipename, PipeDirection.InOut);
					client.Connect(10000);
					
					Console.WriteLine("Connected to pipe.");
					
					_pipeno = pipeDigit;
					break;
				}
				catch (Exception e)
				{
					//Something happened, try again
					Console.WriteLine("Exception: {0}", e.Message);
					client = null;

					pipeDigit++;
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

		public bool Write(string data)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(data);
			return Write(bytes);
		}

		public bool Write(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			return Write(bytes);
		}
	}
}
