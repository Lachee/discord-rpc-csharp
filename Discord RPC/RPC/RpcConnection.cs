using DiscordRPC.Helper;
using DiscordRPC.IO;
using DiscordRPC.RPC.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
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

		private long nonce;

		private Thread thread;
		private PipeConnection pipe;

		private object l_sendqueue = new object();
		private Queue<RichPresence> _sendqueue;

		private BackoffDelay delay;
		#endregion


		public RpcConnection(string applicationID, int processID)
		{
			this.applicationID = applicationID;
			this.processID = processID;

			delay = new BackoffDelay(500, 60 * 1000);
			_sendqueue = new Queue<RichPresence>();
		}


		#region Publics

		public void SetPresence(RichPresence presence)
		{
			lock (_sendqueue)
				_sendqueue.Enqueue(presence.Clone());

			//If we are not aborted, we might as well execute it straight up
			if (thread == null) AttemptConnection();
			if (pipe.IsConnected) WriteQueue();
		}
		public void DequeueEvent()
		{
			throw new NotImplementedException();
		}
		public void DequeueEvents()
		{
			throw new NotImplementedException();
		}

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

		/// <summary>
		/// Main thread loop
		/// </summary>
		private void MainLoop()
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
						//We connected to a pipe! Reset the delay
						Console.WriteLine("Connected to the pipe. Attempting to establish handshake...");
						delay.Reset();

						//Attempt to establish a handshake
						if (EstablishHandshake())
						{
							Console.WriteLine("Connection Established. Starting reading loop...");

							//Being the main read loop. This will return true if it exits for an abort.
							if (ReadLoop())
							{
								//The loop has exited because of an abort,
								// so we should exit out of this loop too.
								Console.WriteLine("Handled Thread Abort: Read Loop has returned true, main loop is now exiting.");
								_inMainLoop = false;
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

					Console.WriteLine("Waiting {0}ms", sleep);
					Thread.Sleep((int)delay.NextDelay());
				}
				catch(ThreadAbortException e)
				{
					//We have been given an abort, so exit out of the main loop after reseting the exception
					Thread.ResetAbort();
					Console.WriteLine("Thread Abort: {0}", e.Message);
					_inMainLoop = false;
					break;
				}
				catch (Exception e)
				{
					//We have just had a unkown error. We will repeat the loop again for saftey.
					Console.WriteLine("Something seriously went wrong! {0}", e.Message);
					Console.WriteLine(e.StackTrace);
				}
			}

			//We are no longer in the main loop
			_inMainLoop = false;
			
			//We have disconnected, so dispose of the thread and the pipe.
			if (thread != null) thread = null;
			DisposePipe();
		}

		/// <summary>
		/// Establishes the handshake with the server. 
		/// </summary>
		/// <returns></returns>
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

			try
			{
				do
				{
					Console.WriteLine("Waiting for handshake frame...");

					//Continously read the frames until we get our handshake.
					PipeFrame ackFrame;
					if (!pipe.TryReadFrame(out ackFrame))
					{
						//We failed to read anything, probably broken pipe
						Console.WriteLine("Failed to read anything from the pipe, probably a broken pipe");
						return false;
					}

					//Make sure we got a frame then process it.
					if (ackFrame.Opcode == Opcode.Frame)
						ProcessFrame(ackFrame.GetPayload<ResponsePayload>());

					//Keep going until we are not longer connecting
				} while (State == RpcState.Connecting);
			}
			catch(ThreadAbortException abort)
			{
				//Throw the abort upwards (this will go into our main thread).
				throw abort;
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception occured while saying hello: {0}", e);
				Console.WriteLine(e.StackTrace);
				return false;
			}

			//Success
			return true;
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
							ProcessFrame(response);
							break;

						default:
						case Opcode.Handshake:
							//We have a invalid opcode, better terminate to be safe
							Console.WriteLine("Invalid opcode: {0}", frame.Opcode);
							_inReadLoop = false;
							break;

					}

					//If we aborted in the switch statement above, we will continue to abort.
					if (isAbort) break;

					//Now we just need to write all remaining items
					//Well, we could do this.... but the pipe is asyncronous, so we can read and write at the same time.
					//
					//We are writing now just incase we had any messages while the pipe wasn't available.
					WriteQueue();

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
				WritePresence(null);
				return true;
			}

			//Something else happened, return false
			return false;
		}

		/// <summary>Handles the response from the pipe and calls appropriate events and changes states.</summary>
		/// <param name="response">The response received by the server.</param>
		private void ProcessFrame(ResponsePayload response)
		{
			Console.WriteLine("Handling Response. Cmd: {0}, Event: {1}", response.Command, response.Event);

			//Check if its a handshake
			if (State == RpcState.Connecting)
			{
				if (response.Command == Command.Dispatch && response.Event.HasValue && response.Event.Value == SubscriptionType.Ready)
				{
					Console.WriteLine("Connection established with the RPC");
					lock (l_property) _state = RpcState.Connected;

					//TODO: Send a OnReady event
					Console.WriteLine("Invoke: OnReady");
					Console.WriteLine("Ready");

					//Process the queue we have
					//We will do this later
					WriteQueue();
				}
			}
			else if (State == RpcState.Connected)
			{
				//TODO: Make event queue
				//TODO: Use a ProcessResponse
				Console.WriteLine("Received Frame. Cmd: {0}, Event: {1}", response.Command, response.Event);
			}
			else
			{
				Console.WriteLine("Received a frame while we are disconnected. Ignoring. Cmd: {0}, Event: {1}", response.Command, response.Event);
			}
		}

		#endregion

		#region Writing

		/// <summary>Sends a rich presence through the pipe. Creating the Frame and the Message data.</summary>
		/// <param name="presence">The presence to send. Can be null.</param>
		private void WritePresence(RichPresence presence)
		{
			try
			{
				//Prepare the frame
				PipeFrame frame = new PipeFrame();
				frame.SetPayload(Opcode.Frame, new RequestPayload()
				{
					Command = Command.SetActivity,
					Nonce = (this.nonce++).ToString(),
					Args = new PresenceUpdate()
					{
						PID = this.processID,
						Presence = presence
					}
				});

				//Write the frame
				pipe.WriteFrame(frame);
			}
			catch (Exception e)
			{
				throw e;
			}
		}

		/// <summary>Goes through the send queue one item at a time and sends them through the pipe</summary>
		private void WriteQueue()
		{
			//Prepare some variabels we will clone into with locks
			bool needsWriting = true;
			RichPresence item = null;

			//Continue looping until we dont need anymore messages
			while (true)
			{
				lock (l_sendqueue)
				{
					//Pull the value and update our writing needs
					// If we have nothing to write, exit the loop
					needsWriting = _sendqueue.Count > 0;
					if (!needsWriting) break;	

					//Dequeue the item
					item = _sendqueue.Dequeue();
				}

				try
				{
					//Write the item
					WritePresence(item);
				}
				catch (Exception e)
				{
					//Something has happened, so abort the entire writing sequence. Probably needs a reconnect
					Console.WriteLine("An exception has occured while trying to write.");
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					break;
				}
			}
		}

		#endregion

		#region Connection

		public bool Reconnect()
		{
			//Abort the thread and wait for it to finish aborting
			thread.Abort();
			thread.Join();

			//Now start it again
			return AttemptConnection();
		}

		/// <summary>
		/// Attempts to connect to the pipe. Returns true on success
		/// </summary>
		/// <returns></returns>
		public bool AttemptConnection()
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
			thread = new Thread(MainLoop);
			thread.Start();

			return true;
		}

		/// <summary>
		/// Disposes the pipe if available
		/// </summary>
		private void DisposePipe()
		{
			Console.WriteLine("Disposing Pipe");
			Console.WriteLine(" - Setting State to DC..");
			lock (l_property) _state = RpcState.Disconnected;

			if (pipe != null)
			{
				Console.WriteLine(" - Removing Pipe...");
				pipe.Dispose();
				pipe = null;
			}
			Console.WriteLine(" - Done");
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
