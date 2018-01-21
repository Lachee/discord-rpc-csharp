using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Helper
{
	internal static class Extentions
	{
		/// <summary>
		/// If the string is empty, make it null instead. Otherwise return the string.
		/// </summary>
		/// <param name="str">The string to check</param>
		/// <returns>Null if the string is empty, otherwise the string</returns>
		public static string Nullify(this string str)
		{
			return string.IsNullOrEmpty(str) ? null : str;
		}
	}
}
