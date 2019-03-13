using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Logging
{
	/// <summary>
	/// Logs the outputs to a file
	/// </summary>
	public class FileLogger : ILogger
	{
		/// <summary>
		/// The level of logging to apply to this logger.
		/// </summary>
		public LogLevel Level { get; set; }

		/// <summary>
		/// Should the output be coloured?
		/// </summary>
		public string File { get; set; }

		private object filelock;

        /// <summary>
        /// Creates a new instance of the file logger
        /// </summary>
        /// <param name="path">The path of the log file.</param>
        public FileLogger(string path)
            : this(path, LogLevel.Info) { }

        /// <summary>
        /// Creates a new instance of the file logger
        /// </summary>
        /// <param name="path">The path of the log file.</param>
        /// <param name="level">The level to assign to the logger.</param>
        public FileLogger(string path, LogLevel level)
        {
            Level = level;
            File = path;
            filelock = new object();
        }


        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;
            lock (filelock) System.IO.File.AppendAllText(File, "\r\nTRCE: " + (args.Length > 0 ? string.Format(message, args) : message));
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info) return;
			lock(filelock) System.IO.File.AppendAllText(File, "\r\nINFO: " + (args.Length > 0 ? string.Format(message, args) : message));
		}

		/// <summary>
		/// Warning log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Warning(string message, params object[] args)
		{
			if (Level > LogLevel.Warning) return;
			lock (filelock)
				System.IO.File.AppendAllText(File, "\r\nWARN: " + (args.Length > 0 ? string.Format(message, args) : message));
		}

		/// <summary>
		/// Error log messsages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Error(string message, params object[] args)
		{
			if (Level > LogLevel.Error) return;
			lock (filelock)
				System.IO.File.AppendAllText(File, "\r\nERR : " + (args.Length > 0 ? string.Format(message, args) : message));
		}

	}
}
