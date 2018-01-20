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

		public bool IsOpen {  get { return _stream != null && _stream.IsConnected; } }

		public int PipeNumber { get { return _pipeno; } }
		private int _pipeno;

		private NamedPipeClientStream _stream;
		private BinaryReader _reader;
		private BinaryWriter _writer;

		public BinaryReader Reader { get { return _reader; } }
		public BinaryWriter Writer { get { return _writer; } }

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
					_stream = new NamedPipeClientStream(pipename);
					_stream.Connect(1000);

					//We have made a connection, prepare the writers
					DiscordClient.WriteLog("Connected to pipe " + pipename);
					_pipeno = pipeDigit;

					_reader = new BinaryReader(_stream);
					_writer = new BinaryWriter(_stream);

					break;
				}
				catch (Exception e)
				{
					//Something happened, try again
					DiscordClient.WriteLog("Connection Exception: {0}", e.Message);
					_stream = null;

					pipeDigit++;
				}
			}

			//Check if we succeded
			return _stream != null;
		}

		public bool Close()
		{
			DiscordClient.WriteLog("Closing Pipe");

			if (_reader != null)
			{
				_reader.Dispose();
				_reader = null;
			}

			if (_writer != null)
			{
				_writer.Dispose();
				_writer = null;
			}

			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
			}
			
			return true;
		}
		
		public void Dispose()
		{
			this.Close();
		}


		/*
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
			if (!IsOpen) return 0;
			return reader.ReadInt32();
		}
		
		public string ReadString(int length, Encoding encoding)
		{
			return reader.ReadString();
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
		*/
	}
}
