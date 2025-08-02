using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	public static class PipePermutation
	{
		const string PIPE_NAME = @"discord-ipc-{0}";

		public static IEnumerable<string> GetPipes(int startPipe = 0, int maxPipe = 10) 
		{
			bool isUnix = IsOSUnix();
			if (IsOSUnix())
				return Enumerable.Range(startPipe, maxPipe).SelectMany(GetUnixPipes);

			return Enumerable.Range(startPipe, maxPipe).SelectMany(GetWindowsPipes);
		}

		private static IEnumerable<string> GetUnixPipes(int index) 
		{
			foreach(var tempDir in TemporaryDirectories()) {
					yield return Path.Combine(tempDir, string.Format(PIPE_NAME, index));
			}
		}

		private static IEnumerable<string> GetWindowsPipes(int index) 
		{
			yield return string.Format(PIPE_NAME, index);	
		}

		private static IEnumerable<string> TemporaryDirectories() {
			string temp = null;
			temp = Environment.GetEnvironmentVariable("XDG_RUNTIME_DIR");
			if (temp != null) yield return temp;
			temp = Environment.GetEnvironmentVariable("TMPDIR");
			if (temp != null) yield return temp;
			temp = Environment.GetEnvironmentVariable("TMP");
			if (temp != null) yield return temp;
			temp = Environment.GetEnvironmentVariable("TEMP");
			if (temp != null) yield return temp;
			yield return "/temp";
		}

		private static bool IsOSUnix() {
			if (Environment.OSVersion.Platform == PlatformID.Unix)
				return true;

			#if NETFRAMEWORK // MacOS was replaced with Unix in .NET Core
			if (Environment.OSVersion.Platform == PlatformID.MacOSX)
				return true;
			#endif

			return false;
		}
	}
}
