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
		private int _pipeno;

		public event PipeReadEvent OnPipeRead;

		#region Pipe Management

		public bool Connect()
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, i);

					//Create the client
					_stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
					_stream.Connect(1000);
				
					//Spin for a bit while we wait for it to finish connecting
					while (!_stream.IsConnected) { Thread.Sleep(250); }
					
					//Store the value
					return true;
				}
				catch (Exception)
				{
					//Something happened, try again
					//TODO: Log the failure condition
					_stream = null;
				}
			}

			return _stream == null;
		}

		#endregion
		

		#region Frame Write

		public void WriteFrame(PipeFrame frame)
		{
			//u_stream is multithread friendly, so we can just write directly
			Write(frame);
		}

		#endregion

		#region IO Operation
		
		#region Read
		[System.Obsolete("Use TryReadFrame instead.")]
		public PipeFrame ReadFrame()
		{
			int op = ReadInt32();
			int len = ReadInt32();

			if (op < 0)
			{
				//TODO: Throw Error Event
				return new PipeFrame() { Opcode = Opcode.Close };
			}

			if (len < 0)
			{
				//TODO: THrow Error Event
				return new PipeFrame() { Opcode = Opcode.Close };
			}
			
			byte[] buff = new byte[len];
			Read(buff, len);

			return new PipeFrame()
			{
				Opcode = (Opcode)op,
				Data = buff
			};
		}

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
		private void Write(PipeFrame frame)
		{
			Write((int)frame.Opcode);
			Write(frame.Length);
			Write(frame.Data);
		}
		private void Write(byte[] buff)
		{
			_stream.Write(buff, 0, buff.Length);
		}
		private void Write(int i)
		{
			byte[] bytes = BitConverter.GetBytes(i);
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			Write(bytes);
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
			}
		}
	}
}
