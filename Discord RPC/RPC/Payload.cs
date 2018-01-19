using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	internal class Payload
	{
		public Command Command { get; set; }
		public string Nonce { get; set; }
		public SubscriptionEvent Event { get; set; }
	}

	internal class ResponsePayload : Payload
	{
		public object Data { get; set; }
	}

	internal class RequestPayload : Payload
	{
		public object Args { get; set; }
	}
}
