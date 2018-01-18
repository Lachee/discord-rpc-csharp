using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn, ItemNullValueHandling = NullValueHandling.Ignore)]
	public struct RichPresence
	{
		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("state")]
		public string State { get; set; }
		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("details")]
		public string Details { get; set; }
		
		[JsonProperty("timestamps")]
		public Timestamps? Timestamps { get; set; }

		/// <summary>
		/// Max 32 Bytes.
		/// </summary>
		[JsonProperty("large_image")]
		public string LargeImageKey { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("large_text")]
		public string LargeImageText { get; set; }

		/// <summary>
		/// Max 32 Bytes.
		/// </summary>
		[JsonProperty("small_image")]
		public string SmallImageKey { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("small_text")]
		public string SmallImageText { get; set; }
		
		[JsonProperty("party")]
		public Party? Party { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("match")]
		public string matchSecret { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("join")]
		public string joinSecret { get; set; }

		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("spectate")]
		public string spectateSecret { get; set; }

		[JsonProperty("instance")]
		public bool instance { get; set; }

		
	}

	public struct Timestamps
	{
		[JsonProperty("start")]
		public long? Start { get; set; }

		[JsonProperty("end")]
		public long? End { get; set; }
	}

	public struct Party
	{
		/// <summary>
		/// Max 128 Bytes.
		/// </summary>
		[JsonProperty("id")]
		public string ID { get; set; }

		[JsonIgnore]
		public int? Size { get; set; }

		[JsonIgnore]
		public int? Max { get; set; }

		[JsonProperty("size")]
		private int[] _size
		{
			get
			{
				//We have no size, so its null
				if (!Size.HasValue)
					return null;

				//We have a size and a max size, so return the full lsit
				if (Max.HasValue)
					return new int[] { Size.Value, Max.Value };

				//We only have size
				return new int[] { Size.Value };
			}
		}
	}
}
