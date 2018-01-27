using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payloads
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	internal class PresenceUpdate
	{
		[JsonProperty("pid")]
		public int PID { get; set; }

		[JsonProperty("activity")]
		public RichPresence Presence { get; set; }
	}
}
