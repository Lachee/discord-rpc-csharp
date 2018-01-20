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
			Connected
		}

		#region Globals
		public static readonly int VERSION = 1;
		public static RpcConnection Instance { get { return _instance; } }
		private static RpcConnection _instance;

		#endregion

		#region Properties
		public bool IsOpen { get { return state == State.Connected && connection.IsOpen; } }
		public State CurrentState { get { return state; } }
		public string ApplicationID { get; }
		public string LastErrorMessage { get { return lastErrorMessage; } }
		public ErrorCode LastErrorCode { get { return lastErrorCode; } }
		#endregion

		#region Events
		public event RpcDisconnectEvent OnDisconnect;
		public event RpcConnectEvent OnConnect;
		public event DiscordErrorEvent OnError;
		#endregion

		#region Privates
		private State state = State.Disconnected;
		private PipeConnection connection;

		private string lastErrorMessage;
		private ErrorCode lastErrorCode;
		private int nonce = 1;
		#endregion


		public RpcConnection(string appid)
		{
			this.connection = new PipeConnection();
			this.ApplicationID = appid;
			this.state = State.Disconnected;
			_instance = this;
		}

		/// <summary>
		/// Confirms the connection is established and will connect if it is not.
		/// </summary>
		/// <returns>True if connected.</returns>
		internal async Task<bool> AttemptConnection()
		{
			switch (state)
			{
				default:
					DiscordClient.WriteLog("RPC is in a invalid state");
					return false;

				case State.Connected:
					DiscordClient.WriteLog("RPC already open");
					return true;

				case State.Disconnected:

					//Try and create a connection. Open cannot be Async
					DiscordClient.WriteLog("Attempting to connect...");					
					if (!await connection.OpenAsync())
					{
						DiscordClient.WriteLog("RPC failed to create connection with discord!");
						lastErrorCode = ErrorCode.PipeException;
						lastErrorMessage = "Failed to establish connection with pipe";
						return false;
					}

					await Task.Delay(1000);

					//Send the handshake
					DiscordClient.WriteLog("Sending Handhsake...");
					state = State.SentHandshake;
					WriteFrame(new MessageFrame(Opcode.Handshake, new Handshake() { ClientID = this.ApplicationID, Version = VERSION }));
					DiscordClient.WriteLog("Done, waiting for response...");

					await Task.Delay(1000);

					return await AttemptConnection();

				case State.SentHandshake:

					DiscordClient.WriteLog("Waiting for handshake, checking events");

					//Try to read the handshake
					ResponsePayload payload = await ReadEventAsync();
					if (payload == null)
					{
						DiscordClient.WriteLog("RPC failed to establish handshake.");
						lastErrorCode = ErrorCode.PipeException;
						lastErrorMessage = "Failed to establish handshake.";
						return false;
					}

					DiscordClient.WriteLog("Found an event!");

					//It was a connect event
					if (payload.Command == Command.Dispatch && payload.Event == SubscriptionEvent.Ready)
					{
						DiscordClient.WriteLog("Connection established with RPC.");

						//WE connected!
						state = State.Connected;
						OnConnect?.Invoke(this, new RpcConnectEventArgs() { Payload = payload });
						return true;
					}

					//It was not a connect event, so ignore
					return false;

			}
		}

		internal bool ReadEvent(out ResponsePayload payload)
		{
			//Set the inital payload
			payload = null;

			//We are not in a valid state
			if (state != State.Connected && state != State.SentHandshake)
				return false;

			while (true)
			{
				//Prepare the frame
				MessageFrame frame = null;

				//Read the message
				try
				{
					frame = MessageFrame.Read(connection);
					if (frame == null) return false;
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
					//We received an actual payload
					case Opcode.Frame:
						payload = JsonConvert.DeserializeObject<ResponsePayload>(frame.Message);
						return true;

					//It is a ping, so we need to respond with a pong
					case Opcode.Ping:

						//Change the opcode and send it away again
						frame.Opcode = Opcode.Pong;
						WriteFrame(frame);

						//Continue reading for messages.
						break;

					//Its a pong, se we shall do nothing. We will read the next message
					case Opcode.Pong: break;

					//Close the socket
					case Opcode.Close:
						DiscordClient.WriteLog("RPC Closing due to received message.");
						PipeError closeEvent = JsonConvert.DeserializeObject<PipeError>(frame.Message);
						SetError(closeEvent.Code, closeEvent.Message);
						Close();
						return false;

					//Default / Unhandled Exception
					default:
					case Opcode.Handshake:
						//Something happened that wasn't suppose to happen... I am scared.
						DiscordClient.WriteLog("RPC Closing due to a bad IPC frame.");
						SetError(ErrorCode.ReadCorrupt, "Bad IPC frame!");
						Close();
						return false;
				}
			}
		}
		internal async Task<ResponsePayload> ReadEventAsync()
		{
			//We are not in a valid state
			if (state != State.Connected && state != State.SentHandshake) return null;

			//Read the frame

			while (true)
			{
				//Prepare the frame
				MessageFrame frame;

				try
				{
					//Read the message
					frame = await MessageFrame.ReadAsync(connection);
					if (frame == null) return null;
				}
				catch (IOException e)
				{
					SetError(ErrorCode.PipeException, e.Message);
					return null;
				}
				catch (Exception e)
				{
					SetError(ErrorCode.ReadCorrupt, e.Message);
					return null;
				}

				//Perform actions on each opcode
				switch (frame.Opcode)
				{
					//We received an actual payload
					case Opcode.Frame:
						return JsonConvert.DeserializeObject<ResponsePayload>(frame.Message);


					//It is a ping, so we need to respond with a pong
					case Opcode.Ping:

						//Change the opcode and send it away again
						frame.Opcode = Opcode.Pong;
						WriteFrame(frame);

						//Continue reading for messages.
						break;	

					//Its a pong, se we shall do nothing. We will read the next message
					case Opcode.Pong: break;

					//Close the socket
					case Opcode.Close:
						DiscordClient.WriteLog("RPC Closing due to received message.");
						PipeError closeEvent = JsonConvert.DeserializeObject<PipeError>(frame.Message);
						SetError(closeEvent.Code, closeEvent.Message);
						Close();
						return null;

					//Default / Unhandled Exception
					default:
					case Opcode.Handshake:						
						//Something happened that wasn't suppose to happen... I am scared.
						DiscordClient.WriteLog("RPC Closing due to a bad IPC frame.");
						SetError(ErrorCode.ReadCorrupt, "Bad IPC frame!");
						Close();
						return null;
				}
			}

		}

		#region Writers
		internal void WriteCommand(Command command, object args)
		{
			RequestPayload request = new RequestPayload()
			{
				Command = command,
				Args = args,
				Nonce = (nonce++).ToString()
			};

			WriteFrame(new MessageFrame(Opcode.Frame, request));
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
	
		internal async Task WriteCommandAsync(Command command, object args)
		{
			//The request payload
			RequestPayload request = new RequestPayload()
			{
				Command = command,
				Args = args,
				Nonce = (nonce++).ToString()
			};
			
			//Send it off
			await WriteFrameAsync(new MessageFrame(Opcode.Frame, request));
		}
		private async Task WriteFrameAsync(MessageFrame frame)
		{
			try
			{
				await frame.WriteAsync(connection);
			}
			catch (Exception e)
			{
				DiscordClient.WriteLog("Exception while trying to write frame async: {0} ", e.Message);
				lastErrorCode = ErrorCode.UnkownError;
				lastErrorMessage = "Exception while trying to write frame async: " + e.Message;
				this.Close();
			}
		}
		#endregion

		private void SetError(ErrorCode error, string message)
		{
			lastErrorCode = error;
			lastErrorMessage = message;
			OnError?.Invoke(this, new DiscordErrorEventArgs() { ErrorCode = error, Message = message });
		}

		#region Disposal
		public void Close()
		{
			//Send a disconnect event
			if (OnDisconnect != null && (state == State.Connected || state == State.SentHandshake))
				OnDisconnect(this, new RpcDisconnectEventArgs() { ErrorCode = lastErrorCode, ErrorMessage = lastErrorMessage });

			if (connection != null)
				connection.Dispose();

			state = State.Disconnected;
		}
		public void Dispose()
		{
			this.Close();
		}
		#endregion
	}
} 