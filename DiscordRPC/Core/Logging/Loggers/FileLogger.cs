using System.IO;

namespace DiscordRPC.Core.Logging.Loggers
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
		/// The path of the log file
		/// </summary>
		private readonly string _filePath;

		/// <summary>
		/// TODO: Add documentation here?
		/// </summary>
		private readonly object _isFileLocked;

		/// <summary>
        /// Creates a new instance of the FileLogger
        /// </summary>
        /// <param name="path">The path of the log file.</param>
        /// <param name="level">The level to assign to the logger.</param>
        public FileLogger(string path, LogLevel level = LogLevel.Info)
        {
            Level = level;
            _filePath = path;
            _isFileLocked = new object();
        }
		
        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            if (Level > LogLevel.Trace) return;
            Log($"\r\nTRCE: {(args.Length > 0 ? string.Format(message, args) : message)}");
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
		{
			if (Level > LogLevel.Info) return;
			Log($"\r\nINFO: {(args.Length > 0 ? string.Format(message, args) : message)}");
		}

		/// <summary>
		/// Warning log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Warning(string message, params object[] args)
		{
			if (Level > LogLevel.Warning) return;
			Log($"\r\nWARN: {(args.Length > 0 ? string.Format(message, args) : message)}");
		}

		/// <summary>
		/// Error log messages
		/// </summary>
		/// <param name="message"></param>
		/// <param name="args"></param>
		public void Error(string message, params object[] args)
		{
			if (Level > LogLevel.Error) return;
			Log($"\r\nERR : {(args.Length > 0 ? string.Format(message, args) : message)}");
		}

		/// <summary>
		/// Log the message to the file when the Trace, Info, Warning or Error method is called.
		/// </summary>
		/// <param name="message"></param>
		private void Log(string message)
		{
			lock (_isFileLocked)
			{
				File.AppendAllText(_filePath, message);
			}
		}
	}
}