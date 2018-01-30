using DiscordRPC.Registry;
using DiscordRPC.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	public class DiscordRpcClient
	{
		private RpcConnection connection;
		
		public DiscordRpcClient(string appid)
		{
			UriScheme.RegisterUriScheme(appid);
		}
	}
}
