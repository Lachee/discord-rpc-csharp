using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	internal static class BinaryExtensions
	{
		public static void WriteLengthString(this BinaryWriter writer, string s, Encoding encoding)
		{
			byte[] bytes = encoding.GetBytes(s);
			writer.Write(bytes.Length);
			writer.Write(bytes, 0, bytes.Length);
		}

		public static string ReadLengthString(this BinaryReader reader, Encoding encoding)
		{
			int length = reader.ReadInt32();

			byte[] buff = new byte[length];
			reader.Read(buff, 0, length);

			return encoding.GetString(buff);
		}
	}
}
