using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Model
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Command
	{
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		[JsonProperty("cmd")]
		public string Action { get; set; }
		
		[JsonProperty("args")]
		public IPayload Args { get; set; }
	}
}
