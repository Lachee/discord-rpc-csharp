using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Command
	{
		const string SET_ACTIVITY = "SET_ACTIVITY";
		const string DISPATCH = "DISPATCH";
		const string SUBSCRIBE = "SUBSCRIBE";

		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		[JsonProperty("cmd")]
		public string Action { get; set; }
		
		[JsonProperty("args")]
		public CommandPayload Args { get; set; }
	}
}
