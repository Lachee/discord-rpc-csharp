using DiscordRPC.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	internal class Payload
	{
		[JsonProperty("cmd"), JsonConverter(typeof(EnumSnakeCaseSerializer))]
		public Command Command { get; set; }

		[JsonProperty("nonce")]
		public string Nonce { get; set; }

	}

	internal class ResponsePayload : Payload
	{
		[JsonProperty("data")]
		public object Data { get; set; }

		[JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseSerializer))]
		public SubscriptionEvent Event { get; set; }
	}

	internal class RequestPayload : Payload
	{
		[JsonProperty("args")]
		public object Args { get; set; }
	}
}
