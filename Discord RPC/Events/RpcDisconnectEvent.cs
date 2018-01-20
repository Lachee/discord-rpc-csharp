using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Events
{
	internal delegate void RpcDisconnectEvent(object sender, RpcDisconnectEventArgs args);
	internal class RpcDisconnectEventArgs : EventArgs
	{
		public ErrorCode ErrorCode { get; set; }
		public string ErrorMessage { get; set; }
	}
}
