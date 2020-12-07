using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
	internal class GetVoiceSettingsCommand : ICommand
	{	public IPayload PreparePayload(long nonce)
		{
			return new ArgumentPayload(this, nonce)
			{
				Command = Command.GetVoiceSettings
			};
		}
	}
}
