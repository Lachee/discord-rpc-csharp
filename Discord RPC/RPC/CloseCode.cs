using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	enum CloseCode
	{
		InvalidClientID = 4000,
		InvalidOrigin = 4001,
		RateLimited = 4002,
		TokenRevoke = 4003,
		InvalidVersion = 4004,
		INvalidEncoding = 4005
	}
}
