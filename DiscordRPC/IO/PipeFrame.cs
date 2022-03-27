using System;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace DiscordRPC.IO
{
	/// <summary>
	/// A frame received and sent to the Discord client for RPC communications.
	/// </summary>
	public struct PipeFrame
	{
		/// <summary>
		/// The maximum size of a pipe frame (16kb).
		/// </summary>
		public const int MaxSize = 16 * 1024;

		/// <summary>
		/// The opcode of the frame
		/// </summary>
		public Opcode Opcode { get; set; }

		/// <summary>
		/// The length of the frame data
		/// </summary>
		public uint Length => (uint) Data.Length;

		/// <summary>
		/// The data in the frame
		/// </summary>
		public byte[] Data { get; set; }
		
		/// <summary>
		/// The data represented as a string.
		/// </summary>
		public string Message
		{
			get => GetMessage();
			set => SetMessage(value);
		}
		
		/// <summary>
		/// Creates a new pipe frame instance
		/// </summary>
		/// <param name="opcode">The opcode of the frame</param>
		/// <param name="data">The data of the frame that will be serialized as JSON</param>
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
		private static Encoding MessageEncoding => Encoding.UTF8;

		/// <summary>
		/// Sets the data based of a string
		/// </summary>
		/// <param name="str"></param>
		private void SetMessage(string str) { Data = MessageEncoding.GetBytes(str); }

		/// <summary>
		/// Gets a string based of the data
		/// </summary>
		/// <returns></returns>
		private string GetMessage() => MessageEncoding.GetString(Data);

		/// <summary>
		/// Serializes the object into json string then encodes it into <see cref="Data"/>.
		/// </summary>
		/// <param name="obj"></param>
		public void SetObject(object obj) => SetMessage(JsonConvert.SerializeObject(obj));

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
		public T GetObject<T>() => JsonConvert.DeserializeObject<T>(GetMessage());

		/// <summary>
		/// Attempts to read the contents of the frame from the stream
		/// </summary>
		/// <param name="stream"></param>
		/// <returns></returns>
		public bool ReadStream(Stream stream)
		{
			// Try to read the opcode
			if (!TryReadUInt32(stream, out var op)) return false;

			// Try to read the length
			if (!TryReadUInt32(stream, out var len)) return false;

			var readsRemaining = len;

			// Read the contents
			using (var mem = new MemoryStream())
			{
				var chunkSize = (uint)Min(2048, len); // Read in chunks of 2KB
				var buffer = new byte[chunkSize];
				int bytesRead;
				while ((bytesRead = stream.Read(buffer, 0, Min(buffer.Length, readsRemaining))) > 0)
				{
					readsRemaining -= chunkSize;
					mem.Write(buffer, 0, bytesRead);
				}

				var result = mem.ToArray();
				if (result.LongLength != len) return false;

				Opcode = (Opcode)op;
				Data = result;
				return true;
			}

			// fun
			// if (a != null) { do { yield return true; switch (a) { case 1: await new Task(); default: lock (obj) { foreach (b in c) { for (int d = 0; d < 1; d++) { a++; } } } while (a is typeof(int) || (new Class()) != null) } goto MY_LABEL;

		}

        /// <summary>
        /// Returns minimum value between a int and a unsigned int
        /// </summary>
		private static int Min(int a, uint b)
		{
			if (b >= a) return a;
			return (int) b;
		}

		/// <summary>
		/// Attempts to read a UInt32
		/// </summary>
		/// <param name="stream"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		private static bool TryReadUInt32(Stream stream, out uint value)
		{
			// Read the bytes available to us
			var bytes = new byte[4];
			var cnt = stream.Read(bytes, 0, bytes.Length);

			// Make sure we actually have a valid value
			if (cnt != 4)
			{
				value = default(uint);
				return false;
			}

            value = BitConverter.ToUInt32(bytes, 0);
			return true;
		}   

		/// <summary>
		/// Writes the frame into the target frame as one big byte block.
		/// </summary>
		/// <param name="stream"></param>
		public void WriteStream(Stream stream)
		{
			// Get all the bytes
			var op = BitConverter.GetBytes((uint) Opcode);
			var len = BitConverter.GetBytes(Length);

			// Copy it all into a buffer
			var buff = new byte[op.Length + len.Length + Data.Length];
			op.CopyTo(buff, 0);
			len.CopyTo(buff, op.Length);
			Data.CopyTo(buff, op.Length + len.Length);

			// Write it to the stream
			stream.Write(buff, 0, buff.Length);
		}		
	}
}