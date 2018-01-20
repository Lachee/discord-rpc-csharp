using DiscordRPC.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	/// <summary>
	/// Payloads received by RPC events
	/// </summary>
	internal class Payload
	{
		/// <summary>
		/// The type of payload
		/// </summary>
		[JsonProperty("cmd"), JsonConverter(typeof(EnumSnakeCaseSerializer))]
		public Command Command { get; set; }

		/// <summary>
		/// A incremental value to help identify payloads
		/// </summary>
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

	}

	/// <summary>
	/// Response payload received by RPC events
	/// </summary>
	internal class ResponsePayload : Payload
	{
		[JsonProperty("data")]
		public object Data { get; set; }

		[JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseSerializer))]
		public SubscriptionEvent Event { get; set; }
	}

	/// <summary>
	/// Request payload sent to RPC
	/// </summary>
	internal class RequestPayload : Payload
	{
		[JsonProperty("args")]
		public object Args { get; set; }
	}
}
