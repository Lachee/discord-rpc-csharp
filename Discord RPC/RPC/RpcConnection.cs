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
				//We need to send a handshake to discord
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
				catch(Exception e)
				{
					lastErrorCode = ErrorCode.ReadCorrupt;
					lastErrorMessage = e.Message;
				}

				//Perform actions on each opcode
				switch(frame.Opcode)
				{
					case Opcode.Close:
						PipeClose payload = JsonConvert.DeserializeObject<PipeClose>(frame.Message);
						lastErrorCode = payload.Code;
						lastErrorMessage = payload.Message;
						this.Close();
						return;

					case Opcode.Frame:
						throw new NotImplementedException();
						break;

					case Opcode.Ping:
						throw new NotImplementedException();
						break;

					case Opcode.Pong:
						throw new NotImplementedException();
						break;
						
					default:
					case Opcode.Handshake:
						throw new NotImplementedException();
						break;
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
			MessageFrame frame = new MessageFrame()
			{
				Opcode = opcode,
				Message = JsonConvert.SerializeObject(obj)
			};
			
		}
		
		public void Dispose()
		{
			this.Close();
		}
	}
}

}
