using System;
using System.Collections;
using System.Collections.Generic;
using DiscordRPC.Logging;
using UnityEngine;

public class UnityLogger : DiscordRPC.Logging.ILogger
{
	public LogLevel Level { get; set; }


	public void Info(string message, params object[] args)
	{
		//if (Level != LogLevel.Info) return;
		Debug.Log(string.Format("Discord: " + message, args));
	}

	public void Warning(string message, params object[] args)
	{
		//if (Level != LogLevel.Warning && Level != LogLevel.Error) return;
		Debug.LogWarning(string.Format("Discord: " + message, args));
	}

	public void Error(string message, params object[] args)
	{
		//if (Level != LogLevel.Error) return;
		Debug.LogError(string.Format("Discord: " + message, args));
	}

}
