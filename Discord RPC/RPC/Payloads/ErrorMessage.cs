using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payloads
{
	internal class ErrorMessage
	{
		[JsonProperty("code")]
		public ErrorCode Code { get; set; }

		[JsonProperty("message")]
		public string Message { get; set; }
	}
}
