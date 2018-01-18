using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Model
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class Container
	{
		[JsonProperty("nonce")]
		public int Nonce { get; set; }

		[JsonProperty("cmd")]
		public string Command { get; set; }

		[JsonProperty("pid")]
		public int PID { get; set; }

		[JsonProperty("args")]
		public IPayload Args { get; set; }
	}
}
