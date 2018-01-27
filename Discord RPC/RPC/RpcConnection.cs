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
	public class RpcConnection : IDisposable
	{
		public static readonly int VERSION = 1;

		private Thread thread;
		private PipeConnection pipe;
		private bool waitingForHandshake = false;

		private int PID;
		private int _nonce = 1;

		public bool IsRunning { get { bool tmp; lock (objlock) tmp = _isrunning; return tmp; } }
		private bool _isrunning = false;

		public string ApplicationID { get { string tmp; lock (objlock) tmp = (string)_applicationid.Clone(); return tmp; } }
		private string _applicationid;

		public enum ConnectionState { Disconnected, SentHandshake, Connected }
		public ConnectionState State { get { ConnectionState tmp; lock (objlock) tmp = _state; return tmp; } }
		private ConnectionState _state = ConnectionState.Disconnected;

		private RichPresence _currentPresence;
		private RichPresence _queue;
		
		private object preslock = new object();
		private object objlock = new object();

		public RpcConnection(string applicationID)
		{
			this._applicationid = applicationID;
			PID = System.Diagnostics.Process.GetCurrentProcess().Id;
		}

		public void SetPresence(RichPresence p)
		{
			//Clone the presence into the queue
			lock (preslock)
				_queue = p.Clone();

			//Make sure we are connected
			TryInitialize();

			//Write the queue. Probably should be in the new thread, but meh.		
			WriteQueue();
		}

		public void TryInitialize()
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

			pipe = new PipeConnection();
			thread = new Thread(MainLoop);
			thread.Start();
		}

		private void MainLoop()
		{
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
						//Write any queue we have aquired
						LogDebug("Writing any queued items...");
						WriteQueue();

						LogDebug("Waiting for frame...");
						PipeFrame frame = pipe.ReadFrame();
						ProcessFrame(frame);
					}
					catch(ThreadAbortException)
					{
						Thread.ResetAbort();
						break;
					}
					catch (Exception e)
					{
						LogError("An exception occured while processing a frame: {0}", e.Message);
						break;
					}
				}

			}

			lock (objlock)
			{
				_isrunning = false;
				_state = ConnectionState.Disconnected;
			}

			LogDebug("Left main loop!");
			thread = null;

			if (pipe != null)
			{
				pipe.Dispose();
				pipe = null;
			}
		}

		private void WriteQueue()
		{
			if (State != ConnectionState.Connected) return;
			if (!IsRunning) return;

			RichPresence temp;
			lock (preslock)
			{
				temp = _queue;
				_queue = null;
			}

			if (temp == null) return;
			WriteRequest(Command.SetActivity, new PresenceUpdate() { PID = this.PID, Presence = temp });
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
				if (_state == ConnectionState.SentHandshake)
				{
					if (response.Command == Command.Dispatch && response.Event.HasValue && response.Event.Value == SubscriptionType.Ready)
					{
						LogDebug("Connectino established with the RPC");
						_state = ConnectionState.Connected;

						//TODO: Send potential OnConnectEvent?
					}

					return;
				}
			}

			//Some other response was received apparently
			LogDebug("Found Item");
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
			lock (objlock) _state = ConnectionState.SentHandshake;
			return true;
		}

		#region IO
		
		private void WriteRequest(Command command, object data)
		{
			WriteFrame(Opcode.Frame, new RequestPayload() { Command = command, Args = data, Nonce = (this._nonce++).ToString() });
		}

		private void WriteFrame(Opcode opcode, object obj)
		{
			//Prepare the frame
			PipeFrame frame = new PipeFrame()
			{
				Opcode = opcode,
				Message = JsonConvert.SerializeObject(obj)
			};

			//Send to the pipe
			pipe.WriteFrame(frame);
		}

		#endregion

		#region Utils
		public void Dispose()
		{
			if (thread != null)
			{
				thread.Abort();
				thread = null;
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
