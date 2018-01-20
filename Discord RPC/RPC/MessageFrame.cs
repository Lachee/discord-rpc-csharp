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

		public void Write(PipeConnection connection)
		{
			DiscordClient.WriteLog("Writing Connection {0}, {1}", Opcode, Message);

			connection.Writer.Write((int)Opcode);
			connection.Writer.WriteLengthString(Message, Encoding.UTF8);
		}

		public void Read(PipeConnection connection)
		{
			Opcode = (Opcode)connection.Reader.ReadInt32();
			Message = connection.Reader.ReadLengthString(Encoding.UTF8);
		}
	}
}
