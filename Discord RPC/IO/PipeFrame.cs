using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	struct PipeFrame
	{
		public int Opcode { get; set; }
		public int Length { get { return Data.Length; } }
		public byte[] Data { get; set; }		
	}
}
