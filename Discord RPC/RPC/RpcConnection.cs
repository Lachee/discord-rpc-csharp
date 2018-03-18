using DiscordRPC.Helper;
using DiscordRPC.Message;
using DiscordRPC.IO;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Threading;
using Newtonsoft.Json;
using DiscordRPC.Logging;

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

		/// <summary>
		/// Should we only send once we finished receiving one?
		/// </summary>
		public static readonly bool LOCK_STEP = true;

		public ILogger Logger { get; set; }

		#region States
		private object l_states = new object();

		public RpcState State { get { var tmp = RpcState.Disconnected; lock (l_states) tmp = _state; return tmp; } }
		private RpcState _state;
		
		public bool IsRunning { get { return thread != null; } }
		private bool _inMainLoop = false;
		private bool _inReadLoop = false;
		#endregion

		#region Privates

		private string applicationID;					//ID of the Discord APP
		private int processID;							//ID of the process to track

		private long nonce;								//Current command index

		private Thread thread;							//The current thread
		private PipeConnection pipe;                    //The current pipe
		private int targetPipe;							//The pipe to taget. Leave as -1 for any available pipe.

		private object l_rtqueue = new object();		//Lock for the send queue
		private Queue<ICommand> _rtqueue;				//The send queue

		private object l_rxqueue = new object();		//Lock for the receive queue
		private Queue<IMessage> _rxqueue;				//The receive queue

		private BackoffDelay delay;						//The backoff delay before reconnecting.
		#endregion
		
		/// <summary>
		/// Creates a new instance of the RPC.
		/// </summary>
		/// <param name="applicationID">The ID of the Discord App</param>
		/// <param name="processID">The ID of the currently running process</param>
		/// <param name="targetPipe">The target pipe to connect too</param>
		public RpcConnection(string applicationID, int processID, int targetPipe)
		{
			this.applicationID = applicationID;
			this.processID = processID;
			this.targetPipe = targetPipe;

			delay = new BackoffDelay(500, 60 * 1000);
			_rtqueue = new Queue<ICommand>();
			_rxqueue = new Queue<IMessage>();

			//Assign a default logger
			Logger = new ConsoleLogger();
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
					Logger.Info("Sneding Queue Process");
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
			Logger.Info("Initializing Thread. Creating pipe object.");
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
						Logger.Warning("Disposing of potentially broken pipe...");
						DisposePipe();
					}

					//Connect to a new pipe
					Logger.Info("Connecting to a new pipe...");
					pipe = new PipeConnection() { Logger = this.Logger };
					if (pipe.AttemptConnection(targetPipe))
					{
						//We connected to a pipe! Reset the delay
						Logger.Info("Connected to the pipe. Attempting to establish handshake...");
						delay.Reset();

						//Attempt to establish a handshake
						if (EstablishHandshake())
						{
							Logger.Info("Connection Established. Starting reading loop...");

							//Being the main read loop. This will return true if it exits for an abort.
							if (ReadLoop())
							{
								//The loop has exited because of an abort,
								// so we should exit out of this loop too.
								Logger.Info("Handled Thread Abort: Read Loop has returned true, main loop is now exiting.");
								EnqueueMessage(new CloseMessage("Connection terminated by game"));
								_inMainLoop = false;
								break;
							}
						}
						else
						{
							//We failed to establish the handshake
							Logger.Error("Failed to establish a handshake for some reason.");
						}
					}
					else
					{
						Logger.Error("Failed to connect for some reason.");
					}

					//We have disconnected for some reason, either a failed pipe or a bad reading,
					// so we are going to wait a bit before doing it again
					long sleep = delay.NextDelay();

					Logger.Info("Waiting {0}ms", sleep);
					Thread.Sleep(delay.NextDelay());
				}
				catch(ThreadAbortException e)
				{
					//We have been given an abort, so exit out of the main loop after reseting the exception
					Thread.ResetAbort();
					Logger.Info("Thread Abort: {0}", e.Message);

					EnqueueMessage(new CloseMessage("Connection terminated by game"));
					_inMainLoop = false;
					break;
				}
				catch (Exception e)
				{
					//We have just had a unkown error. We will repeat the loop again for saftey.
					Logger.Error("Something seriously went wrong! {0}", e.Message);
					Logger.Error(e.StackTrace);
					EnqueueMessage(new CloseMessage(e.Message));
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
						Logger.Warning("Failed to read a frame. Potentially broken pipe!");
						isAbort = false;
						break;
					}
					
					//Do some basic processing on the frame
					switch (frame.Opcode)
					{
						case Opcode.Close:
							//We have been told by discord to close, so we will consider it an abort
							Logger.Warning("We have been told to terminate by discord. ", frame.Message);
							_inReadLoop = false;
							break;
							
							
						case Opcode.Ping:
							//We have pinged, so we will flip it and respond back with pong
							Logger.Info("PING");
							frame.Opcode = Opcode.Pong;
							pipe.WriteFrame(frame);
							break;

						case Opcode.Pong:
							//We have ponged? I have no idea if Discord actually sends ping/pongs.
							Logger.Info("PONG");
							break;
							
						case Opcode.Frame:

							//We have a frame, so we are going to process the payload and add it to the stack
							EventPayload response = frame.GetObject<EventPayload>();
							ProcessFrame(response);
							break;

						default:
						case Opcode.Handshake:
							//We have a invalid opcode, better terminate to be safe
							Logger.Error("Invalid opcode: {0}", frame.Opcode);
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
					Logger.Info("Thread aborted during read. {0}", e.Message);
					isAbort = true;
					break;
				}
				catch(Exception e)
				{
					//An unkown exception has occured, we will just exit now
					Logger.Error("An exception occured while trying to read pipe. {0}", e.Message);
					Logger.Error(e.StackTrace);
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
					Logger.Warning("Failed to clear the presence. Pipe was probably already removed or was the cause of the failure.");
					
				return true;
			}

			//Something else happened, return false
			return false;
		}

		/// <summary>Handles the response from the pipe and calls appropriate events and changes states.</summary>
		/// <param name="response">The response received by the server.</param>
		private void ProcessFrame(EventPayload response)
		{
			Logger.Info("Handling Response. Cmd: {0}, Event: {1}", response.Command, response.Event);

			//Check if it is an error
			if (response.Event.HasValue && response.Event.Value == ServerEvent.Error)
			{
				//We have an error
				Logger.Error("Error received from the RPC");

				//Create the event objetc and push it to the queue
				ErrorMessage err = response.GetObject<ErrorMessage>();
				Logger.Error("Server responded with an error message: ({0}) {1}", err.Code.ToString(), err.Message);

				//Enqueue the messsage and then end
				EnqueueMessage(err);
				return;
			}

			//Check if its a handshake
			if (State == RpcState.Connecting)
			{
				if (response.Command == Command.Dispatch && response.Event.HasValue && response.Event.Value == ServerEvent.Ready)
				{
					Logger.Info("Connection established with the RPC");
					lock (l_states) _state = RpcState.Connected;

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

					case Command.Unsubscribe:
					case Command.Subscribe:

						//Prepare a serializer that can account for snake_case enums.
						JsonSerializer serializer = new JsonSerializer();
						serializer.Converters.Add(new Converters.EnumSnakeCaseConverter());
						
						//Go through the data, looking for the evt property, casting it to a server event
						var evt = response.Data.GetValue("evt").ToObject<ServerEvent>(serializer);

						//Enqueue the appropriate message.
						if (response.Command == Command.Subscribe)
							EnqueueMessage(new SubscribeMessage(evt));
						else
							EnqueueMessage(new UnsubscribeMessage(evt));

						break;
						
					
					case Command.SendActivityJoinInvite:
						Logger.Info("Got invite response ack.");
						break;

					case Command.CloseActivityJoinRequest:
						Logger.Info("Got invite response reject ack.");
						break;
						
					//we have no idea what we were sent
					default:
						Logger.Error("Unkown frame was received! {0}", response.Command);
						return;
				}
				return;
			}

			Logger.Info("Received a frame while we are disconnected. Ignoring. Cmd: {0}, Event: {1}", response.Command, response.Event);			
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
					Logger.Warning("Ignoring {0}", response.Event.Value);
					break;
			}
		}
		
		#endregion

		#region Writting

		private bool ProcessCommandQueue()
		{
			if (LOCK_STEP)
			{
				Logger.Info("Processing Lockstep Queue");
				return ProcessSingleCommandQueue();
			}
			else
			{
				Logger.Info("Processing Entire Queue");
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
				Logger.Info("------ Sending Payload: " + payload.Command);
				if (pipe.WriteFrame(frame))
				{
					//Remove it from the queue and return true
					lock (l_rtqueue) _rtqueue.Dequeue();
					return true;
				}
				else
				{
					//Something bad happened. Bad pipe?
					Logger.Error("Something went wrong during writing!");
					return false;
				}
			}
			catch (Exception e)
			{
				//Something has happened, so abort the entire writing sequence. Probably needs a reconnect
				Logger.Error("An exception has occured while trying to write.");
				Logger.Error(e.Message);
				Logger.Error(e.StackTrace);
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
					Logger.Info("++++++ Sending payloads: " + payload.Command);
					if (pipe.WriteFrame(frame))
					{
						lock (l_rtqueue) _rtqueue.Dequeue();
						return true;
					}
					else
					{
						Logger.Warning("Something went wrong during writing!");
						return false;
					}
				}
				catch (Exception e)
				{
					//Something has happened, so abort the entire writing sequence. Probably needs a reconnect
					Logger.Error("An exception has occured while trying to write.");
					Logger.Error(e.Message);
					Logger.Error(e.StackTrace);
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
			Logger.Info("Attempting to establish a handshake...");

			//We are establishing a lock and not releasing it until we sent the handshake message.
			// We need to set the key, and it would not be nice if someone did things between us setting the key.
			lock (l_states)
			{
				//Check its state
				if (_state != RpcState.Disconnected)
				{
					Logger.Error("State must be disconnected in order to start a handshake!");
					return false;
				}

				//Send it off to the server
				Logger.Info("Sending Handshake...");
				if (!pipe.WriteHandshake(VERSION, applicationID))
				{
					Logger.Error("Failed to write a handshake.");
					return false;
				}

				_state = RpcState.Connecting;
			}

			try
			{
				do
				{
					Logger.Info("Waiting for handshake frame...");

					//Continously read the frames until we get our handshake.
					PipeFrame ackFrame;
					if (!pipe.TryReadFrame(out ackFrame))
					{
						//We failed to read anything, probably broken pipe
						Logger.Warning("Failed to read anything from the pipe, probably a broken pipe");
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
				Logger.Error("Exception occured while saying hello: {0}", e);
				Logger.Error(e.StackTrace);
				return false;
			}

			//Success
			return true;
		}


		public bool Reconnect()
		{
			//Abort the thread and wait for it to finish aborting
			Logger.Info("Attempting to reconnect. Waiting for previous connection to abort...");
			if (thread != null)
			{
				thread.Abort();
				thread.Join();
			}
			
			//Now start it again
			return AttemptConnection();
		}

		/// <summary>
		/// Attempts to connect to the pipe. Returns true on success
		/// </summary>
		/// <returns></returns>
		public bool AttemptConnection()
		{
			Logger.Info("Attempting a new coonnection");

			//The thread mustn't exist already
			if (thread != null)
			{
				Logger.Error("Cannot attempt a new connection as the previous connection thread is not null!");
				return false;
			}

			//We have to be in the disconnected state
			if (State != RpcState.Disconnected)
			{
				Logger.Warning("Cannot attempt a new connection as the previous connection hasn't changed state yet.");
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
			Logger.Info("Disposing Pipe");
			Logger.Info(" - Setting State to DC..");
			lock (l_states) _state = RpcState.Disconnected;

			if (pipe != null)
			{
				Logger.Info(" - Removing Pipe...");
				pipe.Dispose();
				pipe = null;
			}
			Logger.Info(" - Done");
		}

		/// <summary>
		/// Closes the connection
		/// </summary>
		public void Close()
		{
			if (!IsRunning || thread == null)
			{
				Logger.Error("Cannot close as it is not available!");
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
