using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	internal struct PipeFrame
	{
		public Opcode Opcode { get; set; }
		public int Length { get { return Data.Length; } }
		public byte[] Data { get; set; }	
		
		public string Message
		{
			get { return GetString(); }
			set { SetString(value); }
		}

		public Encoding MessageEncoding { get { return Encoding.UTF8; } }

		public void SetString(string str) { Data = MessageEncoding.GetBytes(str); }
		public string GetString() { return MessageEncoding.GetString(Data); }	
	}
}
