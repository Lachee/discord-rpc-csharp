using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Events
{
	public delegate void RpcDisconnectEvent(object sender, RpcDisconnectEventArgs args);
	public class RpcDisconnectEventArgs : EventArgs
	{
		public RPC.ErrorCode ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
	}
}
