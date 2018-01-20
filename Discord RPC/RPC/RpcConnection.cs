using DiscordRPC.IO;
using DiscordRPC.RPC.Payloads;
using DiscordRPC.Events;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace DiscordRPC.RPC
{
	class RpcConnection : IDisposable
	{
		public enum State
		{
			Disconnected,
			SentHandshake,
			AwaitingRepsonse,
			Connected
		}

		#region Globals
		public static readonly int VERSION = 1;
		public static RpcConnection Instance { get { return _instance; } }
		private static RpcConnection _instance;

		#endregion

		#region Properties
		public bool IsOpen { get { return state == State.Connected; } }
		public State CurrentState { get { return state; } }
		public string ApplicationID { get; }
		public string LastErrorMessage { get { return lastErrorMessage; } }
		public ErrorCode LastErrorCode { get { return lastErrorCode; } }
		#endregion

		#region Events
		public event RpcDisconnectEvent OnDisconnect;
		public event RpcConnectEvent OnConnect;
		#endregion

		#region Privates
		private State state = State.Disconnected;
		private PipeConnection connection;

		private string lastErrorMessage;
		private ErrorCode lastErrorCode;
		#endregion


		public RpcConnection(string appid)
		{
			this.connection = new PipeConnection();
			this.ApplicationID = appid;
			this.state = State.Disconnected;
			_instance = this;
		}

		public void AttemptConnection()
		{
			//We are already open, nothing else we can do
			if (state == State.Connected)
			{
				DiscordClient.WriteLog("Cannot open RPC as it is already connected");
				return;
			}

			//We are disconnected, so try and open it
			if (state == State.Disconnected && !connection.Open())
			{
				DiscordClient.WriteLog("We faild to open the connection");
				return;
			}

			if (state == State.SentHandshake)
			{
				DiscordClient.WriteLog("Sent handshake, so just going to update it instead");
				CheckHandshakeResponse();
			}
			else
			{
				DiscordClient.WriteLog("Sending Handshake");

				//Write the handshake
				WriteFrame(Opcode.Handshake, new Handshake()
				{
					Version = VERSION,
					ClientID = ApplicationID
				});

				state = State.SentHandshake;
				while (state != State.Connected) CheckHandshakeResponse();
			}
		}

		private void CheckHandshakeResponse()
		{
			//Try and read the payload
			ResponsePayload payload;
			if (ReadEvent(out payload))
			{
				//It was a connect event
				if (payload.Command == Command.Dispatch && payload.Event == SubscriptionEvent.Ready)
				{
					DiscordClient.WriteLog("We have connected!");

					//WE connected!
					state = State.Connected;
					OnConnect?.Invoke(this, new RpcConnectEventArgs() { Payload = payload });
				}
			}
		}

		public bool ReadEvent(out ResponsePayload payload)
		{
			//Set the inital payload
			payload = null;

			//We are not in a valid state
			if (state != State.Connected && state != State.SentHandshake)
				return false;
			
			while (true)
			{
				//Prepare the frame
				MessageFrame frame = new MessageFrame();

				//Read the message
				try
				{
					frame.Read(connection);
				}
				catch (IOException ioe)
				{
					lastErrorCode = ErrorCode.PipeException;
					lastErrorMessage = ioe.Message;
					return false;
				}
				catch (Exception e)
				{
					lastErrorCode = ErrorCode.ReadCorrupt;
					lastErrorMessage = e.Message;
					return false;
				}

				//Perform actions on each opcode
				switch (frame.Opcode)
				{
					//Close the socket
					case Opcode.Close:

						DiscordClient.WriteLog("Close OPCODE");

						PipeError closeEvent = JsonConvert.DeserializeObject<PipeError>(frame.Message);
						lastErrorCode = closeEvent.Code;
						lastErrorMessage = closeEvent.Message;
						this.Close();
						return false;

					//We received an actual payload
					case Opcode.Frame:
						payload = JsonConvert.DeserializeObject<ResponsePayload>(frame.Message);					
						return true;

					//It is a ping, so we need to respond with a pong
					case Opcode.Ping:
						frame.Opcode = Opcode.Pong;
						WriteFrame(frame);
						return false;

					//Its a pong, se we shall do nothing
					case Opcode.Pong:
						return false;
						
					//Something has happened and we got a opcode we are not expecting!
					default:
					case Opcode.Handshake:

						DiscordClient.WriteLog("Bad IPC Frame!");

						//Something happened that wasn't suppose to happen... I am scared.
						lastErrorCode = ErrorCode.ReadCorrupt;
						lastErrorMessage = "Bad IPC frame!";
						this.Close();
						return false;
				}
			}
		}

		public void Close()
		{
			//Send a disconnect event
			if (OnDisconnect != null && (state == State.Connected || state == State.SentHandshake))
				OnDisconnect(this, new RpcDisconnectEventArgs() { ErrorCode = lastErrorCode, ErrorMessage = lastErrorMessage });

			if (connection != null)
				connection.Dispose();

			state = State.Disconnected;
		}

		private void WriteFrame(Opcode opcode, object obj)
		{
			WriteFrame(new MessageFrame()
			{
				Opcode = opcode,
				Message = JsonConvert.SerializeObject(obj)
			});
		}
		private void WriteFrame(MessageFrame frame)
		{
			try
			{
				frame.Write(connection);
			}
			catch (Exception e)
			{
				DiscordClient.WriteLog("Exception while trying to write frame: {0} ", e.Message);

				lastErrorCode = ErrorCode.UnkownError;
				lastErrorMessage = "Exception while trying to write frame: " + e.Message;
				this.Close();
			}
		}
		
		public void Dispose()
		{
			this.Close();
		}
	}
} 