#if !DISABLE_MSLOGGEREXTENSION
using System;
using Microsoft.Extensions.Logging;
using ILogger    = Microsoft.Extensions.Logging.ILogger;
using ILoggerRpc = DiscordRPC.Logging.ILogger;
using LogLevel   = DiscordRPC.Logging.LogLevel;

namespace Hi3Helper.SharpDiscordRPC.DiscordRPC.Logging
{
    /// <summary>
    /// Wraps a <see cref="ILogger"/> to a <see cref="ILoggerRpc"/>
    /// </summary>
    public class MsILoggerWrapper : ILoggerRpc
    {
        private readonly ILogger _logger;

        /// <summary>
        /// Creates a new instance of the MsILoggerWrapper
        /// <inheritdoc cref="MsILoggerWrapper"/>
        /// </summary>
        /// <param name="logger">Logger interface to be wrapped</param>
        public MsILoggerWrapper(ILogger logger)
        {
            _logger = logger;
        }
        
        private static string FormatMessage(string message, params object[] args)
        {
            return string.Format(message, args);
        }

        /// <summary>
        /// The level of logging to apply to this logger.
        /// </summary>
        public LogLevel Level { get; set; }

        /// <summary>
        /// Formats and writes a trace log message.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Trace(string message, params object[] args)
        {
            _logger.LogTrace(FormatMessage(message), args);
        }

        /// <summary>
        /// Informative log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Info(string message, params object[] args)
        {
            _logger.LogInformation(FormatMessage(message), args);
        }

        /// <summary>
        /// Warning log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Warning(string message, params object[] args)
        {
            _logger.LogWarning(FormatMessage(message), args);
        }

        /// <summary>
        /// Error log messages
        /// </summary>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Error(string message, params object[] args)
        {
            _logger.LogError(FormatMessage(message), args);
        }

        /// <summary>
        /// Error log messages
        /// </summary>
        /// <param name="ex"></param>
        /// <param name="message"></param>
        /// <param name="args"></param>
        public void Error(Exception ex, string message, params object[] args)
        {
            _logger.LogError(ex, FormatMessage(message), args);
        }
    }
}
#endif