using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payloads
{
	internal class Handshake
	{
		[JsonProperty("v")]
		public int Version { get; set; }

		[JsonProperty("client_id")]
		public string ClientID { get; set; }
	}
}
