using DiscordRPC.Helper;
using DiscordRPC.IO;
using DiscordRPC.RPC.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DiscordRPC.RPC
{
	/// <summary>
	/// Communicates between the client and discord through RPC
	/// </summary>
	public class RpcConnection
	{
		/// <summary>
		/// Version of the RPC Protocol
		/// </summary>
		public static readonly int VERSION = 1;

		#region States

		public RpcState State { get { var tmp = RpcState.Disconnected; lock (l_property) tmp = _state; return tmp; } }
		private RpcState _state;
		
		public bool IsRunning { get { return thread != null; } }
		private bool _inMainLoop = false;
		private bool _inReadLoop = false;
		#endregion

		#region Locks
		private object l_property = new object();
		#endregion

		#region Privates
		private string applicationID;
		private int processID;

		private Thread thread;
		private PipeConnection pipe;

		private BackoffDelay delay;
		#endregion

		public RpcConnection(string applicationID, int processID)
		{
			this.applicationID = applicationID;
			this.processID = processID;

			delay = new BackoffDelay(500, 60 * 1000);
		}


		#region Publics

		public void SetPresence(RichPresence presence) { }
		public void DequeueEvent() { }
		public void DequeueEvents() { }
		public void Close()
		{
			if (!IsRunning || thread == null)
			{
				Console.WriteLine("Cannot close as it is not available!");
				return;
			}

			//Abort the thread and wait for it to finish aborting
			thread.Abort();
			thread.Join();
		}
		
		#endregion
		
		#region Read Loop

		public void ThreadStart()
		{

			//initialize the pipe
			Console.WriteLine("Initializing Thread. Creating pipe object.");
			_inMainLoop = true;

			//We need to always be connected, reconnect if we fail
			while (_inMainLoop)
			{
				//Wrap everything up in a try get
				try
				{
					//Dispose of the pipe if we have any (could be broken)
					if (pipe != null)
					{
						Console.WriteLine("Disposing of potentially broken pipe...");
						DisposePipe();
					}

					//Connect to a new pipe
					Console.WriteLine("Connecting to a new pipe...");
					pipe = new PipeConnection();
					if (pipe.AttemptConnection())
					{
						Console.WriteLine("Connected to the pipe!");

						//We connected to a pipe! Reset the delay
						delay.Reset();

						//Attempt to establish a handshake
						if (EstablishHandshake())
						{
							//Being the main read loop. This will return true if it exits for an abort.
							if (ReadLoop())
							{
								//The loop has exited because of an abort,
								// so we should exit out of this loop too.
								Console.WriteLine("Handled Thread Abort: Read Loop has returned true, main loop is now exiting.");
								break;
							}
						}
						else
						{
							//We failed to establish the handshake
							Console.WriteLine("Failed to establish a handshake for some reason.");
						}
					}
					else
					{
						Console.WriteLine("Failed to connect for some reason.");
					}

					//We have disconnected for some reason, either a failed pipe or a bad reading,
					// so we are going to wait a bit before doing it again
					long sleep = delay.NextDelay();

					Console.Write("Waiting {0}ms", sleep);
					Thread.Sleep((int)delay.NextDelay());
				}
				catch(ThreadAbortException e)
				{
					//We have been given an abort, so exit out of the main loop after reseting the exception
					Thread.ResetAbort();
					Console.WriteLine("Thread Abort: {0}", e.Message);
					break;
				}
				catch (Exception e)
				{
					Console.WriteLine("Something seriously went wrong! {0}", e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}

			//We are no longer in the main loop
			_inMainLoop = false;

			//Lock all the properties up while we decide whats happening
			lock (l_property)
			{
				//Set the state
				_state = RpcState.Disconnected;
				_inMainLoop = false;

				//We have disconnected
				if (thread != null) thread = null;
				if (pipe != null)
				{
					pipe.Dispose();
					pipe = null;
				}
			}
		}

		private bool EstablishHandshake()
		{
			Console.WriteLine("Attempting to establish a handshake...");

			//We are establishing a lock and not releasing it until we sent the handshake message.
			// We need to set the key, and it would not be nice if someone did things between us setting the key.
			lock (l_property)
			{
				//Check its state
				if (_state != RpcState.Disconnected)
				{
					Console.WriteLine("State must be disconnected in order to start a handshake!");
					return false;
				}

				
				Console.WriteLine("Sending Handshake...");

				//Prepare the handshake and send it
				PipeFrame handshakeFrame = new PipeFrame();
				handshakeFrame.SetPayload(Opcode.Handshake, new Handshake() { ClientID = applicationID, Version = VERSION });

				//Send it off to the server
				pipe.WriteFrame(handshakeFrame);
				_state = RpcState.Connecting;
			}

			//Wait for a response now
			PipeFrame ackFrame;
			if (!pipe.TryReadFrame(out ackFrame))
			{
				//We failed to read anything, probably broken pipe
				Console.WriteLine("Failed to read anything from the pipe, probably a broken pipe");
				return false;
			}



			return false;
		}

		/// <summary>
		/// The main pipe reading loop. This should not exit until an exception occurs. If the exception is a ThreadAbort, this will return a true,  otherwise false. 
		/// </summary>
		/// <returns>True if the thread was aborted</returns>
		private bool ReadLoop()
		{
			//This stores if we are exiting because of an abort signal
			bool isAbort = false;

			//Enter the main loop
			_inReadLoop = true;
			while (_inReadLoop)
			{
				try
				{
					//Read the frame
					PipeFrame frame;
					if (!pipe.TryReadFrame(out frame))
					{
						//It failed to read, so abort
						Console.WriteLine("Failed to read a frame. Potentially broken pipe!");
						isAbort = false;
						break;
					}
					
					//Do some basic processing on the frame
					switch (frame.Opcode)
					{
						case Opcode.Close:
							//We have been told by discord to close, so we will consider it an abort
							Console.WriteLine("We have been told to terminate by discord.");
							isAbort = true;
							_inReadLoop = false;
							break;
							
						default:
						case Opcode.Handshake:
							//We have a invalid opcode, better terminate to be safe
							Console.WriteLine("Invalid opcode: {0}", frame.Opcode);
							_inReadLoop = false;
							break;
							
						case Opcode.Ping:
							//We have pinged, so we will flip it and respond back with pong
							Console.WriteLine("PING");
							frame.Opcode = Opcode.Pong;
							pipe.WriteFrame(frame);
							break;

						case Opcode.Pong:
							//We have ponged? I have no idea if Discord actually sends ping/pongs.
							Console.WriteLine("PONG");
							break;
							
						case Opcode.Frame:

							//We have a frame, so we are going to process the payload and add it to the stack
							ResponsePayload response = frame.GetPayload<ResponsePayload>();
							//TODO: Make this do things.
							// More speficially, finish the handshake protocol

							//ProcessResponse(response);
							break;
					}

					//If we aborted in the switch statement above, we will continue to abort.
					if (isAbort) break;

					//Add the frame to our message queue
					//If we are still connecting, attempt to decypher it

				}
				catch (ThreadAbortException e)
				{
					//An abort signal has been sent, so we will reset it
					Thread.ResetAbort();

					//We will exit this loop, with the abort flag set
					Console.WriteLine("Thread aborted during read. {0}", e.Message);
					isAbort = true;
					break;
				}
				catch(Exception e)
				{
					//An unkown exception has occured, we will just exit now
					Console.WriteLine("An exception occured while trying to read pipe. {0}", e.Message);
					Console.WriteLine(e.StackTrace);
					isAbort = false;
					break;
				}
			}
			_inReadLoop = false;

			//its an abort, so we will clear our presence if we are able
			if(isAbort && State == RpcState.Connected)
			{
				//TODO: Send the null presence
				return true;
			}

			//Something else happened, return false
			return false;
		}

		#endregion

		#region Connection

		private bool AttemptConnection()
		{
			//The thread mustn't exist already
			if (thread != null)
			{
				Console.WriteLine("Thread is not null!");
				return false;
			}

			//We have to be in the disconnected state
			if (State != RpcState.Disconnected)
			{
				Console.Write("Thread is still connecting?");
				return false;
			}

			//Start the thread up
			thread = new Thread(ThreadStart);
			thread.Start();

			return true;
		}

		private void DisposePipe()
		{
			if (pipe != null)
			{
				Console.WriteLine("Disposing of pipe");
				pipe.Dispose();
				pipe = null;
			}
		}

		#endregion



	}

	public enum RpcState
	{
		Disconnected,
		Connecting,
		Connected
	}
}
