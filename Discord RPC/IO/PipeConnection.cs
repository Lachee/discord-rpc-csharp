using DiscordRPC.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Threading;

namespace DiscordRPC.IO
{
	//TODO: Make Internal
	public class PipeConnection : IDisposable
	{
		/// <summary>
		/// Discord Pipe Name
		/// </summary>
		const string PIPE_NAME = @"discord-ipc-{0}";
		private NamedPipeClientStream _stream;

		public int ConnectedPipe { get; private set; }

		public bool IsConnected { get { return _isconnected; } }
		private bool _isconnected = false;

		public ILogger Logger { get { return _logger; } set { _logger = value; } }
		public ILogger _logger = new NullLogger();

		private object l_rxFrames = new object();
		private Queue<PipeFrame> _rxFrames = new Queue<PipeFrame>();

		public volatile bool isReading = false;
		#region Pipe Management

		/// <summary>
		/// Attempts to establish a connection to the Discord Client
		/// </summary>
		/// <param name="pipe">The pipe the discord client is located on. Set to -1 for any available pipe.</param>
		/// <returns></returns>
		public bool AttemptConnection(int pipe)
		{
			if (pipe < 0)
			{
				//Iterate over each pipe, trying to connect. If we connect, end the loop and return true.
				for (int i = 0; i < 10; i++)
					if (CreateConnection(i)) return true;

				//We failed to conect, so return false
				return false;
			}
			else
			{
				//Attempt to connect to the target pipe
				return CreateConnection(pipe);
			}
		}

		private bool CreateConnection(int pipe)
		{
			//Prepare the pipe name
			string pipename = string.Format(PIPE_NAME, pipe);
			Logger.Info("Attempting to connect to " + pipename);

			try
			{
				//Create the client
				_stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut, PipeOptions.Asynchronous);
				_stream.Connect(1000);
				
				//Spin for a bit while we wait for it to finish connecting
				Logger.Info("Waiting for connection...");
				do { Thread.Sleep(250); } while (!_stream.IsConnected);

				//Store the value
				Logger.Info("Connected to " + pipename);
				ConnectedPipe = pipe;
				_isconnected = true;
				return true;
			}
			catch (Exception e)
			{
				//Something happened, try again
				//TODO: Log the failure condition
				Logger.Error("Failed connection to {0}. {1}", pipename, e.Message);
				_isconnected = false;
				_stream = null;
			}

			//We are succesfull if the stream isn't null
			return _stream == null;
		}
		#endregion
		

		#region IO Operation

		#region Read

		public bool DequeueFrame(out PipeFrame frame)
		{
			lock (l_rxFrames)
			{
				if (_rxFrames.Count == 0)
				{
					frame = default(PipeFrame);
					return false;
				}
				else
				{
					frame = _rxFrames.Dequeue();
					return true;
				}
			}
		}

		private byte[] buffer = new byte[PipeFrame.MAX_SIZE];
		public void BeginRead()
		{
			if (_stream == null) return;
			isReading = true;
			_stream.BeginRead(buffer, 0, buffer.Length, new AsyncCallback(EndReadCallback), _stream);
		}

		private void EndReadCallback(IAsyncResult result)
		{
			Logger.Info("Finished a callback");

			//The stream has been closed, so dont continue anymore.
			if (_stream == null) return;

			Logger.Info(" ----");

			//Get how many bytes we have read and chuck em into the memory stream
			int bytesRead = _stream.EndRead(result);
			using (MemoryStream mem = new MemoryStream(buffer, 0, bytesRead))
			{
				//Attempt to extract the frame from the stream
				PipeFrame frame;
				if (TryReadFrame(mem, out frame))
				{
					//Enqueue the frame we just read
					lock (l_rxFrames)
						_rxFrames.Enqueue(frame);
				}
				else
				{
					//Something went wrong. We could have potentially aborted.
					Logger.Error("Something went wrong while trying to read a frame");
				}
			}

			//Read more in the buffer
			if (isReading) BeginRead();
		}

		public bool TryReadFrame(out PipeFrame frame)
		{
			return TryReadFrame(_stream, out frame);
		}

		private bool TryReadFrame(Stream stream, out PipeFrame frame)
		{
			//Set the pipe frame to default
			frame = default(PipeFrame);
			
			//Try to read the values
			uint op;
			if (!TryReadUInt32(stream, out op))
			{
				Logger.Error("Bad OpCode");
				return false;
			}

			uint len;
			if (!TryReadUInt32(stream, out len))
			{
				Logger.Error("Bad Length");
				return false;
			}


			//Read the data. This could potentially cause issues if we ever get anything greater than a int.
			//TODO: Better implementation of this read using uints
			byte[] buff = new byte[len];
			int bytesread = Read(stream, buff, (int)len);

			if (bytesread != len)
			{
				Logger.Error("Bad Data");
				return false;
			}

			//Create the frame
			frame = new PipeFrame()
			{
				Opcode = (Opcode)op,
				Data = buff
			};

			//Success!
			return true;
		}
		
		private int Read(Stream stream, byte[] buff, int length) { return stream.Read(buff, 0, length); }
		private bool TryReadUInt32(Stream stream, out uint value)
		{
			//Read the bytes
			byte[] bytes = new byte[4];
			int cnt = Read(stream, bytes, 4);
			if (cnt != 4)
			{
				Logger.Error("Did not ready 4 bytes!");
				value = 0;
				return false;
			}

			//Convert to int
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			value = BitConverter.ToUInt32(bytes, 0);
			return true;
		}
		#endregion

		#region Write
		/// <summary>
		/// Writes the handshake to the connection
		/// </summary>
		/// <param name="version">Version of the IPC protocol</param>
		/// <param name="client">The client ID</param>
		/// <returns></returns>
		public bool WriteHandshake(int version, string client)
		{
			PipeFrame frame = new PipeFrame();
			frame.SetObject(Opcode.Handshake, new Handshake() { Version = version, ClientID = client });

			return WritePipeFrame(frame);
		}

		public bool WritePipeFrame(PipeFrame frame)
		{
			//Get all the bytes
			byte[] op = ConvertBytes((uint)frame.Opcode);
			byte[] len = ConvertBytes(frame.Length);
			byte[] data = frame.Data;

			//Copy it all into a buffer
			byte[] buffer = new byte[op.Length + len.Length + data.Length];
			op.CopyTo(buffer, 0);
			len.CopyTo(buffer, op.Length);
			data.CopyTo(buffer, op.Length + len.Length);

			//Write it to the stream
			//_stream.Write(buffer, 0, buffer.Length);
			_stream.BeginWrite(buffer, 0, buffer.Length, new AsyncCallback(EndWriteCallback), _stream);
			return true;
		}

		private void EndWriteCallback(IAsyncResult result)
		{
			_stream.EndWrite(result);
		}

		/// <summary>
		/// Gets the bytes of a uint32 value in LE format.
		/// </summary>
		/// <param name="uint32"></param>
		/// <returns></returns>
		private byte[] ConvertBytes(uint uint32)
		{
			byte[] bytes = BitConverter.GetBytes(uint32);

			//If we are already in LE, we dont need to flip it
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);

			//Give back the bytes
			return bytes;
		}

		#endregion
		#endregion

		/// <summary>
		/// Closes the pipe (but does not dispose of this object).
		/// </summary>
		public void Close()
		{
			if (_stream != null)
			{
				Logger.Info("Closing stream...");
				_stream.Flush();
				_stream.Close();
			}
		}

		/// <summary>
		/// Disposes the pipe and this object.
		/// </summary>
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
