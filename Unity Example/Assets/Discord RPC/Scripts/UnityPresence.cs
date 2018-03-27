using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//[CreateAssetMenu(fileName = "Unity Rich Presence", menuName = "Discord/Rich Presence", order = -1)]
[Serializable]
public class UnityPresence
{
	/// <summary>
	/// The details about the game. Appears underneath the game name
	/// </summary>
	[Tooltip("The details about the game")]
	public string details = "Playing a game";

	/// <summary>
	/// The current state of the game (In Game, In Menu etc). Appears next to the party size
	/// </summary>
	[Tooltip("The current state of the game (In Game, In Menu). It appears next to the party size.")]
	public string state = "In Game";

	/// <summary>
	/// The time the game started. 0 if the game hasn't started
	/// </summary>
	[Tooltip("The time the game started. Leave as 0 if the game has not yet started.")]
	public Timestamp startTime = 0;

	/// <summary>
	/// The time the game will end in. 0 to ignore endtime.
	/// </summary>
	[Tooltip("Time the game will end. Leave as 0 to ignore it.")]
	public Timestamp endTime = 0;

	/// <summary>
	/// The images used for the presence.
	/// </summary>
	[Tooltip("The images used for the presence")]
	public Assets assets = new Assets();


	/// <summary>
	/// The current party
	/// </summary>
	[Tooltip("The current party. Identifier must not be empty")]
	public Party party = new Party("", 0, 0);

	/// <summary>
	/// Creates a new Presence object
	/// </summary>
	public UnityPresence() { }

	/// <summary>
	/// Creates a new Presence object, copying values form the supplied Discord Presence
	/// </summary>
	/// <param name="presence">The presence to copy from</param>
	public UnityPresence(DiscordRPC.RichPresence presence)
	{
		if (presence == null) return;

		//Read the state and details
		details = presence.Details;
		state = presence.State;

		//Read the timestamps
		if (presence.Timestamps != null)
		{
			if (presence.Timestamps.Start.HasValue)
				startTime = new Timestamp(presence.Timestamps.Start.Value);

			if (presence.Timestamps.End.HasValue)
				endTime = new Timestamp(presence.Timestamps.End.Value);
		}

		//Read the assets
		if (presence.Assets != null)
		{
			assets = new Assets()
			{
				largeKey = presence.Assets.LargeImageKey,
				largeTooltip = presence.Assets.LargeImageText,
				smallKey = presence.Assets.SmallImageKey,
				smallTooltip = presence.Assets.SmallImageText
			};
		}

		//Read the party
		if (presence.Party != null)
		{
			party = new Party(presence.Party.ID, presence.Party.Size, presence.Party.Max);
		}

	}

	/// <summary>
	/// Converts the Unity Presence into a Discord ready RichPresence.
	/// </summary>
	/// <returns></returns>
	public DiscordRPC.RichPresence ToRichPresence()
	{
		//Create a new presence
		var presence = new DiscordRPC.RichPresence()
		{
			State = this.state,
			Details = this.details
		};

		//Set the timestamps
		if (startTime > 0 && endTime > 0)
		{
			presence.Timestamps = new DiscordRPC.Timestamps();
			if (startTime > 0) presence.Timestamps.Start = startTime.GetDateTime();
			if (endTime > 0) presence.Timestamps.End = endTime.GetDateTime();
		}

		//Set the assets
		if (assets != null)
		{
			presence.Assets = new DiscordRPC.Assets()
			{
				LargeImageKey = assets.largeKey,
				LargeImageText = assets.largeTooltip,
				SmallImageKey = assets.smallKey,
				SmallImageText = assets.smallTooltip
			};
		}

		//Set the party
		if (party != null && !string.IsNullOrEmpty(party.identifer))
		{
			presence.Party = new DiscordRPC.Party()
			{
				ID = party.identifer,
				Size = party.size,
				Max = party.maxSize
			};
		}

		return presence;
	}

	/// <summary>
	/// A special time class that can convert all manners of time into timestamps.
	/// </summary>
	[System.Serializable]
	public class Timestamp
	{
		/// <summary>
		/// The stored timestamp
		/// </summary>
		[Tooltip("Unix Epoch Timestamp")]
		public long timestamp = 0;

		/// <summary>
		/// Creates a new stamp of the current time.
		/// </summary>
		public Timestamp() : this(DateTime.UtcNow) { }

		/// <summary>
		/// Creates a new stamp with the supplied datetime
		/// </summary>
		/// <param name="time">The DateTime</param>
		public Timestamp(DateTime time) : this(ToUnixTime(DateTime.UtcNow)) { }

		/// <summary>
		/// Creates a new stamp with the specified unix epoch
		/// </summary>
		/// <param name="timestamp">The time in unix epoch</param>
		public Timestamp(long timestamp)
		{
			this.timestamp = timestamp;
		}

		/// <summary>
		/// Creates a new stamp that is relative to the Unity Startup time. See <see cref="UnityEngine.Time.realtimeSinceStartup"/>
		/// </summary>
		/// <param name="time">The time relative to <see cref="UnityEngine.Time.realtimeSinceStartup"/></param>
		public Timestamp(float time)
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
		public static implicit operator long(Timestamp stamp)
		{
			return stamp.timestamp;
		}
		public static implicit operator float(Timestamp stamp)
		{
			return stamp.GetTime();
		}
		public static implicit operator DateTime(Timestamp stamp)
		{
			return stamp.GetDateTime();
		}
		#endregion

		#region Stamp Conversions
		public static implicit operator Timestamp(long time)
		{
			return new Timestamp(time);
		}
		public static implicit operator Timestamp(DateTime time)
		{
			return new Timestamp(time);
		}
		public static implicit operator Timestamp(float time)
		{
			return new Timestamp(time);
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

	[System.Serializable]
	public class Assets
	{
		public string largeKey;
		public string largeTooltip;

		public string smallKey;
		public string smallTooltip;
	}

	[System.Serializable]
	public class Party
	{
		/// <summary>
		/// The unique ID of the party. Leave empty for no party.
		/// </summary>
		public string identifer;

		/// <summary>
		/// The size of the party
		/// </summary>
		public int size;

		/// <summary>
		/// The max size of the party. Cannot be smaller than size.
		/// </summary>
		public int maxSize;

		public Party(string id, int size, int max)
		{
			this.identifer = id;
			this.size = size;
			this.maxSize = max;
		}

	}
}
