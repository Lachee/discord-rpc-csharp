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

        public static string ToCamelCase(this string str)
        {
            if (str == null) return null;
            return str.ToLowerInvariant().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1)).Aggregate(string.Empty, (s1, s2) => s1 + s2);
        }

        public static string ToSnakeCase(this string str)
        {
            if (str == null) return null;
            return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
        }
    }
}
