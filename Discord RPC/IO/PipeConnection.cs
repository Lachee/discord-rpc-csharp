using System;
using System.IO;
using System.IO.Pipes;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	internal class PipeConnection
	{
		/// <summary>
		/// Discord Pipe Name
		/// </summary>
		const string PIPE_NAME = @"discord-ipc-{0}";

		/// <summary>
		/// Has the pipe connectced to discord?
		/// </summary>
		public bool IsOpen {  get { return stream != null && stream.IsConnected; } }

		/// <summary>
		/// The current pipe we are connected to
		/// </summary>
		public int PipeNumber { get { return _pipeno; } }
		private int _pipeno;

		private NamedPipeClientStream stream;

		/// <summary>
		/// Opens a new pipe stream to discord
		/// </summary>
		/// <returns></returns>
		public bool Open()
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, i);
					DiscordClient.WriteLog("Attempting {0}", pipename);

					//Create the client
					stream = new NamedPipeClientStream(pipename);
					stream.Connect(100);

					while (!stream.IsConnected) { Task.Delay(100); }

					//We have made a connection, prepare the writers
					DiscordClient.WriteLog("Connected to pipe " + pipename);
					_pipeno = i;

					//We have succesfully connected
					return IsOpen;
				}
				catch (Exception e)
				{
					//Something happened, try again
					DiscordClient.WriteLog("Connection Exception: {0}", e.Message);
					stream = null;
				}
			}

			//Check if we succeded
			return stream != null;
		}
		
		#region Disposal
		/// <summary>
		/// Closes the pipe stream.
		/// </summary>
		/// <returns></returns>
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
		
		/// <summary>
		/// Disposes the pipe stream. 
		/// </summary>
		public void Dispose()
		{
			this.Close();
		}
		#endregion

		#region Read
		public int Read(byte[] buff, int length)
		{
			if (!IsOpen) return -1;
			return stream.Read(buff, 0, length);
		}
		public int ReadInt()
		{
			if (!IsOpen) return -1;

			//Read the bytes
			byte[] buff = new byte[4];
			Read(buff, buff.Length);

			//Flip if required
			if (!BitConverter.IsLittleEndian) Array.Reverse(buff);

			//Convert to a int
			int value = BitConverter.ToInt32(buff, 0);
			return value;
		}
		public string ReadString(Encoding encoding)
		{
			if (!IsOpen) return null;

			//Read the length
			int length = ReadInt();

			//Read the bytes
			byte[] buff = new byte[length];
			Read(buff, length);

			string message = encoding.GetString(buff);
			return message;
		}


		public async Task<int> ReadAsync(byte[] buff, int length)
		{
			if (!IsOpen) return -1;
			return await stream.ReadAsync(buff, 0, length);
		}
		public async Task<int> ReadIntAsync()
		{
			if (!IsOpen) return -1;

			//Read the bytes
			byte[] buff = new byte[4];
			await ReadAsync(buff, buff.Length);

			//Flip if required
			if (!BitConverter.IsLittleEndian) Array.Reverse(buff);

			//Convert to a int
			int value = BitConverter.ToInt32(buff, 0);
			return value;
		}
		public async Task<string> ReadStringAsync(Encoding encoding)
		{
			if (!IsOpen) return null;

			//Read the length
			int length = await ReadIntAsync();

			//Read the bytes
			byte[] buff = new byte[length];
			Read(buff, length);

			string message = encoding.GetString(buff);
			return message;
		}
		#endregion
		#region Write
		public bool Write(byte[] data)
		{
			if (!IsOpen) return false;

			stream.Write(data, 0, data.Length);
			return true;
		}
		public bool Write(string data, Encoding encoding)
		{
			byte[] bytes = encoding.GetBytes(data);
			Write(bytes.Length);
			return Write(bytes);
		}
		public bool Write(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return Write(bytes);
		}
		
		public async Task WriteAsync(byte[] data)
		{
			if (!IsOpen) return;
			await stream.WriteAsync(data, 0, data.Length);
		}
		public async Task WriteAsync(int data)
		{
			byte[] bytes = BitConverter.GetBytes(data);
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			await WriteAsync(bytes);
		}
		public async Task WriteAsync(string data, Encoding encoding)
		{
			byte[] bytes = encoding.GetBytes(data);
			await WriteAsync(bytes.Length);
			await WriteAsync(bytes);
		}
		#endregion
	}
}
