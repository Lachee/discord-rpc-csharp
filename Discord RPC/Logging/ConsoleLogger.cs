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
		/// <summary>
		/// The level of logging to apply to this logger.
		/// </summary>
		public LogLevel Level { get; set; }

		/// <summary>
		/// Should the output be coloured?
		/// </summary>
		public bool Coloured { get; set; }
		
		/// <summary>
		/// A alias too <see cref="Coloured"/>
		/// </summary>
		public bool Colored { get { return Coloured; } set { Coloured = value; } }

		/// <summary>
		/// Informative log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Info(string message, params object[] args)
		{
			if (Level != LogLevel.Info) return;

			if (Coloured) Console.ForegroundColor = ConsoleColor.White;
			Console.WriteLine("INFO: " + message, args);
		}

		/// <summary>
		/// Warning log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Warning(string message, params object[] args)
		{
			if (Level != LogLevel.Info && Level != LogLevel.Warning) return;

			if (Coloured) Console.ForegroundColor = ConsoleColor.Yellow;
			Console.WriteLine("WARN: " + message, args);
		}

		/// <summary>
		/// Error log messsages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Error(string message, params object[] args)
		{
			if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;

			if (Coloured) Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("ERR : " + message, args);
		}

	}
}
