using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
	[System.Obsolete("RPC Authentication is not supported by this library.")]
	internal class AuthenticateCommand : ICommand
	{	
		[JsonProperty("access_token")]
		public string AccessToken { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Command.Authenticate
			};
		}
	}
}
