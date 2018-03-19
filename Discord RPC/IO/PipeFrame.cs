using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	//TODO: Make Internal
	public struct PipeFrame
	{
		public const int MAX_SIZE = 16 * 1024;

		/// <summary>
		/// The opcode of the frame
		/// </summary>
		public Opcode Opcode { get; set; }

		/// <summary>
		/// The length of the frame data
		/// </summary>
		public uint Length { get { return (uint) Data.Length; } }

		/// <summary>
		/// The data in the frame
		/// </summary>
		public byte[] Data { get; set; }
		
		/// <summary>
		/// The data represented as a string.
		/// </summary>
		public string Message
		{
			get { return GetMessage(); }
			set { SetMessage(value); }
		}
		
		/// <summary>
		/// Gets the encoding used for the pipe frames
		/// </summary>
		public Encoding MessageEncoding { get { return Encoding.UTF8; } }

		/// <summary>
		/// Sets the data based of a string
		/// </summary>
		/// <param name="str"></param>
		private void SetMessage(string str) { Data = MessageEncoding.GetBytes(str); }

		/// <summary>
		/// Gets a string based of the data
		/// </summary>
		/// <returns></returns>
		private string GetMessage() { return MessageEncoding.GetString(Data); }

		/// <summary>
		/// Serializes the object into json string then encodes it into <see cref="Data"/>.
		/// </summary>
		/// <param name="obj"></param>
		public void SetObject(object obj)
		{
			string json = JsonConvert.SerializeObject(obj);
			SetMessage(json);
		}

		/// <summary>
		/// Sets the opcodes and serializes the object into a json string.
		/// </summary>
		/// <param name="opcode"></param>
		/// <param name="obj"></param>
		public void SetObject(Opcode opcode, object obj)
		{
			Opcode = opcode;
			SetObject(obj);
		}

		/// <summary>
		/// Deserializes the data into the supplied type using JSON.
		/// </summary>
		/// <typeparam name="T">The type to deserialize into</typeparam>
		/// <returns></returns>
		public T GetObject<T>()
		{
			string json = GetMessage();
			return JsonConvert.DeserializeObject<T>(json);
		}

				
	}
}
