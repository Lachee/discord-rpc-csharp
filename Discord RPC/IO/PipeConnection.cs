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

		public bool IsOpen {  get { return stream != null && stream.IsConnected; } }

		public int PipeNumber { get { return _pipeno; } }
		private int _pipeno;

		private NamedPipeClientStream stream;
		private BinaryReader reader;

		public bool Open()
		{
			int pipeDigit = 0;

			while (pipeDigit < 10)
			{ 
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, pipeDigit);
					DiscordClient.WriteLog("Attempting {0}", pipename);

					//Create the client
					stream = new NamedPipeClientStream(pipename);
					stream.Connect(1000);

					//We have made a connection, prepare the writers
					DiscordClient.WriteLog("Connected to pipe " + pipename);
					_pipeno = pipeDigit;

					reader = new BinaryReader(stream);

					break;
				}
				catch (Exception e)
				{
					//Something happened, try again
					DiscordClient.WriteLog("Connection Exception: {0}", e.Message);
					stream = null;

					pipeDigit++;
				}
			}

			//Check if we succeded
			return stream != null;
		}

		public bool Close()
		{
			DiscordClient.WriteLog("Closing Pipe");
		
			if (stream != null)
			{
				stream.Dispose();
				stream = null;
			}
			
			return true;
		}
		
		public void Dispose()
		{
			this.Close();
		}
		
		public bool CanRead()
		{
			return reader.PeekChar() >= 0;
		}

		public int Read(byte[] buff, int length)
		{
			if (!IsOpen) return 0;			
			return reader.Read(buff, 0, length);
		}

		public int ReadInt()
		{
			//Read the bytes
			byte[] buff = new byte[4];
			Read(buff, buff.Length);

			//Flip if required
			if (!BitConverter.IsLittleEndian) Array.Reverse(buff);

			//Convert to a int
			int value = BitConverter.ToInt32(buff, 0);
			return value;
		}

		public string ReadString(int length, Encoding encoding)
		{
			//Read the bytes
			byte[] buff = new byte[length];
			Read(buff, length);

			string message =  encoding.GetString(buff);
			return message;
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
			
			stream.Write(data, 0, data.Length);
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
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return Write(bytes);
		}
	}
}
