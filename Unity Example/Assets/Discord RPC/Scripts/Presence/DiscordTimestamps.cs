using System;
using UnityEngine;

/// <summary>
/// A special time class that can convert all manners of time into timestamps.
/// </summary>
[System.Serializable]
public class DiscordTimestamp
{
	/// <summary>
	/// The stored timestamp
	/// </summary>
	[Tooltip("Unix Epoch Timestamp")]
	public long timestamp = 0;

	/// <summary>
	/// Creates a new stamp of the current time.
	/// </summary>
	public DiscordTimestamp() : this(DateTime.UtcNow) { }

	/// <summary>
	/// Creates a new stamp with the supplied datetime
	/// </summary>
	/// <param name="time">The DateTime</param>
	public DiscordTimestamp(DateTime time) : this(ToUnixTime(DateTime.UtcNow)) { }
	
	/// <summary>
	/// Creates a new stamp with the specified unix epoch
	/// </summary>
	/// <param name="timestamp">The time in unix epoch</param>
	public DiscordTimestamp(long timestamp)
	{
		this.timestamp = timestamp;
	}

	/// <summary>
	/// Creates a new stamp that is relative to the Unity Startup time. See <see cref="UnityEngine.Time.realtimeSinceStartup"/>
	/// </summary>
	/// <param name="time">The time relative to <see cref="UnityEngine.Time.realtimeSinceStartup"/></param>
	public DiscordTimestamp(float time)
	{
		//Calculate the difference
		float diff = time - UnityEngine.Time.realtimeSinceStartup;

		//Miliseconds
		TimeSpan timespan = TimeSpan.FromSeconds(diff);
		timestamp = ToUnixTime(DateTime.UtcNow + timespan);
	}

	/// <summary>
	/// Converts the timestamp into a datetime
	/// </summary>
	/// <returns></returns>
	public DateTime GetDateTime()
	{
		return FromUnixTime(timestamp);
	}

	/// <summary>
	/// Converss the timestamp into a <see cref="UnityEngine.Time.realtimeSinceStartup"/> relative time.
	/// </summary>
	/// <returns></returns>
	public float GetTime()
	{
		DateTime time = GetDateTime();
		TimeSpan timespan = time - DateTime.UtcNow;
		return UnityEngine.Time.realtimeSinceStartup + (float)timespan.TotalSeconds;
	}

	#region Value Conversions
	public static implicit operator long(DiscordTimestamp stamp)
	{
		return stamp.timestamp;
	}
	public static implicit operator float(DiscordTimestamp stamp)
	{
		return stamp.GetTime();
	}
	public static implicit operator DateTime(DiscordTimestamp stamp)
	{
		return stamp.GetDateTime();
	}
	#endregion

	#region Stamp Conversions
	public static implicit operator DiscordTimestamp(long time)
	{
		return new DiscordTimestamp(time);
	}
	public static implicit operator DiscordTimestamp(DateTime time)
	{
		return new DiscordTimestamp(time);
	}
	public static implicit operator DiscordTimestamp(float time)
	{
		return new DiscordTimestamp(time);
	}
	#endregion


	private static DateTime FromUnixTime(long unixTime)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return epoch.AddSeconds(unixTime);
	}
	private static long ToUnixTime(DateTime date)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return Convert.ToInt64((date - epoch).TotalSeconds);
	}
}