using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	internal struct PipeFrame
	{
		/// <summary>
		/// The opcode of the frame
		/// </summary>
		public Opcode Opcode { get; set; }

		/// <summary>
		/// The length of the frame data
		/// </summary>
		public int Length { get { return Data.Length; } }

		/// <summary>
		/// The data in the frame
		/// </summary>
		public byte[] Data { get; set; }

		/// <summary>
		/// The data represented as a string.
		/// </summary>
		public string Message
		{
			get { return GetData(); }
			set { SetData(value); }
		}
		
		/// <summary>
		/// Gets the encoding used for the pipe frames
		/// </summary>
		public Encoding MessageEncoding { get { return Encoding.UTF8; } }

		/// <summary>
		/// Sets the data based of a string
		/// </summary>
		/// <param name="str"></param>
		public void SetData(string str) { Data = MessageEncoding.GetBytes(str); }

		/// <summary>
		/// Serializes the object into json string then encodes it into <see cref="Data"/>.
		/// </summary>
		/// <param name="obj"></param>
		public void SetPayload(object obj)
		{
			SetData(JsonConvert.SerializeObject(obj));
		}

		/// <summary>
		/// Sets the opcodes and serializes the object into a json string.
		/// </summary>
		/// <param name="opcode"></param>
		/// <param name="obj"></param>
		public void SetPayload(Opcode opcode, object obj)
		{
			Opcode = opcode;
			SetPayload(obj);
		}

		/// <summary>
		/// Deserializes the data into the supplied type using JSON.
		/// </summary>
		/// <typeparam name="T">The type to deserialize into</typeparam>
		/// <returns></returns>
		public T GetPayload<T>()
		{
			return JsonConvert.DeserializeObject<T>(GetData());
		}

		/// <summary>
		/// Gets a string based of the data
		/// </summary>
		/// <returns></returns>
		public string GetData() { return MessageEncoding.GetString(Data); }
				
	}
}
