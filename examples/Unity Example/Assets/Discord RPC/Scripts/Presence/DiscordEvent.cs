using UnityEngine;

/// <summary>
/// Events to receive from Discord
/// </summary>
[System.Flags]
public enum DiscordEvent
{
	/// <summary>
	/// No Events
	/// </summary>
	None = 0,

	/// <summary>
	/// Listen to Spectate Events
	/// </summary>
	Spectate = 1,

	/// <summary>
	/// Listen to Join Events
	/// </summary>
	Join = 2,

	/// <summary>
	/// Listen for Join Requests
	/// </summary>
	JoinRequest = 4
}

public static class DiscordEventExtensions
{
	public static DiscordRPC.EventType ToDiscordRPC(this DiscordEvent ev)
	{
		return (DiscordRPC.EventType)((int)ev);
	}

	public static DiscordEvent ToUnity(this DiscordRPC.EventType type)
	{
		return (DiscordEvent)((int)type);
	}
}