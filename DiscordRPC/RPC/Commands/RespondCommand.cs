using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
    internal class RespondCommand : ICommand
	{
		/// <summary>
		/// The user ID that we are accepting / rejecting
		/// </summary>
		[JsonProperty("user_id")]
		public string UserID { get; set; }

		/// <summary>
		/// If true, the user will be allowed to connect.
		/// </summary>
		[JsonIgnore]
		public bool Accept { get; set; }

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Accept ? Command.SendActivityJoinInvite : Command.CloseActivityJoinRequest
			};
		}
	}
}
