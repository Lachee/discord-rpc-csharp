using DiscordRPC.IO.Events;
using System;
using System.IO.Pipes;
using System.Threading;

namespace DiscordRPC.IO
{
	internal class PipeConnection : IDisposable
	{
		/// <summary>
		/// Discord Pipe Name
		/// </summary>
		const string PIPE_NAME = @"discord-ipc-{0}";
		private NamedPipeClientStream _stream;

		public bool IsConnected { get { return _isconnected; } }
		private bool _isconnected = false;

		#region Pipe Management

		public bool AttemptConnection()
		{
			for (int i = 0; i < 10; i++)
			{
				//Prepare the pipe name
				string pipename = string.Format(PIPE_NAME, i);
				Console.WriteLine("Attempting to connect to " + pipename);

				try
				{
					//Create the client
					_stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
					_stream.Connect(1000);

					//Spin for a bit while we wait for it to finish connecting
					Console.WriteLine("Waiting for connection...");
					do { Thread.Sleep(250); } while (!_stream.IsConnected);

					//Store the value
					Console.WriteLine("Connected to " + pipename);
					_isconnected = true;
					return true;
				}
				catch (Exception e)
				{
					//Something happened, try again
					//TODO: Log the failure condition
					Console.WriteLine("Failed connection to {0}. {1}", pipename, e.Message);
					_isconnected = false;
					_stream = null;
				}
			}

			//We are succesfull if the stream isn't null
			return _stream == null;
		}

		#endregion
		

		#region Frame Write

		public bool WriteFrame(PipeFrame frame)
		{
			//u_stream is multithread friendly, so we can just write directly
			return Write(frame);
		}

		#endregion

		#region IO Operation
		
		#region Read
		public bool TryReadFrame(out PipeFrame frame)
		{
			//Set the pipe frame to default
			frame = default(PipeFrame);

			//Try to read the values
			int op = ReadInt32();
			if (op < 0) return false;

			int len = ReadInt32();
			if (len < 0) return false;

			//Read the data
			byte[] buff = new byte[len];
			Read(buff, len);

			//Create the frame
			frame = new PipeFrame()
			{
				Opcode = (Opcode)op,
				Data = buff
			};

			//Success!
			return true;
		}

		private int Read(byte[] buff, int length) { return _stream.Read(buff, 0, length); }
		private int ReadInt32()
		{
			//Read the bytes
			byte[] bytes = new byte[4];
			int cnt = Read(bytes, 4);
			if (cnt != 4) return -1;
			
			//Convert to int
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return BitConverter.ToInt32(bytes, 0);
		}
		#endregion

		#region Write
		private bool Write(PipeFrame frame)
		{
			if (!Write((int)frame.Opcode)) return false;
			if (!Write(frame.Length)) return false;
			if (!Write(frame.Data)) return false;
			return true;
		}
		private bool Write(byte[] buff)
		{
			try
			{
				_stream.Write(buff, 0, buff.Length);
				return true;
			}
			catch (Exception e)
			{
				//TODO: Replace with error event
				Console.WriteLine("An error has occured in the pipe: {0}", e.Message);
				return false;
			}
		}
		private bool Write(int int32)
		{
			byte[] bytes = BitConverter.GetBytes(int32);
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return Write(bytes);
		}
		#endregion
	
		#endregion

		public void Dispose()
		{
			//Abort the thread. The thread will manage everything else automatically
			if (_stream != null)
			{
				_stream.Dispose();
				_stream = null;
				_isconnected = false;
			}
		}
	}
}
