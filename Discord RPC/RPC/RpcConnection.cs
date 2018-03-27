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
	public class RpcConnection : IDisposable
	{
		/// <summary>
		/// Version of the RPC Protocol
		/// </summary>
		public static readonly int VERSION = 1;

		/// <summary>
		/// The rate of poll to the discord pipe.
		/// </summary>
		public static readonly int POLL_RATE = 1000;

		/// <summary>
		/// Should we only send once we finished receiving one?
		/// </summary>
		public static readonly bool LOCK_STEP = false;

		public ILogger Logger
		{
			get { return _logger; }
			set
			{
				_logger = value;
				if (namedPipe != null)
					namedPipe.Logger = value;
			}
		}
		private ILogger _logger;

		#region States
		private object l_states = new object();

		public RpcState State { get { var tmp = RpcState.Disconnected; lock (l_states) tmp = _state; return tmp; } }
		private RpcState _state;

		private volatile bool aborting = false;
		private bool disposed = false;

		public bool IsRunning { get { return thread != null; } }
		#endregion

		#region Privates

		private string applicationID;					//ID of the Discord APP
		private int processID;							//ID of the process to track

		private long nonce;								//Current command index

		private Thread thread;							//The current thread
		private INamedPipeClient namedPipe;

		private int targetPipe;							//The pipe to taget. Leave as -1 for any available pipe.

		private object l_rtqueue = new object();		//Lock for the send queue
		private Queue<ICommand> _rtqueue;				//The send queue

		private object l_rxqueue = new object();		//Lock for the receive queue
		private Queue<IMessage> _rxqueue;               //The receive queue

		private AutoResetEvent queueUpdatedEvent = new AutoResetEvent(false);
		private BackoffDelay delay;                     //The backoff delay before reconnecting.
		#endregion

		/// <summary>
		/// Creates a new instance of the RPC.
		/// </summary>
		/// <param name="applicationID">The ID of the Discord App</param>
		/// <param name="processID">The ID of the currently running process</param>
		/// <param name="targetPipe">The target pipe to connect too</param>
		public RpcConnection(string applicationID, int processID, int targetPipe, INamedPipeClient client)
		{
			this.applicationID = applicationID;
			this.processID = processID;
			this.targetPipe = targetPipe;
			this.namedPipe = client;

			//Assign a default logger
			Logger = new ConsoleLogger();

			delay = new BackoffDelay(500, 60 * 1000);
			_rtqueue = new Queue<ICommand>();
			_rxqueue = new Queue<IMessage>();
			
			Random rand = new Random();
			nonce = 0;// rand.Next(0, 10000000);
		}
		
			
		private long GetNextNonce()
		{
			nonce += 1;
			return nonce;
		}

		#region Queues
		/// <summary>
		/// Enqueues a command
		/// </summary>
		/// <param name="presence"></param>
		internal void EnqueueCommand(ICommand command)
		{
			//Enqueue the set presence argument
			lock (_rtqueue)
				_rtqueue.Enqueue(command);

			//Tell the thread something happened
			//if (State == RpcState.Connected) queueUpdatedEvent.Set();
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

			//Forever trying to connect unless the abort signal is sent
			//Keep Alive Loop
			while (!aborting)
			{
				try
				{
					//Wrap everything up in a try get
					//Dispose of the pipe if we have any (could be broken)
					if (namedPipe == null)
					{
						Logger.Error("Something bad has happened with our pipe client!");
						aborting = true;
						return;
					}

					//Connect to a new pipe
					Logger.Info("Connecting to the pipe through the {0}", namedPipe.GetType().FullName);
					if (namedPipe.Connect(targetPipe))
					{
						//We connected to a pipe! Reset the delay
						Logger.Info("Connected to the pipe. Attempting to establish handshake...");
						EnqueueMessage(new ConnectionEstablishedMessage() { ConnectedPipe = namedPipe.ConnectedPipe });
						delay.Reset();

						//Attempt to establish a handshake
						EstablishHandshake();
						Logger.Info("Connection Established. Starting reading loop...");

						//Continously iterate, waiting for the frame
						PipeFrame frame;
						bool mainloop = true;
						while (mainloop && !aborting)
						{
							//Iterate over every frame we have queued up, processing its contents
							Logger.Info("Trying to read frames...");
							if (namedPipe.IsConnected && namedPipe.ReadFrame(out frame))
							{
								Logger.Info("Read Payload: {0}", frame.Opcode);

								//Do some basic processing on the frame
								switch (frame.Opcode)
								{
									case Opcode.Close:
										//We have been told by discord to close, so we will consider it an abort
										Logger.Warning("We have been told to terminate by discord. ", frame.Message);
										mainloop = false;
										break;


									case Opcode.Ping:
										//We have pinged, so we will flip it and respond back with pong
										Logger.Info("PING");
										frame.Opcode = Opcode.Pong;
										namedPipe.WriteFrame(frame);
										break;

									case Opcode.Pong:
										//We have ponged? I have no idea if Discord actually sends ping/pongs.
										Logger.Info("PONG");
										break;

									case Opcode.Frame:

										//We have a frame, so we are going to process the payload and add it to the stack
										if (frame.Data == null)
										{
											Logger.Error("We received no data from the frame so we cannot get the event payload!");
											break;
										}
										else
										{
											EventPayload response = frame.GetObject<EventPayload>();
											ProcessFrame(response);
											break;
										}

									default:
									case Opcode.Handshake:
										//We have a invalid opcode, better terminate to be safe
										Logger.Error("Invalid opcode: {0}", frame.Opcode);
										mainloop = false;
										break;
								}
							}

							//Process the entire command queue we have left
							ProcessCommandQueue();

							//We should wait some time, unless we have been aborted.
							if (!aborting)
							{
								//Wait for some time, or until a command has been queued up
								Logger.Info("Waiting for {0}ms or until some event occurs...", POLL_RATE);
								queueUpdatedEvent.WaitOne(POLL_RATE);

							}
						}

						Logger.Warning("Left main read loop for some reason. IsAbort: {0}", aborting);
					}
					else
					{
						Logger.Error("Failed to connect for some reason.");
						EnqueueMessage(new ConnectionFailedMessage());
					}

					//If we are not aborting, we have to wait a bit before trying to connect again
					if (!aborting)
					{
						//We have disconnected for some reason, either a failed pipe or a bad reading,
						// so we are going to wait a bit before doing it again
						long sleep = delay.NextDelay();

						Logger.Info("Waiting {0}ms", sleep);
						Thread.Sleep(delay.NextDelay());
					}
				}
				catch(InvalidPipeException e)
				{
					Logger.Error("Invalid Pipe Exception: {0}", e.Message);
				}
				catch (Exception e)
				{
					Logger.Error("Unhandled Exception: {0}", e.GetType().FullName);
					Logger.Error(e.Message);
					Logger.Error(e.StackTrace);
				}
				finally
				{
					//Disconnect from the pipe because something bad has happened. An exception has been thrown or the main read loop has terminated.
					if (namedPipe.IsConnected) namedPipe.Close();
				}
			}

			//We have disconnected, so dispose of the thread and the pipe.
			Logger.Info("Left Main Loop");
			namedPipe.Dispose();

			Logger.Info("Thread Terminated");
		}

		#region Reading

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
						if (response.Data == null)
						{
							EnqueueMessage(new PresenceMessage());
						}
						else
						{
							RichPresenceResponse rp = response.GetObject<RichPresenceResponse>();
							EnqueueMessage(new PresenceMessage(rp));
						}
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
		
		private void ProcessCommandQueue()
		{
			Logger.Info("Checking Write Queue");
			if (aborting)
			{
				Logger.Warning("We have been told to write a queue but we have also been aborted.");
				return;
			}

			//Prepare some variabels we will clone into with locks
			bool needsWriting = true;
			ICommand item = null;
			
			//Continue looping until we dont need anymore messages
			while (needsWriting)
			{
				lock (l_rtqueue)
				{
					//Pull the value and update our writing needs
					// If we have nothing to write, exit the loop
					needsWriting = _rtqueue.Count > 0;
					if (!needsWriting) break;	

					//Peek at the item
					item = _rtqueue.Peek();
				}
				
				//Prepare the payload
				IPayload payload = item.PreparePayload(GetNextNonce());

				//Prepare the frame
				PipeFrame frame = new PipeFrame();
				frame.SetObject(item is CloseCommand ? Opcode.Handshake : Opcode.Frame, payload);

				//Write it and if it wrote perfectly fine, we will dequeue it
				Logger.Info("++++++ Sending payloads: " + payload.Command);
				if (namedPipe.WriteFrame(frame))
				{
					lock (l_rtqueue) _rtqueue.Dequeue();
				}
				else
				{
					Logger.Warning("Something went wrong during writing!");
					Thread.Sleep(100);
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
				if (!namedPipe.WriteFrame(new PipeFrame(Opcode.Handshake, new Handshake() { Version = VERSION, ClientID = applicationID })))
				{
					Logger.Error("Failed to write a handshake.");
					return false;
				}

				_state = RpcState.Connecting;
			}
			
			//Success
			return true;
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
			thread.Name = "Discord IPC Thread";
			thread.IsBackground = true;
			thread.Start();

			return true;
		}
		
		/// <summary>
		/// Closes the connection
		/// </summary>
		public void Close()
		{
			if (thread == null)
			{
				Logger.Error("Cannot close as it is not available!");
				return;
			}

			if (aborting)
			{
				Logger.Error("Cannot abort as it has already been aborted");
				return;
			}

			//Set the abort state
			Logger.Info("Updating Abort State...");
			aborting = true;
			
			//Terminate
			queueUpdatedEvent.Set();
		}

		public void Dispose()
		{
			disposed = true;
			Close();
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
