using DiscordRPC.IO;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	internal class MessageFrame
	{
		/// <summary>
		/// The Operation 
		/// </summary>
		public Opcode Opcode { get; set; }

		/// <summary>
		/// The JSON payload
		/// </summary>
		public string Message { get; set; }

		public MessageFrame() { }
		public MessageFrame(Opcode opcode, object obj)
		{
			Opcode = opcode;
			Message = JsonConvert.SerializeObject(obj, new JsonSerializerSettings() { NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore });

			//Console.WriteLine(Message);
		}

		/// <summary>
		/// Writes the MessageFrame to the Pipe
		/// </summary>
		/// <param name="connection"></param>
		public void Write(PipeConnection connection)
		{
			/*
			//This is disabled for consistency with the WriteAsync
			connection.Write((int)Opcode);
			connection.Write(Message, Encoding.UTF8);
			*/
			connection.Write(Serialize());
		}

		/// <summary>
		/// Writes the MessageFrame to the pipe
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public async Task WriteAsync(PipeConnection connection)
		{
			await connection.WriteAsync(Serialize());
		}

		/// <summary>
		/// Converts this object into bytes
		/// </summary>
		/// <returns></returns>
		private byte[] Serialize()
		{
			byte[] opcode = BitConverter.GetBytes((int)Opcode);
			byte[] message = Encoding.UTF8.GetBytes(Message);
			byte[] length = BitConverter.GetBytes(message.Length);

			if (!BitConverter.IsLittleEndian)
			{
				opcode.Reverse();
				length.Reverse();
			}

			byte[] buffer = new byte[opcode.Length + message.Length + length.Length];
			opcode.CopyTo(buffer, 0);
			length.CopyTo(buffer, opcode.Length);
			message.CopyTo(buffer, opcode.Length + length.Length);

			return buffer;
		}

		/// <summary>
		/// Reads a new frame from the pipe. This is blocking.
		/// </summary>
		/// <param name="connection"></param>
		public static MessageFrame Read(PipeConnection connection)
		{
			MessageFrame frame = new MessageFrame();
			frame.Opcode = (Opcode)connection.ReadInt();
			frame.Message = connection.ReadString(Encoding.UTF8);
			return frame;
		}

		/// <summary>
		/// Reads a new frame from the pipe.
		/// </summary>
		/// <param name="connection"></param>
		/// <returns></returns>
		public static async Task<MessageFrame> ReadAsync(PipeConnection connection)
		{
			MessageFrame frame = new MessageFrame();
			frame.Opcode = (Opcode) await connection.ReadIntAsync();
			frame.Message = await connection.ReadStringAsync(Encoding.UTF8);
			return frame;
		}
		
	}
}
