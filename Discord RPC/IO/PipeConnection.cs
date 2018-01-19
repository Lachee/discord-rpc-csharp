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

		public int ReadInt()
		{
			//Read the bytes
			byte[] buff = new byte[4];
			Read(buff, buff.Length);

			//Flip if required
			if (BitConverter.IsLittleEndian) Array.Reverse(buff);

			//Convert to a int
			return BitConverter.ToInt32(buff, 0);
		}

		public string ReadString(int length, Encoding encoding)
		{
			//Read the bytes
			byte[] buff = new byte[length];
			Read(buff, length);
			return encoding.GetString(buff);
		}

		public string ReadString(Encoding encoding)
		{
			//Read the length of the string
			int length = ReadInt();
			return ReadString(length, encoding);
		}

		public bool Write(byte[] data)
		{
			if (!IsOpen) return false;
			client.Write(data, 0, data.Length);
			return true;
		}

		public bool Write(string data, Encoding encoding, bool includeLength = false)
		{
			byte[] bytes = encoding.GetBytes(data);
			if (includeLength) Write(bytes.Length);
			return Write(bytes);
		}

		public bool Write(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			if (BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return Write(bytes);
		}
	}
}
