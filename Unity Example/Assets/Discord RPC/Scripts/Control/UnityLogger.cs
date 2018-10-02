using UnityEngine;
using DiscordRPC.Logging;

namespace DiscordRPC.Unity
{
    /// <summary>
    /// This is a bridge between the Discord IPC logging and Unity Logging. Useful for debugging errors within the pipe.
    /// </summary>
    public class UnityLogger : Logging.ILogger
    {
        public LogLevel Level { get; set; }

        public void Info(string message, params object[] args)
        {
            if (Level != LogLevel.Info) return;
            Debug.Log("<IPC> " + (args.Length > 0 ? string.Format(message, args) : message));
        }

        public void Warning(string message, params object[] args)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning) return;
            Debug.LogWarning("<IPC> " + (args.Length > 0 ? string.Format(message, args) : message));
        }

        public void Error(string message, params object[] args)
        {
            if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;
            Debug.LogError("<IPC> " + (args.Length > 0 ? string.Format(message, args) : message));
        }
    }
}
