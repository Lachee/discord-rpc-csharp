using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
	internal class AuthorizeCommand : ICommand
	{	
		[JsonProperty("client_id")]
		public string ClientId { get; set; }

		[JsonProperty("scopes")]
		public string[] Scopes { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Command.Authorize
			};
		}
	}
}
