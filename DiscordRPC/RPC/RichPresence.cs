using DiscordRPC.RPC.Types.RPC;
using Newtonsoft.Json;

namespace DiscordRPC.RPC
{
    /// <summary>
    /// The Rich Presence structure that will be sent and received by Discord. Use this class to build your presence and update it appropriately.
    /// </summary>
    // This is broken up in this way because the response inherits the BaseRichPresence.
    public sealed class RichPresence : BaseRichPresence
    {
        /// <summary>
        /// The buttons to display in the presence. 
        /// <para>Max of 2</para>
        /// </summary>
        [JsonProperty("buttons", NullValueHandling = NullValueHandling.Ignore)]
        public Button[] Buttons { get; set; }

        /// <summary>
        /// Does the Rich Presence have any buttons?
        /// </summary>
        /// <returns></returns>
        public bool HasButtons() => Buttons != null && Buttons.Length > 0;
        
        #region Builder
        /// <summary>
        /// Sets the state of the Rich Presence. See also <seealso cref="BaseRichPresence.State"/>.
        /// </summary>
        /// <param name="state">The user's current <see cref="Party"/> status.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithState(string state)
        {
            State = state;
            return this;
        }

        /// <summary>
        /// Sets the details of the Rich Presence. See also <seealso cref="BaseRichPresence.Details"/>.
        /// </summary>
        /// <param name="details">What the user is currently doing.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithDetails(string details)
        {
            Details = details;
            return this;
        }

        /// <summary>
        /// Sets the timestamp of the Rich Presence. See also <seealso cref="Timestamps"/>.
        /// </summary>
        /// <param name="timestamps">The time elapsed / remaining time data.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithTimestamps(Timestamps timestamps)
        {
            Timestamps = timestamps;
            return this;
        }

        /// <summary>
        /// Sets the assets of the Rich Presence. See also <seealso cref="Assets"/>.
        /// </summary>
        /// <param name="assets">The names of the images to use and the tooltips to give those images.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithAssets(Assets assets)
        {
            Assets = assets;
            return this;
        }

        /// <summary>
        /// Sets the Rich Presence's party. See also <seealso cref="Party"/>.
        /// </summary>
        /// <param name="party">The party the player is currently in.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithParty(Party party)
        {
            Party = party;
            return this;
        }

        /// <summary>
        /// Sets the Rich Presence's secrets. See also <seealso cref="Secrets"/>.
        /// </summary>
        /// <param name="secrets">The secrets used for Join / Spectate.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithSecrets(Secrets secrets)
        {
            Secrets = secrets;
            return this;
        }
        #endregion


        #region Cloning and Merging 

        /// <summary>
        /// Clones the presence into a new instance. Used for thread safe writing and reading. This function will ignore properties if they are in a invalid state.
        /// </summary>
        /// <returns></returns>
        public RichPresence Clone()
        {
            return new RichPresence
            {
                State = _state?.Clone() as string,
                Details = _details?.Clone() as string,
                
                Buttons = !HasButtons() ? null : Buttons.Clone() as Button[],
                Secrets = !HasSecrets() ? null : new Secrets
                {
                    // MatchSecret = this.Secrets.MatchSecret?.Clone() as string,
                    JoinSecret = Secrets.JoinSecret?.Clone() as string,
                    SpectateSecret = Secrets.SpectateSecret?.Clone() as string
                },

                Timestamps = !HasTimestamps() ? null : new Timestamps
                {
                    Start = Timestamps.Start,
                    End = Timestamps.End
                },

                Assets = !HasAssets() ? null : new Assets
                {
                    LargeImageKey = Assets.LargeImageKey?.Clone() as string,
                    LargeImageText = Assets.LargeImageText?.Clone() as string,
                    SmallImageKey = Assets.SmallImageKey?.Clone() as string,
                    SmallImageText = Assets.SmallImageText?.Clone() as string
                },

                Party = !HasParty() ? null : new Party
                {
                    Id = Party.Id,
                    Size = Party.Size,
                    Max = Party.Max,
                    Privacy = Party.Privacy,
                },

            };
        }

        /// <summary>
        /// Merges the passed presence with this one, taking into account the image key to image id annoyance.
        /// </summary>
        /// <param name="presence"></param>
        /// <returns>self</returns>
        internal RichPresence Merge(BaseRichPresence presence)
        {
            _state = presence.State;
            _details = presence.Details;
            Party = presence.Party;
            Timestamps = presence.Timestamps;
            Secrets = presence.Secrets;

            // If they have assets, we should merge them
            if (presence.HasAssets())
            {
                // Make sure we actually have assets too
                if (!HasAssets())
                {
                    // We dont, so we will just use theirs
                    Assets = presence.Assets;
                }
                else
                {
                    // We do, so we better merge them!
                    Assets.Merge(presence.Assets);
                }
            }
            else
            {
                // They dont have assets, so we will just set ours to null
                Assets = null;
            }

            return this;
        }

        internal override bool Matches(RichPresence other)
        {
            if (!base.Matches(other)) return false;

            // Check buttons
            if (Buttons == null ^ other.Buttons == null) return false;
            if (Buttons == null) return true;
            if (other.Buttons != null && Buttons.Length != other.Buttons.Length) return false;
            for (var i = 0; i < Buttons.Length; i++)
            {
                var a = Buttons[i];
                if (other.Buttons == null) continue;
                var b = other.Buttons[i];
                if (a.Label != b.Label || a.Url != b.Url) return false;
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Operator that converts a presence into a boolean for null checks.
        /// </summary>
        /// <param name="presence"></param>
        public static implicit operator bool(RichPresence presence) => presence != null;
    }
}