using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Logging
{
	/// <summary>
	/// Logs the outputs to the console using <see cref="Console.WriteLine"/>
	/// </summary>
	public class ConsoleLogger : ILogger
	{
		public LogLevel Level { get; set; }

		public void Info(string message, params object[] args)
		{
			if (Level != LogLevel.Info) return;
			Console.WriteLine("INFO: " + message, args);
		}

		public void Warning(string message, params object[] args)
		{
			if (Level != LogLevel.Warning && Level != LogLevel.Error) return;
			Console.WriteLine("WARN: " + message, args);
		}

		public void Error(string message, params object[] args)
		{
			if (Level != LogLevel.Error) return;
			Console.WriteLine("ERR : " + message, args);
		}

	}
}
