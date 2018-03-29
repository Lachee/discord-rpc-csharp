using DiscordRPC.Example.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Example.Server
{
	struct Frame
	{
		public Opcode OP { get; set; }
		public string Json { get; set; }
	}
}
