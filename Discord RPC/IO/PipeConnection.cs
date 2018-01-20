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
		private BinaryWriter writer;
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

					writer = new BinaryWriter(stream);
					reader = new BinaryReader(stream);

					break;
				}
				catch (Exception e)
				{
					//Something happened, try again
					DiscordClient.WriteLog("Exception: {0}", e.Message);
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

			if (writer != null)
			{
				writer.Dispose();
				writer = null;
			}

			if (reader != null)
			{
				reader.Dispose();
				reader = null;
			}

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

		public int Read(byte[] buff, int length)
		{
			DiscordClient.WriteLog("Reading {0} bytes", length);
			if (!IsOpen) return 0;
			return stream.Read(buff, 0, length);
		}

		public int ReadInt()
		{
			DiscordClient.WriteLog("Reading Int");

			//Read the bytes
			byte[] buff = new byte[4];
			Read(buff, buff.Length);

			//Flip if required
			if (!BitConverter.IsLittleEndian) Array.Reverse(buff);

			//Convert to a int
			int value = BitConverter.ToInt32(buff, 0);
			DiscordClient.WriteLog(" - Value: {0}", value);

			return value;
		}

		public string ReadString(int length, Encoding encoding)
		{
			DiscordClient.WriteLog("Reading String of size {0}", length);

			//Read the bytes
			byte[] buff = new byte[length];
			Read(buff, length);

			string message =  encoding.GetString(buff);
			DiscordClient.WriteLog(" - Value: {0}", message);

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
			if (!IsOpen)
			{
				DiscordClient.WriteLog("Cannot write bytes as we are not open!");
				return false;
			}

			DiscordClient.WriteLog("Writing {0} bytes", data.Length);
			stream.Write(data, 0, data.Length);
			//stream.Flush();
			return true;
		}

		public bool Write(string data, Encoding encoding, bool includeLength = false)
		{
			DiscordClient.WriteLog("Writing String {0}", data);

			byte[] bytes = encoding.GetBytes(data);
			if (includeLength) Write(bytes.Length);
			return Write(bytes);
		}

		public bool Write(int data)
		{
			DiscordClient.WriteLog("Writing Int {0}", data);

			byte[] bytes = BitConverter.GetBytes(data);
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return Write(bytes);
		}
	}
}
