using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Logging
{
	/// <summary>
	/// Logs the outputs to the console using <see cref="Console.WriteLine()"/>
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
        /// Creates a new instance of a Console Logger.
        /// </summary>
        public ConsoleLogger()
        {
            this.Level = LogLevel.Info;
            Coloured = false;
        }

        /// <summary>
        /// Creates a new instance of a Console Logger with a set log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="coloured"></param>
        public ConsoleLogger(LogLevel level, bool coloured = false)
        {
            Level = level;
            Coloured = coloured;
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;

            if (Coloured) Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine("TRACE: " + message, args);
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info) return;

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
			if (Level > LogLevel.Warning) return;

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
			if (Level > LogLevel.Error) return;

			if (Coloured) Console.ForegroundColor = ConsoleColor.Red;
			Console.WriteLine("ERR : " + message, args);
		}

	}
}
