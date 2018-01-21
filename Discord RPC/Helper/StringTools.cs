using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Helper
{
	public static class StringTools
	{
		public const string Whitespace = "  ";

		/// <summary>
		/// If the string is empty, make it null instead. Otherwise return the string.
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns>Null if the string is empty, otherwise the string</returns>
		public static string ClearEmpty(this string str)
		{
			return string.IsNullOrEmpty(str) ? null : str;
		}

		/// <summary>
		/// Generates whitespaces that Discord doesn't remove.
		/// </summary>
		/// <param name="count">The number of spaces.</param>
		/// <returns>Returns the list of whitespaces</returns>
		public static string CreateWhitespace(int count)
		{
			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < count; i++) builder.Append(Whitespace);
			return builder.ToString();
		}
	}
}
