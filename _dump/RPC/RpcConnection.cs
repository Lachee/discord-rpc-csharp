using DiscordRPC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	internal class RpcConnection : IDisposable
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
		private IConnection connection;

		public RpcConnection(string appid, IConnection connection)
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
				//We have done something else
				MessageFrame frame = new MessageFrame()
				{
					opcode = Opcode.Handshake,
					message = Newtonsoft.Json.JsonConvert.SerializeObject(command)
			}
			}
		}

		public void Dispose()
		{
			if (connection != null)
				connection.Dispose();
		}
	}
}
