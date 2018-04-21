using System;
using System.Collections;
using System.Collections.Generic;
using DiscordRPC.Logging;
using UnityEngine;

/// <summary>
/// This is a bridge between the Discord IPC logging and Unity Logging. Useful for debugging errors within the pipe.
/// </summary>
public class UnityLogger : DiscordRPC.Logging.ILogger
{
	public LogLevel Level { get; set; }
	public void Info(string message, params object[] args)
	{
		if (Level != LogLevel.Info) return;
		Debug.Log(string.Format("<IPC> " + message, args));
	}

	public void Warning(string message, params object[] args)
	{
		if (Level != LogLevel.Info && Level != LogLevel.Warning) return;
		Debug.LogWarning(string.Format("<IPC> " + message, args));
	}

	public void Error(string message, params object[] args)
	{
		if (Level != LogLevel.Info && Level != LogLevel.Warning && Level != LogLevel.Error) return;
		Debug.LogError(string.Format("<IPC> " + message, args));
	}

}
