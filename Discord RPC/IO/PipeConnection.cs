using System;
using System.Collections.Generic;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace DiscordRPC.IO
{
	internal class PipeConnection : IDisposable
	{
		/// <summary>
		/// Discord Pipe Name
		/// </summary>
		const string PIPE_NAME = @"discord-ipc-{0}";

		private NamedPipeClientStream u_stream;
		private Queue<PipeFrame> u_readQueue = new Queue<PipeFrame>();
		private object l_readQueueLock = new object();

		public bool IsConnected {  get { bool tmp; lock (l_isconnectedLock) tmp = u_isconnected; return tmp; } }
		private bool u_isconnected = false;
		private object l_isconnectedLock = new object();

		private int u_pipeno;
		private Thread _thread;


		#region Pipe Management

		public void Connect()
		{
			//We are already connected, cannot connect again!
			if (IsConnected) return;

			//Create and run a new thread.
			_thread = new Thread(Run);
			_thread.Start();
		}
		
		private void Run()
		{
			if (U_OpenConnection())
			{
				//TODO: Error Loging
				//We failed to connect
				Terminate();
				return;
			}

			//Set isconnected to true!
			SetConnectionState(true);

			//Run until aborted (or something else bad has happened).
			bool isRunning = true;
			while (isRunning)
			{
				try
				{
					//Try to read the next frame. This is a blocking call. 
					PipeFrame frame = ReadFrame();
					EnqueueFrame(frame);
				}
				catch (ThreadAbortException) { Thread.ResetAbort(); }		//Thread abort was called
				catch (Exception) { }										//Something else bad happened!
				finally
				{
					//Stop running the loop
					isRunning = false;
				}
			}

			//We have disconnected
			Terminate();
		}

		private void Terminate()
		{
			//We are no longer connected.
			SetConnectionState(false);

			//TODO: Throw closing event

			//Destroy the connection
			if (u_stream != null)
			{
				u_stream.Dispose();
				u_stream = null;
			}

			//Clear the thread for properity
			_thread = null;
		}


		private void SetConnectionState(bool state) { lock (l_isconnectedLock) u_isconnected = state; }
		private bool U_OpenConnection()
		{
			for (int i = 0; i < 10; i++)
			{
				try
				{
					//Prepare the pipe name
					string pipename = string.Format(PIPE_NAME, i);

					//Create the client
					u_stream = new NamedPipeClientStream(".", pipename, PipeDirection.InOut);
					u_stream.Connect(1000);

					//Spin for a bit while we wait for it to finish connecting
					while (!u_stream.IsConnected) { Thread.Sleep(100); }

					//Store the value
					return true;
				}
				catch (Exception)
				{
					//Something happened, try again
					//TODO: Log the failure condition
					u_stream = null;
				}
			}

			return u_stream == null;
		}

		#endregion

		#region Frame Read Pool
		/// <summary>
		/// Is the read queue available?
		/// </summary>
		/// <returns></returns>
		public bool HasFramesAvailable()
		{
			bool available = false;
			lock (l_readQueueLock)
				available = u_readQueue.Count > 0;

			return available;
		}

		/// <summary>
		//	Fetches all the frames recently read.
		/// </summary>
		/// <returns></returns>
		public PipeFrame[] ReadFrames()
		{
			PipeFrame[] queue = null;
			lock (l_readQueueLock)
			{
				//Copy the queue over
				queue = new PipeFrame[u_readQueue.Count];
				u_readQueue.CopyTo(queue, 0);

				//Empty the queue out
				u_readQueue.Clear();
			}

			//Return the current queue
			return queue;
		}

		/// <summary>
		/// Adds a frame to the current queue
		/// </summary>
		/// <param name="frame"></param>
		private void EnqueueFrame(PipeFrame frame)
		{
			//TODO: EVents
			lock (l_readQueueLock)
				u_readQueue.Enqueue(frame);
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
		private PipeFrame ReadFrame()
		{
			int op = ReadInt32();
			int len = ReadInt32();

			byte[] buff = new byte[len];
			Read(buff, len);

			return new PipeFrame()
			{
				Opcode = op,
				Data = buff
			};
		}

		private int Read(byte[] buff, int length) { return u_stream.Read(buff, 0, length); }
		private int ReadInt32()
		{
			//Read the bytes
			byte[] bytes = new byte[4];
			if (Read(bytes, 4) != 4) return -1;
			
			//Convert to int
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			return BitConverter.ToInt32(bytes, 0);
		}
		#endregion

		#region Write
		private void Write(PipeFrame frame)
		{
			Write(frame.Opcode);
			Write(frame.Length);
			Write(frame.Data);
		}
		private void Write(byte[] buff)
		{
			u_stream.Write(buff, 0, buff.Length);
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
			_thread.Abort();
		}
	}
}
