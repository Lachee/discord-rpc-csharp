using Newtonsoft.Json;
using DiscordRPC.RPC;

namespace DiscordRPC.RPC.Payloads
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class PresenceUpdate
	{
		[JsonProperty("pid")]
		public int PID { get; set; }

		[JsonProperty("activity")]
		public RichPresence Presence { get; set; }
		
	}

}
