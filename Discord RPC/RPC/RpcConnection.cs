using DiscordRPC.Helper;
using DiscordRPC.Message;
using DiscordRPC.IO;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;

namespace DiscordRPC.RPC
{




	/*





		TODO:

		Make sure I dont send any messages until I get a response back









	*/




	/// <summary>
	/// Communicates between the client and discord through RPC
	/// </summary>
	public class RpcConnection
	{
		/// <summary>
		/// Version of the RPC Protocol
		/// </summary>
		public static readonly int VERSION = 1;

		/// <summary>
		/// Should we only send once we finished receiving one?
		/// </summary>
		public static readonly bool LOCK_STEP = true;

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

		private object l_rtqueue = new object();
		private Queue<ICommand> _rtqueue;

		private object l_rxqueue = new object();
		private Queue<IMessage> _rxqueue;

		private BackoffDelay delay;
		#endregion
		
		public RpcConnection(string applicationID, int processID)
		{
			this.applicationID = applicationID;
			this.processID = processID;

			delay = new BackoffDelay(500, 60 * 1000);
			_rtqueue = new Queue<ICommand>();
			_rxqueue = new Queue<IMessage>();
		}
			
		#region Queues
		/// <summary>
		/// Enqueues a command
		/// </summary>
		/// <param name="presence"></param>
		internal void EnqueueCommand(ICommand command)
		{
			int count = 0;
			lock (_rtqueue)
			{
				//Enqueue the set presence argument
				_rtqueue.Enqueue(command);
				count = _rtqueue.Count;
			}

			//If we are not aborted, we might as well execute it straight up
			if (thread == null) { AttemptConnection(); return; }
			if (pipe != null && pipe.IsConnected)
			{
				if (!LOCK_STEP || (LOCK_STEP && count == 1))
				{
					Console.WriteLine("Sneding Queue Process");
					ProcessCommandQueue();
				}
			}
		}

		/// <summary>
		/// Adds a message to the message queue. Does not copy the message, so besure to copy it yourself or dereference it.
		/// </summary>
		/// <param name="message">The message to add</param>
		private void EnqueueMessage(IMessage message)
		{
			lock (l_rxqueue)
				_rxqueue.Enqueue(message);
		}

		/// <summary>
		/// Dequeues a single message from the event stack. Returns null if none are available.
		/// </summary>
		/// <returns></returns>
		internal IMessage DequeueMessage()
		{
			lock (l_rxqueue)
			{
				//We have nothing, so just return null.
				if (_rxqueue.Count == 0) return null;

				//Get the value and remove it from the list at the same time
				return _rxqueue.Dequeue();
			}
		}

		/// <summary>
		/// Dequeues all messages from the event stack. 
		/// </summary>
		/// <returns></returns>
		internal IMessage[] DequeueMessages()
		{
			lock (l_rxqueue)
			{
				//Copy the messages into an array
				IMessage[] messages = _rxqueue.ToArray();

				//Clear the entire queue
				_rxqueue.Clear();

				//return the array
				return messages;
			}
		}
		#endregion
				
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
								EnqueueMessage(new CloseMessage("Connection terminated by game"));
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
					Thread.Sleep(delay.NextDelay());
				}
				catch(ThreadAbortException e)
				{
					//We have been given an abort, so exit out of the main loop after reseting the exception
					Thread.ResetAbort();
					Console.WriteLine("Thread Abort: {0}", e.Message);
					EnqueueMessage(new CloseMessage("Connection terminated by game"));
					_inMainLoop = false;
					break;
				}
				catch (Exception e)
				{
					//We have just had a unkown error. We will repeat the loop again for saftey.
					Console.WriteLine("Something seriously went wrong! {0}", e.Message);
					EnqueueMessage(new CloseMessage(e.Message));
					Console.WriteLine(e.StackTrace);
				}

			}

			//We are no longer in the main loop
			_inMainLoop = false;
			
			//We have disconnected, so dispose of the thread and the pipe.
			if (thread != null) thread = null;
			DisposePipe();
		}

		#region Reading

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
							Console.WriteLine(frame.Message);
							//isAbort = true;
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
							EventPayload response = frame.GetObject<EventPayload>();
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


				//Now we just need to write all remaining items
				//Well, we could do this.... but the pipe is asyncronous, so we can read and write at the same time.
				//
				//We are writing now just incase we had any messages while the pipe wasn't available.
				ProcessCommandQueue();
			}
			_inReadLoop = false;

			//its an abort, so we will clear our presence if we are able
			if(isAbort && State == RpcState.Connected)
			{
				//Prepare the frame
				PipeFrame frame = new PipeFrame();
				frame.SetObject(Opcode.Frame, (new PresenceCommand() { PID = processID, Presence = null }).PreparePayload(nonce++));

				//Write it
				if (pipe != null && !pipe.WriteFrame(frame))
					Console.WriteLine("Failed to clear the presence. Pipe was probably already removed or was the cause of the failure.");
					
				return true;
			}

			//Something else happened, return false
			return false;
		}

		/// <summary>Handles the response from the pipe and calls appropriate events and changes states.</summary>
		/// <param name="response">The response received by the server.</param>
		private void ProcessFrame(EventPayload response)
		{
			Console.WriteLine("Handling Response. Cmd: {0}, Event: {1}", response.Command, response.Event);

			//Check if it is an error
			if (response.Event.HasValue && response.Event.Value == ServerEvent.Error)
			{
				//We have an error
				Console.WriteLine("Error received from the RPC");

				//Create the event objetc and push it to the queue
				ErrorMessage err = response.GetObject<ErrorMessage>();
				Console.WriteLine("Server responded with an error message: ({0}) {1}", err.Code.ToString(), err.Message);

				//Enqueue the messsage and then end
				EnqueueMessage(err);
				return;
			}

			//Check if its a handshake
			if (State == RpcState.Connecting)
			{
				if (response.Command == Command.Dispatch && response.Event.HasValue && response.Event.Value == ServerEvent.Ready)
				{
					Console.WriteLine("Connection established with the RPC");
					lock (l_property) _state = RpcState.Connected;

					//Enqueue a ready event
					ReadyMessage ready = response.GetObject<ReadyMessage>();
					EnqueueMessage(ready);

					//Process the queue we have
					//We will do this later
					ProcessCommandQueue();

					return;
				}
			}

			if (State == RpcState.Connected)
			{
				switch(response.Command)
				{
					//We were sent a dispatch, better process it
					case Command.Dispatch:
						ProcessDispatch(response);
						break;

					//We were sent a Activity Update, better enqueue it
					case Command.SetActivity:
						RichPresenceResponse rp = response.GetObject<RichPresenceResponse>();
						EnqueueMessage(new PresenceMessage(rp));
						break;

					case Command.Subscribe:
						JsonSerializer serializer = new JsonSerializer();
						serializer.Converters.Add(new Converters.EnumSnakeCaseConverter());

						var evt = response.Data.GetValue("evt").ToObject<ServerEvent>(serializer);
						EnqueueMessage(new SubscribeMessage(evt));
						break;

					case Command.SendActivityJoinInvite:
						Console.WriteLine("Got invite response ack.");
						break;

					case Command.CloseActivityJoinRequest:
						Console.WriteLine("Got invite response reject ack.");
						break;
						
					//we have no idea what we were sent
					default:
						Console.WriteLine("Unkown frame was received! {0}", response.Command);
						return;
				}
				return;
			}

			Console.WriteLine("Received a frame while we are disconnected. Ignoring. Cmd: {0}, Event: {1}", response.Command, response.Event);			
		}

		private void ProcessDispatch(EventPayload response)
		{
			if (response.Command != Command.Dispatch) return;
			if (!response.Event.HasValue) return;

			switch(response.Event.Value)
			{
				//We are to join the server
				case ServerEvent.ActivitySpectate:
					var spectate = response.Data.ToObject<SpectateMessage>();
					EnqueueMessage(spectate);
					break;

				case ServerEvent.ActivityJoin:
					var join = response.Data.ToObject<JoinMessage>();
					EnqueueMessage(join);
					break;

				case ServerEvent.ActivityJoinRequest:
					var request = response.Data.ToObject<JoinRequestMessage>();
					EnqueueMessage(request);
					break;

				//Unkown dispatch event received. We should just ignore it.
				default:
					Console.WriteLine("Ignoring {0}", response.Event.Value);
					break;
			}
		}
		
		#endregion

		#region Writting

		private bool ProcessCommandQueue()
		{
			if (LOCK_STEP)
			{
				Console.WriteLine("Processing Lockstep Queue");
				return ProcessSingleCommandQueue();
			}
			else
			{
				Console.WriteLine("Processing Entire Queue");
				return ProcessEntireCommandQueue();
			}
		}
		private bool ProcessSingleCommandQueue()
		{
			//Get the item if its available
			ICommand item = null;
			lock (l_rtqueue)
			{
				if (_rtqueue.Count > 0)
					item = _rtqueue.Peek();
			}

			//We have no items available
			if (item == null) return true;

			try
			{
				//Prepare the payload
				IPayload payload = item.PreparePayload(nonce++);
				
				//Prepare the frame
				PipeFrame frame = new PipeFrame();
				frame.SetObject(Opcode.Frame, payload);

				//Write it and if it wrote perfectly fine, we will dequeue it
				Console.WriteLine("------ Sending Payload: " + payload.Command);
				if (pipe.WriteFrame(frame))
				{
					//Remove it from the queue and return true
					lock (l_rtqueue) _rtqueue.Dequeue();
					return true;
				}
				else
				{
					//Something bad happened. Bad pipe?
					Console.WriteLine("Something went wrong during writing!");
					return false;
				}
			}
			catch (Exception e)
			{
				//Something has happened, so abort the entire writing sequence. Probably needs a reconnect
				Console.WriteLine("An exception has occured while trying to write.");
				Console.WriteLine(e.Message);
				Console.WriteLine(e.StackTrace);
				return false;
			}
		}
		private bool ProcessEntireCommandQueue()
		{
			//Prepare some variabels we will clone into with locks
			bool needsWriting = true;
			ICommand item = null;

			//Continue looping until we dont need anymore messages
			while (true)
			{
				lock (l_rtqueue)
				{
					//Pull the value and update our writing needs
					// If we have nothing to write, exit the loop
					needsWriting = _rtqueue.Count > 0;
					if (!needsWriting) return true;	

					//Peek at the item
					item = _rtqueue.Peek();
				}

				try
				{
					//Prepare the payload
					IPayload payload = item.PreparePayload(nonce++);

					//Prepare the frame
					PipeFrame frame = new PipeFrame();
					frame.SetObject(Opcode.Frame, payload);

					//Write it and if it wrote perfectly fine, we will dequeue it
					Console.WriteLine("++++++ Sending payloads: " + payload.Command);
					if (pipe.WriteFrame(frame))
					{
						lock (l_rtqueue) _rtqueue.Dequeue();
						return true;
					}
					else
					{
						Console.WriteLine("Something went wrong during writing!");
						return false;
					}
				}
				catch (Exception e)
				{
					//Something has happened, so abort the entire writing sequence. Probably needs a reconnect
					Console.WriteLine("An exception has occured while trying to write.");
					Console.WriteLine(e.Message);
					Console.WriteLine(e.StackTrace);
					return false;
				}
			}
		}

		#endregion

		#region Connection

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

				//Send it off to the server
				Console.WriteLine("Sending Handshake...");
				if (!pipe.WriteHandshake(VERSION, applicationID))
					return false;
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
						ProcessFrame(ackFrame.GetObject<EventPayload>());

					//Keep going until we are not longer connecting
				} while (State == RpcState.Connecting);
			}
			catch (ThreadAbortException abort)
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

		/// <summary>
		/// Closes the connection
		/// </summary>
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
		
	}

	public enum RpcState
	{
		Disconnected,
		Connecting,
		Connected
	}
}
