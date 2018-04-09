using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
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
		
		public PipeFrame(Opcode opcode, object data)
		{
			//Set the opcode and a temp field for data
			Opcode = opcode;
			Data = null;

			//Set the data
			SetObject(data);
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

		/// <summary>
		/// Attempts to read the contents of the frame from the stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public bool ReadStream(Stream stream)
		{
			//Try to read the opcode
			uint op;
			if (!TryReadUInt32(stream, out op))
				return false;

			//Try to read the length
			uint len;
			if (!TryReadUInt32(stream, out len))
				return false;

			//Read the data. This could potentially cause issues if we ever get anything greater than a int.
			//TODO: Better implementation of this read using uints
			byte[] buff = new byte[len];
			int bytesread = stream.Read(buff, 0, buff.Length);

			//Make sure we actually read data.
			if (bytesread != len)
				return false;

			//Apply the values we read
			Opcode = (Opcode)op;
			Data = buff;
			return true;
		}

		/// <summary>
		/// Attempts to read a UInt32
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private bool TryReadUInt32(Stream stream, out uint value)
		{
			//Read the bytes available to us
			byte[] bytes = new byte[4];
			int cnt = stream.Read(bytes, 0, bytes.Length);

			//Make sure we actually have a valid value
			if (cnt != 4)
			{
				value = default(uint);
				return false;
			}

			//Flip the endianess if required then convert it to a number
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);
			value = BitConverter.ToUInt32(bytes, 0);
			return true;
		}

		/// <summary>
		/// Writes the frame into the target frame as one big byte block.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteStream(Stream stream)
		{
			//Get all the bytes
			byte[] op = ConvertBytes((uint) Opcode);
			byte[] len = ConvertBytes(Length);

			//Copy it all into a buffer
			byte[] buff = new byte[op.Length + len.Length + Data.Length];
			op.CopyTo(buff, 0);
			len.CopyTo(buff, op.Length);
			Data.CopyTo(buff, op.Length + len.Length);

			//Write it to the stream
			stream.Write(buff, 0, buff.Length);
		}


		/// <summary>
		/// Gets the bytes of a uint32 value in LE format.
		/// </summary>
		/// <param name="uint32"></param>
		/// <returns></returns>
		private byte[] ConvertBytes(uint uint32)
		{
			byte[] bytes = BitConverter.GetBytes(uint32);

			//If we are already in LE, we dont need to flip it
			if (!BitConverter.IsLittleEndian) Array.Reverse(bytes);

			//Give back the bytes
			return bytes;
		}
	}
}
