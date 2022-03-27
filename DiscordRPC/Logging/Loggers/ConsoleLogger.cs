using System;

namespace DiscordRPC.Logging.Loggers
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
        /// Should the output be colored?
        /// </summary>
        public bool Colored;
        
        /// <summary>
        /// Creates a new instance of a ConsoleLogger with a set log level
        /// </summary>
        /// <param name="level"></param>
        /// <param name="coloured"></param>
        public ConsoleLogger(LogLevel level = LogLevel.Info, bool coloured = false)
        {
            Level = level;
            Colored = coloured;
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;
            Log(ConsoleColor.Gray, $"TRACE: {message}", args);
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info) return;
			Log(ConsoleColor.White, $"INFO: {message}", args);
		}

		/// <summary>
		/// Warning log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Warning(string message, params object[] args)
		{
			if (Level > LogLevel.Warning) return;
			Log(ConsoleColor.Yellow, $"WARN: {message}", args);
		}

		/// <summary>
		/// Error log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Error(string message, params object[] args)
		{
			if (Level > LogLevel.Error) return;
			Log(ConsoleColor.Red, $"ERR: {message}", args);
		}

		/// <summary>
		/// Log the message to the console when the Trace, Info, Warning or Error method is called.
		/// </summary>
		/// <param name="color"></param>
		/// <param name="message"></param>
		/// <param name="args"></param>
		private void Log(ConsoleColor color, string message, params object[] args)
		{
			if (Colored) Console.ForegroundColor = color;
			Console.WriteLine(message, args);
		}
    }
}