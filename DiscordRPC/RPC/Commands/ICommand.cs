using DiscordRPC.RPC.Payload;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Commands
{
    internal interface ICommand
	{
		IPayload PreparePayload(long nonce);
	}
}
