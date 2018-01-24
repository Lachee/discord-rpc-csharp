using Newtonsoft.Json;
using System;

namespace DiscordRPC
{

	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	[Serializable]
	public class RichPresence
	{
		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
		public string State { get { return _state; } set { _state = value.ClearEmpty(); } }
		private string _state;

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
		public string Details { get { return _details; } set { _details = value.ClearEmpty(); } }
		private string _details;
		
		[JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
		public Timestamps Timestamps { get; set; }

		[JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
		public Assets Assets { get; set; }
		
		[JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
		public Party Party { get; set; }

		#region Not Yet Implemented
		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
		private string MatchSecret { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		private string JoinSecret { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
		private string SpectateSecret { get; set; }

		[JsonProperty("instance", NullValueHandling = NullValueHandling.Ignore)]
		private bool Instance { get; set; }
		#endregion
		
	}

	/// <summary>
	/// A rich presence that has been parsed from the pipe as a response.
	/// </summary>
	internal class RichPresenceResponse : RichPresence
	{
		/// <summary>
		/// ID of the client
		/// </summary>
		public string ClientID { get { return _clientid; } }

		/// <summary>
		/// Name of the bot
		/// </summary>
		public string Name { get { return _name; } }

		//Disabling warning that these are never assigned. These are assigned by the JSON converter.
#pragma warning disable 0649

		[JsonProperty("application_id")]
		private string _clientid;

		[JsonProperty("name")]
		private string _name;

#pragma warning restore 0649

	}

	public class Assets
	{
		/// <summary>
		/// Max 32 Bytes.
		/// </summary>
		[JsonProperty("large_image")]//, NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageKey { get { return _largeimagekey; } set { _largeimagekey = value.ClearEmpty(); } }
		private string _largeimagekey;

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("large_text")]//, NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageText { get { return _largeimagetext; } set { _largeimagetext = value.ClearEmpty(); } }
		private string _largeimagetext;

		/// <summary>
		/// Max 32 Bytes.
		/// </summary>
		[JsonProperty("small_image")]//, NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageKey { get { return _smallimagekey; } set { _smallimagekey = value.ClearEmpty(); } }
		private string _smallimagekey;

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("small_text")]//, NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageText { get { return _smallimagetext; } set { _smallimagetext = value.ClearEmpty(); } }
		private string _smallimagetext;
	}

	public class Timestamps
	{
		[JsonIgnore]
		public DateTime? Start { get; set; }

		[JsonIgnore]
		public DateTime? End { get; set; }

		[JsonProperty("start", NullValueHandling = NullValueHandling.Ignore)]
		private long? epochStart { get { return Start.HasValue ? GetEpoch(Start.Value) : (long?)null; } }

		[JsonProperty("end", NullValueHandling = NullValueHandling.Ignore)]
		private long? epochEnd { get { return End.HasValue ? GetEpoch(End.Value) : (long?)null; } }

		public static long GetEpoch(DateTime time)
		{
			DateTime epochStart = new DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc);
			return (long)(time - epochStart).TotalSeconds;
		}
	}

	public class Party
	{
		/// <summary>
		/// A optional unique ID for the party. Max 128 Bytes.
		/// </summary>
		[JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
		public string ID { get { return _partyid; } set { _partyid = value.ClearEmpty(); } }
		private string _partyid;

		/// <summary>
		/// The current size of the party
		/// </summary>
		[JsonIgnore]
		public int Size { get; set; }

		/// <summary>
		/// The maxium size of the party. This is required to be larger than <see cref="Size"/>. If it is smaller than the current party size, it will automatically be set too <see cref="Size"/> when the presence is sent.
		/// </summary>
		[JsonIgnore]
		public int Max { get; set; }

		[JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
		private int[] _size
		{
			get
			{
				//see issue https://github.com/discordapp/discord-rpc/issues/111
				int size = Math.Max(1, Size);
				return new int[] { size, Math.Max(size, Max) };
			}
		}
	}
}
