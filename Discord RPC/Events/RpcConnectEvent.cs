using DiscordRPC.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Events
{
	public delegate void RpcConnectEvent(object sender, RpcConnectEventArgs args);
	public class RpcConnectEventArgs : EventArgs
	{
		public ResponsePayload Payload { get; set; }
	}
}
