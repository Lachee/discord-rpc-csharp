using DiscordRPC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{

	struct MessageFrame
	{
		public Opcode Opcode { get; set; }
		public string Message { get; set; }
		public int Length { get { return Message.Length; } }
		public void Write(PipeConnection connection)
		{
			connection.Write((int)Opcode);
			connection.Write(Length);
			connection.Write(Message);
		}
	}
}
