using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
	internal class CloseCommand : ICommand
	{
		/// <summary>
		/// The process ID
		/// </summary>
		[JsonProperty("pid")]
		public int PID { get; set; }

		/// <summary>
		/// The rich presence to be set. Can be null.
		/// </summary>
		[JsonProperty("close_reason")]
		public string value = "Unity 5.5 doesn't handle thread aborts. Can you please close me discord?";

		public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload()
			{
				Command = Command.Dispatch,
				Nonce = null,
				Arguments = null
			};
		}
	}
}
