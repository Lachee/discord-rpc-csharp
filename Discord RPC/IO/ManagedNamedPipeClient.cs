using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.Logging;
using System.IO.Pipes;
using System.Threading;
using System.IO;

namespace DiscordRPC.IO
{
	public class ManagedNamedPipeClient : INamedPipeClient
	{
		const string PIPE_NAME = @"discord-ipc-{0}";

		public ILogger Logger { get; set; }
		public bool IsConnected {  get { return _stream != null && _stream.IsConnected; } }
		public int ConnectedPipe {  get { return _connectedPipe; } }

		private int _connectedPipe;
		private NamedPipeClientStream _stream;

		private byte[] _buffer = new byte[PipeFrame.MAX_SIZE];

		private Queue<PipeFrame> _framequeue = new Queue<PipeFrame>();
		private object _framequeuelock = new object();

		public bool Connect(int pipe)
		{
			if (pipe > 9)
				throw new ArgumentOutOfRangeException("pipe", "Argument cannot be greater than 9");

			//Attempt to connect to the specific pipe
			if (pipe >= 0 && AttemptConnection(pipe))
			{
				BeginRead();
				return true;
			}

			//Iterate until we connect to a pipe
			for (int i = 0; i < 10; i++)
			{
				if (AttemptConnection(i))
				{
					BeginRead();
					return true;
				}
			}

			//We failed to connect
			return false;
		}
		private bool AttemptConnection(int pipe)
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
				do { Thread.Sleep(10); } while (!_stream.IsConnected);

				//Store the value
				Logger.Info("Connected to " + pipename);
				_connectedPipe = pipe;
				return true;
			}
			catch (Exception e)
			{
				//Something happened, try again
				//TODO: Log the failure condition
				Logger.Error("Failed connection to {0}. {1}", pipename, e.Message);
				if (_stream != null) _stream.Dispose();
				_stream = null;
				return false;
			}
		}

		private void BeginRead()
		{
			if (!IsConnected) return;

			try
			{
				Logger.Info("Begining Read of {0} bytes", _buffer.Length);
				_stream.BeginRead(_buffer, 0, _buffer.Length, new AsyncCallback(EndRead), IsConnected);
			}
			catch(ObjectDisposedException)
			{
				Logger.Warning("Attempted to start reading from a disposed pipe");
				return;
			}
			catch (InvalidOperationException)
			{
				//The pipe has been closed
				Logger.Warning("Attempted to start reading from a closed pipe");
				return;
			}
		}

		private void EndRead(IAsyncResult callback)
		{
			Logger.Info("Ending Read");
			int bytes = 0;

			try
			{
				//Attempt to read the bytes, catching for IO exceptions or dispose exceptions
				bytes = _stream.EndRead(callback);
			}
			catch (IOException)
			{
				Logger.Warning("Attempted to end reading from a closed pipe");
				return;
			}
			catch(NullReferenceException)
			{
				Logger.Warning("Attempted to read from a null pipe");
				return;
			}
			catch(ObjectDisposedException)
			{
				Logger.Warning("Attemped to end reading from a disposed pipe");
				return;
			}

			//How much did we read?
			Logger.Info("Read {0} bytes", bytes);

			//Did we read anything? If we did we should enqueue it.
			if (bytes > 0)
			{
				//Load it into a memory stream and read the frame
				using (MemoryStream stream = new MemoryStream(_buffer, 0, bytes))
				{
					PipeFrame frame = new PipeFrame();
					if (frame.ReadStream(stream))
					{
						Logger.Info("Read a frame: {0}", frame.Opcode);

						//Enqueue the stream
						lock (_framequeuelock)
							_framequeue.Enqueue(frame);
					}
					else
					{
						//TODO: Enqueue a pipe close event here as we failed to read something.
						Logger.Error("Pipe failed to read from the data received by the stream.");
					}
				}
			}

			//We are still connected, so continue to read
			if (IsConnected)
			{
				Logger.Info("Starting another read");
				BeginRead();
			}
		}

		public bool ReadFrame(out PipeFrame frame)
		{
			//Check the queue, returning the pipe if we have anything available. Otherwise null.
			lock(_framequeuelock)
			{
				if (_framequeue.Count == 0)
				{
					//We found nothing, so just default and return null
					frame = default(PipeFrame);
					return false;
				}

				//Return the dequed frame
				frame = _framequeue.Dequeue();
				return true;
			}
		}

		public bool WriteFrame(PipeFrame frame)
		{
			//Write the frame. We are assuming proper duplex connection here
			if (!IsConnected)
			{
				Logger.Error("Failed to write frame because the stream is closed");
				return false;
			}

			try
			{
				//Write the pipe
				frame.WriteStream(_stream);
				return true;
			}
			catch(IOException io)
			{
				Logger.Error("Failed to write frame because of a IO Exception: {0}", io.Message);
			}
			catch (ObjectDisposedException)
			{
				Logger.Warning("Failed to write frame as the stream was already disposed");
			}
			catch (InvalidOperationException)
			{
				Logger.Warning("Failed to write frame because of a invalid operation");
			}

			//We must have failed the try catch
			return false;
		}
		
		/// <summary>
		/// Closes the pipe
		/// </summary>
		public void Close()
		{
			//flush and dispose			
			try
			{
				if (_stream != null)
				{
					try { _stream.Flush(); } catch (Exception) { }
					_stream.Dispose();
				}
				else
				{
					Logger.Warning("Stream was closed, but no stream was available to begin with!");
				}
			}
			catch (ObjectDisposedException)
			{
				Logger.Warning("Tried to dispose already disposed stream");
			}
			finally
			{
				//set the stream to null
				_connectedPipe = -1;
				_stream = null;
			}

		}

		public void Dispose()
		{
			//Close the stream (disposing of it too)
			Close();
		}		
	}
}
