using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// Information about the pictures used in the Rich Presence.
	/// </summary>
	[Serializable]
	public class Assets
	{
		/// <summary>
		/// Name of the uploaded image for the large profile artwork.
		/// <para>Max 256 characters.</para>
		/// </summary>
		/// <remarks>Allows URL to directly link to images.</remarks>
		[JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageKey
		{
			get { return _largeimagekey; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _largeimagekey, false, 256))
					throw new StringOutOfRangeException(256);

				//Get if this is a external link
				_islargeimagekeyexternal = _largeimagekey?.StartsWith("mp:external/") ?? false;

				//Reset the large image ID
				_largeimageID = null;
			}
		}
		private string _largeimagekey;

		/// <summary>
		/// Gets if the large square image is from an external link
		/// </summary>
		[JsonIgnore]
		public bool IsLargeImageKeyExternal
		{
			get { return _islargeimagekeyexternal; }
		}
		private bool _islargeimagekeyexternal;

		/// <summary>
		/// The tooltip for the large square image. For example, "Summoners Rift" or "Horizon Lunar Colony".
		/// <para>Max 128 characters.</para>
		/// </summary>
		[JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageText
		{
			get { return _largeimagetext; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _largeimagetext, false, 128))
					throw new StringOutOfRangeException(128);
			}
		}
		private string _largeimagetext;

		/// <summary>
		/// URL that is linked to when clicking on the large image in the activity card.
		/// <para>Max 256 characters.</para>
		/// </summary>
		[JsonProperty("large_url", NullValueHandling = NullValueHandling.Ignore)]
		public string LargeImageUrl
		{
			get { return _largeimageurl; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _largeimageurl, false, 256))
					throw new StringOutOfRangeException(256);

				if (!BaseRichPresence.ValidateUrl(_largeimageurl))
					throw new ArgumentException("Url must be a valid URI");
			}
		}
		private string _largeimageurl;

		/// <summary>
		/// Name of the uploaded image for the small profile artwork.
		/// <para>Max 256 characters.</para>
		/// </summary>
		/// <remarks>Allows URL to directly link to images.</remarks>
		[JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageKey
		{
			get { return _smallimagekey; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _smallimagekey, false, 256))
					throw new StringOutOfRangeException(256);

				//Get if this is a external link
				_issmallimagekeyexternal = _smallimagekey?.StartsWith("mp:external/") ?? false;

				//Reset the small image id
				_smallimageID = null;
			}
		}
		private string _smallimagekey;

		/// <summary>
		/// Gets if the small profile artwork is from an external link
		/// </summary>
		[JsonIgnore]
		public bool IsSmallImageKeyExternal
		{
			get { return _issmallimagekeyexternal; }
		}
		private bool _issmallimagekeyexternal;

		/// <summary>
		/// The tooltip for the small circle image. For example, "LvL 6" or "Ultimate 85%".
		/// <para>Max 128 characters.</para>
		/// </summary>
		[JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageText
		{
			get { return _smallimagetext; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _smallimagetext, false, 128))
					throw new StringOutOfRangeException(128);
			}
		}
		private string _smallimagetext;

		/// <summary>
		/// URL that is linked to when clicking on the small image in the activity card.
		/// <para>Max 256 characters.</para>
		/// </summary>
		[JsonProperty("small_url", NullValueHandling = NullValueHandling.Ignore)]
		public string SmallImageUrl
		{
			get { return _smallimageurl; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _smallimageurl, false, 256))
					throw new StringOutOfRangeException(256);

				if (!BaseRichPresence.ValidateUrl(_smallimageurl))
					throw new ArgumentException("Url must be a valid URI");
			}
		}
		private string _smallimageurl;

		/// <summary>
		/// The ID of the large image. This is only set after Update Presence and will automatically become null when <see cref="LargeImageKey"/> is changed.
		/// </summary>
		[JsonIgnore]
		public ulong? LargeImageID { get { return _largeimageID; } }
		private ulong? _largeimageID;

		/// <summary>
		/// The ID of the small image. This is only set after Update Presence and will automatically become null when <see cref="SmallImageKey"/> is changed.
		/// </summary>
		[JsonIgnore]
		public ulong? SmallImageID { get { return _smallimageID; } }
		private ulong? _smallimageID;

		/// <summary>
		/// Merges this asset with the other, taking into account for ID's instead of keys.
		/// </summary>
		/// <param name="other"></param>
		internal void Merge(Assets other)
		{
			//Copy over the names
			_smallimagetext = other._smallimagetext;
			_smallimageurl = other._smallimageurl;
			_largeimagetext = other._largeimagetext;
			_largeimageurl = other._largeimageurl;

			//Convert large ID
			ulong largeID;
			if (ulong.TryParse(other._largeimagekey, out largeID))
			{
				_largeimageID = largeID;
			}
			else
			{
				_largeimagekey = other._largeimagekey;
				_largeimageID = null;
			}

			//Convert the small ID
			ulong smallID;
			if (ulong.TryParse(other._smallimagekey, out smallID))
			{
				_smallimageID = smallID;
			}
			else
			{
				_smallimagekey = other._smallimagekey;
				_smallimageID = null;
			}
		}
	}

}
