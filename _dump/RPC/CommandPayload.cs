using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public interface CommandPayload
	{
		[JsonProperty("pid")]
		int PID { get; set; }
	}
}
