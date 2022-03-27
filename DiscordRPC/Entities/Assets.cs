using System;
using System.Text;
using DiscordRPC.Exceptions;
using Newtonsoft.Json;

namespace DiscordRPC.Entities
{
    /// <summary>
    /// Information about the pictures used in the Rich Presence.
    /// </summary>
    [Serializable]
    public class Assets
    {
        /// <summary>
        /// Name of the uploaded image for the large profile artwork.
        /// <para>Max 256 Bytes.</para>
        /// </summary>
        [JsonProperty("large_image", NullValueHandling = NullValueHandling.Ignore)]
        public string LargeImageKey
        {
            get => _largeImageKey;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _largeImageKey, 256, Encoding.UTF8))
                    throw new StringOutOfRangeException(256);

                // Get if this is a external link
                _islLargeImageKeyExternal = _largeImageKey?.StartsWith("mp:external/") ?? false;
                
                // Reset the large image ID
                _largeImageId = null;
            }
        }
        private string _largeImageKey;

        /// <summary>
        /// Gets if the large square image is from an external link
        /// </summary>
        [JsonIgnore]
        public bool IsLargeImageKeyExternal => _islLargeImageKeyExternal;

        private bool _islLargeImageKeyExternal;

        /// <summary>
        /// The tooltip for the large square image. For example, "Summoners Rift" or "Horizon Lunar Colony".
        /// <para>Max 128 Bytes.</para>
        /// </summary>
        [JsonProperty("large_text", NullValueHandling = NullValueHandling.Ignore)]
        public string LargeImageText
        {
            get => _largeImageText;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _largeImageText, 128, Encoding.UTF8))
                    throw new StringOutOfRangeException(128);
            }
        }
        private string _largeImageText;


        /// <summary>
        /// Name of the uploaded image for the small profile artwork.
        /// <para>Max 256 Bytes.</para>
        /// </summary>
        [JsonProperty("small_image", NullValueHandling = NullValueHandling.Ignore)]
        public string SmallImageKey
        {
            get => _smallImageKey;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _smallImageKey, 256, Encoding.UTF8))
                    throw new StringOutOfRangeException(256);

                // Get if this is a external link
                _isSmallImageKeyExternal = _smallImageKey?.StartsWith("mp:external/") ?? false;

                // Reset the small image id
                _smallImageId = null;
            }
        }
        private string _smallImageKey;

        /// <summary>
        /// Gets if the small profile artwork is from an external link
        /// </summary>
        [JsonIgnore]
        public bool IsSmallImageKeyExternal => _isSmallImageKeyExternal;

        private bool _isSmallImageKeyExternal;
        
        /// <summary>
        /// The tooltip for the small circle image. For example, "LvL 6" or "Ultimate 85%".
        /// <para>Max 128 Bytes.</para>
        /// </summary>
        [JsonProperty("small_text", NullValueHandling = NullValueHandling.Ignore)]
        public string SmallImageText
        {
            get => _smallImageText;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _smallImageText, 128, Encoding.UTF8))
                    throw new StringOutOfRangeException(128);
            }
        }
        private string _smallImageText;

        /// <summary>
        /// The ID of the large image. This is only set after Update Presence and will automatically become null when <see cref="LargeImageKey"/> is changed.
        /// </summary>
        [JsonIgnore]
        public ulong? LargeImageId => _largeImageId;

        private ulong? _largeImageId;

        /// <summary>
        /// The ID of the small image. This is only set after Update Presence and will automatically become null when <see cref="SmallImageKey"/> is changed.
        /// </summary>
        [JsonIgnore]
        public ulong? SmallImageId => _smallImageId;

        private ulong? _smallImageId;

        /// <summary>
        /// Merges this asset with the other, taking into account for ID's instead of keys.
        /// </summary>
        /// <param name="other"></param>
        internal void Merge(Assets other)
        {
            // Copy over the names
            _smallImageText = other._smallImageText;
            _largeImageText = other._largeImageText;

            // Convert large ID
            if (ulong.TryParse(other._largeImageKey, out var largeId))
            {
                _largeImageId = largeId;
            }
            else
            {
                _largeImageKey = other._largeImageKey;
                _largeImageId = null;
            }

            // Convert the small ID
            if (ulong.TryParse(other._smallImageKey, out var smallId))
            {
                _smallImageId = smallId;
            }
            else
            {
                _smallImageKey = other._smallImageKey;
                _smallImageId = null;
            }
        }
    }
}