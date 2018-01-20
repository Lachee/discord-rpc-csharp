using DiscordRPC.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Events
{
	internal delegate void RpcConnectEvent(object sender, RpcConnectEventArgs args);
	internal class RpcConnectEventArgs : EventArgs
	{
		public ResponsePayload Payload { get; set; }
	}
}
