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
		[System.Obsolete("Use Coloured")]
		public bool Colored {
			get => Coloured;
			set => Coloured = value;
		}
       
        /// <summary>
        /// Creates a new instance of a Console Logger.
        /// </summary>
        public ConsoleLogger()
        {
            this.Level = LogLevel.Info;
            Coloured = false;
        }

		/// <summary>
		/// Creates a new instance of a Console Logger
		/// </summary>
		/// <param name="level">The log level</param>
		public ConsoleLogger(LogLevel level)
			: this()
        {
			Level = level;
        }

        /// <summary>
        /// Creates a new instance of a Console Logger with a set log level
        /// </summary>
        /// <param name="level">The log level</param>
        /// <param name="coloured">Should the logs be in colour?</param>
        public ConsoleLogger(LogLevel level, bool coloured)
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

			string prefixedMessage = "TRACE: " + message;

			if (args.Length > 0)
			{
				Console.WriteLine(prefixedMessage, args);
			}
			else
			{
				Console.WriteLine(prefixedMessage);
			}
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

			string prefixedMessage = "INFO: " + message;

			if (args.Length > 0)
			{
				Console.WriteLine(prefixedMessage, args);
			}
			else
			{
				Console.WriteLine(prefixedMessage);
			}
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

			string prefixedMessage = "WARN: " + message;

			if (args.Length > 0)
			{
				Console.WriteLine(prefixedMessage, args);
			}
			else
			{
				Console.WriteLine(prefixedMessage);
			}
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

			string prefixedMessage = "ERR : " + message;

			if (args.Length > 0)
			{
				Console.WriteLine(prefixedMessage, args);
			}
			else
			{
				Console.WriteLine(prefixedMessage);
			}
		}

	}
}
