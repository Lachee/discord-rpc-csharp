using DiscordRPC.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Events
{
	public delegate void DiscordErrorEvent(object sender, DiscordErrorEventArgs args);
	public class DiscordErrorEventArgs : EventArgs
	{
		public ErrorCode ErrorCode { get; set; }
		public string Message { get; set; }
	}
}
