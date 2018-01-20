using DiscordRPC.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	class MessageFrame
	{
		public Opcode Opcode { get; set; }
		public string Message { get; set; }

		public MessageFrame() { }
		public MessageFrame(Opcode opcode, object obj)
		{
			Opcode = opcode;
			Message = JsonConvert.SerializeObject(obj);
		}

		public void Write(PipeConnection connection)
		{
			connection.Write((int)Opcode);
			connection.Write(Message, Encoding.UTF8);
		}
		public async Task WriteAsync(PipeConnection connection)
		{
			await connection.WriteAsync((int)Opcode);
			await connection.WriteAsync(Message, Encoding.UTF8);
		}

		public void Read(PipeConnection connection)
		{
			Opcode = (Opcode)connection.ReadInt();
			Message = connection.ReadString(Encoding.UTF8);
		}
		public static async Task<MessageFrame> ReadAsync(PipeConnection connection)
		{
			MessageFrame frame = new MessageFrame();
			frame.Opcode = (Opcode) await connection.ReadIntAsync();
			frame.Message = await connection.ReadStringAsync(Encoding.UTF8);
			return frame;
		}
		
	}
}
