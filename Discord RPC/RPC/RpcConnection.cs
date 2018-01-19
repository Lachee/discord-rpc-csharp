using DiscordRPC.IO;
using DiscordRPC.RPC.Payloads;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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


		public static readonly int VERSION = 1;
		public static RpcConnection Instance { get { return _instance; } }
		private static RpcConnection _instance;

		private State state = State.Disconnected;

		public string ApplicationID { get; }
		private PipeConnection connection;

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
				//Prepare the handshake
				string handshake = JsonConvert.SerializeObject(new Handshake()
				{
					Version = VERSION,
					ClientID = ApplicationID
				});

				//Write the handshake

			}
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
			if (connection != null)
				connection.Dispose();
		}
	}
}

}
