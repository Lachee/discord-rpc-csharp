using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// Structure representing the start and endtimes of a match.
	/// </summary>
	[Serializable]
	public class Timestamps
	{
		/// <summary>A new timestamp that starts from the current time.</summary>
		public static Timestamps Now { get { return new Timestamps(DateTime.UtcNow); } }

		/// <summary>
		/// Creates a new timestamp starting at the current time and ending in the supplied timespan
		/// </summary>
		/// <param name="seconds">How long the Timestamp will last for in seconds.</param>
		/// <returns>Returns a new timestamp with given duration.</returns>
		public static Timestamps FromTimeSpan(double seconds) { return FromTimeSpan(TimeSpan.FromSeconds(seconds)); }

		/// <summary>
		/// Creates a new timestamp starting at current time and ending in the supplied timespan
		/// </summary>
		/// <param name="timespan">How long the Timestamp will last for.</param>
		/// <returns>Returns a new timestamp with given duration.</returns>
		public static Timestamps FromTimeSpan(TimeSpan timespan)
		{
			return new Timestamps()
			{
				Start = DateTime.UtcNow,
				End = DateTime.UtcNow + timespan
			};
		}

		/// <summary>
		/// The time that match started. When included (not-null), the time in the rich presence will be shown as "00:01 elapsed".
		/// </summary>
		[JsonIgnore]
		public DateTime? Start { get; set; }

		/// <summary>
		/// The time the match will end. When included (not-null), the time in the rich presence will be shown as "00:01 remaining". This will override the "elapsed" to "remaining".
		/// </summary>
		[JsonIgnore]
		public DateTime? End { get; set; }

		/// <summary>
		/// Creates a empty timestamp object
		/// </summary>
		public Timestamps()
		{
			Start = null;
			End = null;
		}

		/// <summary>
		/// Creates a timestamp with the set start time
		/// </summary>
		/// <param name="start"></param>
		public Timestamps(DateTime start)
		{
			Start = start;
			End = null;
		}

		/// <summary>
		/// Creates a timestamp with a set duration
		/// </summary>
		/// <param name="start">The start time</param>
		/// <param name="end">The end time</param>
		public Timestamps(DateTime start, DateTime end)
		{
			Start = start;
			End = end;
		}

		/// <summary>
		/// Converts between DateTime and Milliseconds to give the Unix Epoch Time for the <see cref="Timestamps.Start"/>.
		/// </summary>
		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		public ulong? StartUnixMilliseconds
		{
			get
			{
				return Start.HasValue ? ToUnixMilliseconds(Start.Value) : (ulong?)null;
			}

			set
			{
				Start = value.HasValue ? FromUnixMilliseconds(value.Value) : (DateTime?)null;
			}
		}


		/// <summary>
		/// Converts between DateTime and Milliseconds to give the Unix Epoch Time  for the <see cref="Timestamps.End"/>.
		/// <seealso cref="End"/>
		/// </summary>
		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		public ulong? EndUnixMilliseconds
		{
			get
			{
				return End.HasValue ? ToUnixMilliseconds(End.Value) : (ulong?)null;
			}

			set
			{
				End = value.HasValue ? FromUnixMilliseconds(value.Value) : (DateTime?)null;
			}
		}

		/// <summary>
		/// Converts a Unix Epoch time into a <see cref="DateTime"/>.
		/// </summary>
		/// <param name="unixTime">The time in milliseconds since 1970 / 01 / 01</param>
		/// <returns></returns>
		public static DateTime FromUnixMilliseconds(ulong unixTime)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return epoch.AddMilliseconds(Convert.ToDouble(unixTime));
		}

		/// <summary>
		/// Converts a <see cref="DateTime"/> into a Unix Epoch time (in milliseconds).
		/// </summary>
		/// <param name="date">The datetime to convert</param>
		/// <returns></returns>
		public static ulong ToUnixMilliseconds(DateTime date)
		{
			var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			return Convert.ToUInt64((date - epoch).TotalMilliseconds);
		}

	}

}
