using System;
using UnityEngine;

/// <summary>
/// A special time class that can convert all manners of time into timestamps.
/// </summary>
[System.Serializable]
public class DiscordTimestamp
{
	/// <summary>
	/// Representation of a invalid timestamp (unix epoch of 0 seconds).
	/// </summary>
	public static readonly DiscordTimestamp Invalid = new DiscordTimestamp(0L);

	/// <summary>
	/// The linux epoch of the timestamp. Use conversion methods such as <see cref="GetTime"/> to convert the time into unity relative times.
	/// <para>This is used for implicit casting into a <see cref="long"/></para>
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
	/// Creates a new stamp that is relative to the Unity Startup time, where "now" is equal too <see cref="UnityEngine.Time.realtimeSinceStartup"/>.
	/// </summary>
	/// <param name="time">The time relative to <see cref="UnityEngine.Time.realtimeSinceStartup"/></param>
	public DiscordTimestamp(float time)
	{
		//Calculate the difference
		float diff = time - UnityEngine.Time.realtimeSinceStartup;

		//Convert to timespan and then to unix epoch
		TimeSpan timespan = TimeSpan.FromSeconds(diff);		//Convert the difference to a TimeSpan for easier maths
		timestamp = ToUnixTime(DateTime.UtcNow + timespan);	//Add the difference to the current time
	}

	/// <summary>
	/// Converts the timestamp into a <see cref="DateTime"/>
	/// <para>This is used for implicit conversion into a <see cref="DateTime"/></para>
	/// </summary>
	/// <returns></returns>
	public DateTime GetDateTime()
	{
		return FromUnixTime(timestamp);
	}

	/// <summary>
	/// Converts the timestamp into the number of seconds since the startup of the game (Unity relative), where "now" is equal too <see cref="UnityEngine.Time.realtimeSinceStartup"/>.
	/// <para>This is used for implicit convertion into a <see cref="float"/>.</para>
	/// </summary>
	/// <returns></returns>
	public float GetTime()
	{
		DateTime time = GetDateTime();
		TimeSpan timespan = time - DateTime.UtcNow;
		return UnityEngine.Time.realtimeSinceStartup + (float)timespan.TotalSeconds;
	}

	/// <summary>
	/// Checks if the timestamp is valid (above 0 seconds relative to unix epoch).
	/// </summary>
	/// <returns>Returns true if the timestamp is a non-zero epoch.</returns>
	public bool IsValid() { return this.timestamp > 0; }

	/// <summary>
	/// Adds seconds onto the timestamp.
	/// </summary>
	/// <param name="seconds">The number of seconds to add</param>
	/// <returns>Returns the same timestamp object.</returns>
	public DiscordTimestamp AddSeconds(int seconds)
	{
		timestamp += seconds;
		return this;
	}

	/// <summary>
	/// Adds minutes onto the timestamp.
	/// </summary>
	/// <param name="minutes">The number of minutes to add</param>
	/// <returns>Returns the same timestamp object.</returns>
	public DiscordTimestamp AddMinutes(int minutes)
	{
		return AddSeconds(minutes * 60);
	}

	/// <summary>
	/// Adds minutes onto the timestamp, rounding off to the nearest second.
	/// </summary>
	/// <param name="minutes">The fraction of minutes to add</param>
	/// <returns>Returns the same timestamp object.</returns>
	public DiscordTimestamp AddMinutes(float minutes)
	{
		//Convert the time into a integer form
		int mins = Mathf.FloorToInt(minutes);
		int secs = Mathf.RoundToInt((minutes - mins) * 60);

		return AddMinutes(mins).AddSeconds(secs);
	}

	/// <summary>
	/// Adds hours onto the timestamp.
	/// </summary>
	/// <param name="hours">The number of hours to add</param>
	/// <returns>Returns the same timestamp object</returns>
	public DiscordTimestamp AddHours(int hours)
	{
		return AddMinutes(hours * 60);
	}

	/// <summary>
	/// Adds hours onto the timestamp, rounding off to the nearest second.
	/// </summary>
	/// <param name="hours">The fraction of hours to add</param>
	/// <returns>Returns the same timestamp object.</returns>
	public DiscordTimestamp AddHours(float hours)
	{
		int h = Mathf.FloorToInt(hours);
		float m = (hours - h) * 60f;
		return AddHours(h).AddMinutes(m);
	}
	

	#region Value Conversions
	/// <summary>
	/// Casts the timestamp into a unix epoch count of seconds.
	/// </summary>
	/// <param name="stamp">The timestamp</param>
	public static implicit operator long(DiscordTimestamp stamp)
	{
		return stamp.timestamp;
	}
	/// <summary>
	/// Converts the timestamp into a unity epoch (start of the game time as origin) count of seconds, where <see cref="UnityEngine.Time.realtimeSinceStartup"/> is now.
	/// </summary>
	/// <param name="stamp">The timestamp</param>
	public static implicit operator float(DiscordTimestamp stamp)
	{
		return stamp.GetTime();
	}
	/// <summary>
	/// Converts the timestamp into a <see cref="DateTime"/> representation.
	/// </summary>
	/// <param name="stamp">The timestamp</param>
	public static implicit operator DateTime(DiscordTimestamp stamp)
	{
		return stamp.GetDateTime();
	}
	#endregion

	#region Stamp Conversions
	/// <summary>
	/// Casts a unixh epoch count of seconds into a timestamp
	/// </summary>
	/// <param name="time">The time</param>
	public static implicit operator DiscordTimestamp(long time)
	{
		return new DiscordTimestamp(time);
	}
	/// <summary>
	/// Converts the <see cref="DateTime"/> into a timestamp
	/// </summary>
	/// <param name="time">The time</param>
	public static implicit operator DiscordTimestamp(DateTime time)
	{
		return new DiscordTimestamp(time);
	}
	/// <summary>
	/// Converts a unity epoch (start of game time as origin) count of seconds (where <see cref="UnityEngine.Time.realtimeSinceStartup"/> is now) into a timestamp.
	/// </summary>
	/// <param name="time">The time</param>
	public static implicit operator DiscordTimestamp(float time)
	{
		return new DiscordTimestamp(time);
	}
	#endregion

	/// <summary>
	/// Converts a Unix Epoch time into a <see cref="DateTime"/>.
	/// </summary>
	/// <param name="unixTime">The time in seconds since 1970 / 01 / 01</param>
	/// <returns></returns>
	public static DateTime FromUnixTime(long unixTime)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return epoch.AddSeconds(unixTime);
	}

	/// <summary>
	/// Converts a <see cref="DateTime"/> into a Unix Epoch time.
	/// </summary>
	/// <param name="date">The datetime to convert</param>
	/// <returns></returns>
	public static long ToUnixTime(DateTime date)
	{
		var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
		return Convert.ToInt64((date - epoch).TotalSeconds);
	}
}