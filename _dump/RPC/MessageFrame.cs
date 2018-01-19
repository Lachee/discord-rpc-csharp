using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	struct MessageFrame
	{
		public Opcode opcode;
		public int length;
		public string message;
	}
}
