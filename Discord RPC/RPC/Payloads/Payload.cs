using DiscordRPC.Converters;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payloads
{
	/// <summary>
	/// Payloads received by RPC events
	/// </summary>
	internal class Payload
	{
		/// <summary>
		/// The type of payload
		/// </summary>
		[JsonProperty("cmd"), JsonConverter(typeof(EnumSnakeCaseConverter))]
		public Command Command { get; set; }

		/// <summary>
		/// A incremental value to help identify payloads
		/// </summary>
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		public override string ToString()
		{
			return "Payload || Command: " + Command.ToString() + ", Nonce: " + Nonce?.ToString();
		}
	}

	/// <summary>
	/// Response payload received by RPC
	/// </summary>
	internal class ResponsePayload : Payload
	{
		[JsonProperty("data")]
		public object Data { get; set; }

		[JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseConverter))]
		public SubscriptionType? Event { get; set; }

		public override string ToString()
		{
			return "Response " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
		}
	}


	/// <summary>
	/// Request payload sent to RPC
	/// </summary>
	internal class RequestPayload : Payload
	{
		[JsonProperty("args")]
		public object Args { get; set; }

		public override string ToString()
		{
			return "Request " + base.ToString();
		}
	}
}
