using DiscordRPC.IO;
using DiscordRPC.RPC.Payloads;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace DiscordRPC.RPC
{
	internal class RpcConnection : IDisposable
	{
		/// <summary>
		/// Version of the RPC Protocol
		/// </summary>
		public static readonly int VERSION = 1;

		//The thread and the pipe we use
		private Thread thread;
		private PipeConnection pipe;

		//Process ID to handshake with and the current nonce
		private int PID;
		private int _nonce = 1;

		/// <summary>
		/// Gets a value indicating if the connection is reading and processing  messages from Discord
		/// </summary>
		public bool IsRunning { get { bool tmp; lock (objlock) tmp = _isrunning; return tmp; } }
		private bool _isrunning = false;

		/// <summary>
		/// Gets the application used to connect too.
		/// </summary>
		public string ApplicationID { get { string tmp; lock (objlock) tmp = (string)_applicationid.Clone(); return tmp; } }
		private string _applicationid;

		/// <summary>
		/// A state of connection between the RPC Connection and Discord.
		/// </summary>
		public enum ConnectionState { Disconnected, Connecting, Connected, Disconnecting }

		/// <summary>
		/// Gets a value indicating the current state of the connection.
		/// </summary>
		public ConnectionState State { get { ConnectionState tmp; lock (objlock) tmp = _state; return tmp; } }
		private ConnectionState _state = ConnectionState.Disconnected;

		/// <summary>
		/// Gets a value indicating the current rich presence.
		/// </summary>
		public RichPresence CurrentPresence { get { RichPresence temp; lock (preslock) temp = _currentPresence.Clone(); return temp; } }
		private RichPresence _currentPresence;

		//The current queue.
		//TODO: Make this an actual queue with messages instead of just RichPresesnce
		private Queue<RichPresence> _queue;
		
		private object preslock = new object();
		private object objlock = new object();

		/// <summary>
		/// Creates a new RPC Connection 
		/// </summary>
		/// <param name="applicationID"></param>
		/// <param name="process"></param>
		public RpcConnection(string applicationID, int process)
		{
			this._applicationid = applicationID;
			PID = process;
			_queue = new Queue<RichPresence>();
		}

		/// <summary>
		/// Adds a presence update to the current queue
		/// </summary>
		/// <param name="presence">The presence to add</param>
		/// <param name="attemptConnection">If true, a connection will be established with the server</param>
		public void SetPresence(RichPresence presence, bool attemptConnection = true)
		{
			LogDebug("Setting Presence... waiting for presence lock...");

			//Clone the presence into the queue
			lock (preslock) _queue.Enqueue(presence != null ? presence.Clone() : null);

			//Make sure we are connected before we send the queue off
			if (!IsRunning)
			{
				if (attemptConnection)
				{
					//attempt a connection
					LogDebug("Trying to initialize server...");
					AttemptInitialization();
				}
				else
				{
					//We have been told don't bother with a connection. Probably a clear presence.
					LogDebug("Could not initialize the server as requested... aborting presence set.");
					return;
				}
			}
			else
			{
				//We are already running and waiting for a message. We will just process the queue now
				WriteQueue();
			}
		}
		public void Close()
		{
			LogDebug("Closing server!");


			//Set the state to disconnecting
			lock (objlock)
			{
				//If we are already disconnected, there isn't much we can do.
				if (_state == ConnectionState.Connected)
				{
					//Now close everything
					_state = ConnectionState.Disconnecting;

					//Set the presence to null
					//if (thread != null) thread.Abort();
				}
				else
				{
					LogError("Cannot close server as we are not connected!");
				}
			}

		}

		/// <summary>
		/// Creates the thread and initializes a new pipe
		/// </summary>
		private void AttemptInitialization()
		{
			if (thread != null)
			{
				LogError("Thread has already started!");
				return;
			}

			if (IsRunning)
			{
				LogError("Thread is still running!");
				return;
			}

			//If the pipe isn't null, we obviously havn't disposed of it yet.
			if (pipe != null)
				pipe.Dispose();
			
			//Create a new pipe and thread
			pipe = new PipeConnection();
			thread = new Thread(ReadLoop);

			//Start  the main loop
			thread.Start();
		}

		private void ReadLoop()
		{
			//Update the _isrunning flag to tue
			LogDebug("Entering main loop");
			lock (objlock) _isrunning = true;

			//Connect to the pipe
			LogDebug("Connecting to pipe...");
			if (Connect())
			{
				//Continously read the frame
				while (true)
				{
					try
					{
						//Write any queue we have collected during our downtime
						LogDebug("Writing any queued items...");
						WriteQueue();

						Thread.Sleep(1000);

						//Read the frame. TryReadFrame might not be 100% blocking if we read bad data!	
						LogDebug("Waiting for frame...");
											
						PipeFrame frame;
						if (!pipe.TryReadFrame(out frame))
						{
							//We have failed to read, this shouldn't really occur unless the pipe is bad
							LogError("Pipe failed to read a frame. Potentially broken pipe.");
							break;
						}

						//Process the frame
						ProcessFrame(frame);

						//Has the server been told to terminate?
						if (State == ConnectionState.Disconnecting) break;
					}
					catch(ThreadAbortException)
					{
						//We have been aborted, so reset the abort
						Thread.ResetAbort();
						break;
					}
					catch (Exception e)
					{
						//A unkown exception has occured, this isn't good.
						LogError("An exception occured while processing a frame: {0}", e.Message);
						break;
					}
				}

			}



			lock (objlock)
			{
				if (_state == ConnectionState.Disconnecting)
				{
					LogDebug("Sending last request...");
					WriteRequest(Command.SetActivity, new PresenceUpdate() { PID = this.PID, Presence = null });
				}

				//Set our state to disconnected and we are no longer running in the thread
				_isrunning = false;
				_state = ConnectionState.Disconnected;
			}
			
			//Dispose of the pipe
			if (pipe != null)
			{
				LogDebug("Disposing of the pipe object...");
				pipe.Dispose();
				pipe = null;
			}

			//Clear the thread object
			thread = null;

			LogDebug("Finished stopping thread.");
		}

		/// <summary>
		/// Processes the current queue and writes the frames to the server
		/// </summary>
		private void WriteQueue()
		{
			LogDebug("Attempting to write queue");
			if (!IsRunning) return;
			if (State != ConnectionState.Connected && State != ConnectionState.Disconnecting) return;

			LogDebug(" - Trying to get presence lock...");
			lock (preslock)
			{
				while (_queue.Count > 0)
				{
					LogDebug(" - Writing queue item");
					RichPresence presence = _queue.Dequeue();
					WriteRequest(Command.SetActivity, new PresenceUpdate() { PID = this.PID, Presence = presence });
				}
			}

			LogDebug("Done");
		}

		/// <summary>
		/// Processes a frame.
		/// </summary>
		private void ProcessFrame(PipeFrame frame)
		{
			switch(frame.Opcode)
			{
				case Opcode.Close:
					break;

				case Opcode.Frame:
					ResponsePayload response = JsonConvert.DeserializeObject<ResponsePayload>(frame.Message);
					ProcessResponse(response);
					break;

				case Opcode.Ping:
					frame.Opcode = Opcode.Pong;
					pipe.WriteFrame(frame);
					break;

				case Opcode.Pong:
					break;

				default:
				case Opcode.Handshake:
					LogError("Invalid Opcode! {0}", frame.Opcode);
					break;
			}
		}

		/// <summary>
		/// Processes the response from the pipe
		/// </summary>
		private void ProcessResponse(ResponsePayload response)
		{
			LogDebug("Processing Response...");
			LogDebug("Response: {0}", response);

			//Send some response
			lock (objlock)
			{
				//Check if its a handshake
				if (_state == ConnectionState.Connecting)
				{
					if (response.Command == Command.Dispatch && response.Event.HasValue && response.Event.Value == SubscriptionType.Ready)
					{
						LogDebug("Connection established with the RPC");
						_state = ConnectionState.Connected;

						//TODO: Send potential OnConnectEvent?

						//Process the queue we have
						WriteQueue();
					}

					return;
				}

				//It wasn't a handshake and we are not connected, so we should prob just ignore this.
				if (_state != ConnectionState.Connected)
				{
					LogDebug("Received a non-handshake frame while we are still connecting? {0}", response);
					return;
				}
			}

			//Prepare the internal data
			JObject jobj = response.Data as JObject;

			if (response.Command == Command.SetActivity)
			{
				LogDebug("Presence Found!");

				//prepare the new presence
				RichPresence newpres = null;
				if (jobj != null)
				{
					//Get the presence response
					RichPresenceResponse presresponse = jobj.ToObject<RichPresenceResponse>();
					if (presresponse != null) newpres = (RichPresence) presresponse;
				}

				//Set the presence
				lock (preslock) _currentPresence = newpres;
			}
		}

		/// <summary>
		/// Connects to the pipe and sends the handshake
		/// </summary>
		/// <returns></returns>
		private bool Connect()
		{
			if (pipe == null) return false;
			if (State != ConnectionState.Disconnected)
			{
				LogError("State must be disconnected in order to connect!");
				return false;
			}

			//Connect to the pipe
			LogDebug("Connecting...");
			if (!pipe.Connect())
			{
				LogDebug("Failed to connect.");
				return false;
			}

			//Send the handshake message
			LogDebug("Sending Handshake...");
			WriteFrame(Opcode.Handshake, new Handshake()
			{
				ClientID = this.ApplicationID,
				Version = VERSION
			});

			//Set our new state
			lock (objlock) _state = ConnectionState.Connecting;
			return true;
		}

		#region IO
		
		/// <summary>
		/// Writes a Request to the server
		/// </summary>
		/// <param name="command">The command to send to the server</param>
		/// <param name="args">The arguments of the command</param>
		private void WriteRequest(Command command, object args)
		{
			LogDebug("Trying to write request {0}", command);
			WriteFrame(Opcode.Frame, new RequestPayload() { Command = command, Args = args, Nonce = (this._nonce++).ToString() });
		}

		/// <summary>
		/// Writes a frame to the sever
		/// </summary>
		/// <param name="opcode">The OpCode of the frame</param>
		/// <param name="obj">The body of the frame</param>
		private void WriteFrame(Opcode opcode, object obj)
		{
			LogDebug("Trying to write frame {0}", opcode);

			//Prepare the frame
			PipeFrame frame = new PipeFrame()
			{
				Opcode = opcode,
				Message = JsonConvert.SerializeObject(obj)
			};

			//Send to the pipe
			pipe.WriteFrame(frame);
			LogDebug("Done {0}", opcode);
		}

		#endregion

		#region Utils
		public void Dispose()
		{
			Close();
			if (thread != null)
			{
				thread.Abort();
				thread.Join();
			}
		}
	
		public static void LogError(string message, params object[] args)
		{
			Console.WriteLine("Err: " + message, args);
		}
		public static void LogDebug(string message, params object[] args)
		{
			Console.WriteLine("Msg: " + message, args);
		}
		#endregion
	}
}
