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
		enum State
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


		public RpcConnection(string appid, PipeConnection connection)
		{
			this.connection = connection;
			this.ApplicationID = appid;
			this.state = State.Disconnected;
			_instance = this;
		}

		public void Open()
		{
			//We are already open, nothing else we can do
			if (state == State.Connected) return;

			//We are disconnected, so try and open it
			if (state == State.Disconnected && !connection.Open()) return;

			if (state == State.SentHandshake)
			{
				//We sent a handshake, so now all we have to do is read!
				Read();
			}
			else
			{
				//Write the handshake
				WriteFrame(Opcode.Handshake, new Handshake()
				{
					Version = VERSION,
					ClientID = ApplicationID
				});
			}
		}

		public void Read()
		{
			//We are not in a valid state
			if (state != State.Connected && state != State.SentHandshake)
				return;

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
				}
				catch (Exception e)
				{
					lastErrorCode = ErrorCode.ReadCorrupt;
					lastErrorMessage = e.Message;
				}

				//Perform actions on each opcode
				switch (frame.Opcode)
				{
					//Close the socket
					case Opcode.Close:
						PipeClose payload = JsonConvert.DeserializeObject<PipeClose>(frame.Message);
						lastErrorCode = payload.Code;
						lastErrorMessage = payload.Message;
						this.Close();
						break;

					case Opcode.Frame:
						ResponsePayload response = JsonConvert.DeserializeObject<ResponsePayload>(frame.Message);
						if (response.Command == Command.Dispatch && response.Event == SubscriptionEvent.Ready)
						{
							state = State.Connected;
							OnConnect?.Invoke(this, new RpcConnectEventArgs() { Payload = response });
						}
						break;

					//Return the frame
					case Opcode.Ping:
						frame.Opcode = Opcode.Pong;
						WriteFrame(frame);
						break;

					//Do nothing, we shouldn't really get these.
					case Opcode.Pong:
						break;
						
					default:
					case Opcode.Handshake:

						//Something happened that wasn't suppose to happen... I am scared.
						lastErrorCode = ErrorCode.ReadCorrupt;
						lastErrorMessage = "Bad IPC frame!";
						this.Close();
						break;
				}
			}
		}

		public void Close()
		{
			//TODO: Complete Error Messages
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