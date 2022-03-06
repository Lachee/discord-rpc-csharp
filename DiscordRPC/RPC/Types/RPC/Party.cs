using System;
using DiscordRPC.Core.Helpers;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Types.RPC
{
    /// <summary>
    /// Structure representing the part the player is in.
    /// </summary>
    [Serializable]
    public class Party
    {
        /// <summary>
        /// The current size of the players party / lobby / group.
        /// </summary>
        [JsonIgnore]
        public int Size { get; set; }

        /// <summary>
        /// The maximum size of the party / lobby / group. This is required to be larger than <see cref="Size"/>. If it is smaller than the current party size, it will automatically be set too <see cref="Size"/> when the presence is sent.
        /// </summary>
        [JsonIgnore]
        public int Max { get; set; }
        
        /// <summary>
        /// The privacy of the party
        /// </summary>
        [JsonProperty("privacy", NullValueHandling = NullValueHandling.Include, DefaultValueHandling = DefaultValueHandling.Include)]
        public PrivacySetting Privacy { get; set; }
        
        /// <summary>
        /// A unique ID for the player's current party / lobby / group. If this is not supplied, they player will not be in a party and the rest of the information will not be sent. 
        /// <para>Max 128 Bytes</para>
        /// </summary>
        [JsonProperty("id", NullValueHandling = NullValueHandling.Ignore)]
        public string Id
        { 
            get => _partyId;
            set => _partyId = value.GetNullOrString();
        }
        private string _partyId;

        [JsonProperty("size", NullValueHandling = NullValueHandling.Ignore)]
        private int[] _size
        {
            get
            {
                // see issue https://github.com/discordapp/discord-rpc/issues/111
                var size = Math.Max(1, Size);
                return new[] { size, Math.Max(size, Max) };
            }

            set
            {
                if (value.Length != 2)
                {
                    Size = 0;
                    Max = 0;
                }
                else
                {
                    Size = value[0];
                    Max = value[1];
                }
            }

        }
    }
}