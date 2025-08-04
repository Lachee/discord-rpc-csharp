using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
    /// <summary>
    /// The base rich presence structure
    /// </summary>
    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    public class BaseRichPresence
    {
        /// <summary>
        /// The user's current <see cref="Party"/> status. For example, "Playing Solo" or "With Friends".
        /// <para>Max 128 characters</para>
        /// </summary>
        [JsonProperty("state", NullValueHandling = NullValueHandling.Ignore)]
        public string State
        {
            get { return _state; }
            set
            {
                if (!ValidateString(value, out _state, false, 128))
                    throw new StringOutOfRangeException("State", 0, 128);
            }
        }

        /// <summary>Inernal inner state string</summary>
        protected internal string _state;
        
        /// <summary>
        /// URL that is linked to when clicking on the details text in the activity card.
        /// <para>Max 256 characters</para>
        /// </summary>
        [JsonProperty("state_url", NullValueHandling = NullValueHandling.Ignore)]
        public string StateUrl
        {
            get { return _stateUrl; }
            set
            {
                if (!ValidateString(value, out _stateUrl, false, 256))
                    throw new StringOutOfRangeException(256);

                if (!ValidateUrl(_stateUrl))
                    throw new ArgumentException("Url must be a valid URI");
            }
        }
        /// <summary>Inernal inner state URL string</summary>
        protected internal string _stateUrl;

        /// <summary>
        /// What the user is currently doing. For example, "Competitive - Total Mayhem".
        /// <para>Max 128 characters</para>
        /// </summary>
        [JsonProperty("details", NullValueHandling = NullValueHandling.Ignore)]
        public string Details
        {
            get { return _details; }
            set
            {
                if (!ValidateString(value, out _details, false, 128))
                    throw new StringOutOfRangeException(128);
            }
        }
        /// <summary>Inernal inner detail string</summary>
        protected internal string _details;
        
        /// <summary>
        /// URL that is linked to when clicking on the details text in the activity card.
        /// <para>Max 256 characters</para>
        /// </summary>
        [JsonProperty("details_url", NullValueHandling = NullValueHandling.Ignore)]
        public string DetailsUrl
        {
            get { return _detailsUrl; }
            set
            {
                if (!ValidateString(value, out _detailsUrl, false, 256))
                    throw new StringOutOfRangeException(256);

                if (!ValidateUrl(_detailsUrl))
                    throw new ArgumentException("Url must be a valid URI");
            }
        }
        /// <summary>Inernal inner detail URL string</summary>
        protected internal string _detailsUrl;

        /// <summary>
        /// The time elapsed / remaining time data.
        /// </summary>
        [JsonProperty("timestamps", NullValueHandling = NullValueHandling.Ignore)]
        public Timestamps Timestamps { get; set; }

        /// <summary>
        /// The names of the images to use and the tooltips to give those images.
        /// </summary>
        [JsonProperty("assets", NullValueHandling = NullValueHandling.Ignore)]
        public Assets Assets { get; set; }

        /// <summary>
        /// The party the player is currently in. The <see cref="Party.ID"/> must be set for this to be included in the RichPresence update.
        /// </summary>
        [JsonProperty("party", NullValueHandling = NullValueHandling.Ignore)]
        public Party Party { get; set; }

        /// <summary>
        /// The secrets used for Join / Spectate. Secrets are obfuscated data of your choosing. They could be match ids, player ids, lobby ids, etc. Make this object null if you do not wish too / unable too implement the Join / Request feature.
        /// <para>To keep security on the up and up, Discord requires that you properly hash/encode/encrypt/put-a-padlock-on-and-swallow-the-key-but-wait-then-how-would-you-open-it your secrets.</para>
        /// <para>Visit the <see href="https://discordapp.com/developers/docs/rich-presence/how-to#secrets">Rich Presence How-To</see> for more information.</para>
        /// </summary>
        [JsonProperty("secrets", NullValueHandling = NullValueHandling.Ignore)]
        public Secrets Secrets { get; set; }

        /// <summary>
        /// The activity type
        /// </summary>
        [JsonProperty("type", NullValueHandling = NullValueHandling.Ignore)]
        public ActivityType Type { get; set; }
        
        /// <summary>
        /// The display type for the status
        /// </summary>
        [JsonProperty("status_display_type", NullValueHandling = NullValueHandling.Ignore)]
        public StatusDisplayType StatusDisplay { get; set; }


        /// <summary>
        /// Marks the <see cref="Secrets.MatchSecret"/> as a game session with a specific beginning and end. It was going to be used as a form of notification, but was replaced with the join feature. It may potentially have use in the future, but it currently has no use.
        /// <para>
        /// "TLDR it marks the matchSecret field as an instance, that is to say a context in game that’s not like a lobby state/not in game state. It was gonna he used for notify me, but we scrapped that for ask to join. We may put it to another use in the future. For now, don’t worry about it" - Mason (Discord API Server 14 / 03 / 2018)
        ///    </para>
        /// </summary>
        [Obsolete("This was going to be used, but was replaced by JoinSecret instead", true)]
        private bool Instance;

        #region Has Checks
        /// <summary>
        /// Does the Rich Presence have valid timestamps?
        /// </summary>
        /// <returns></returns>
        public bool HasTimestamps()
        {
            return this.Timestamps != null && (Timestamps.Start != null || Timestamps.End != null);
        }

        /// <summary>
        /// Does the Rich Presence have valid assets?
        /// </summary>
        /// <returns></returns>
        public bool HasAssets()
        {
            return this.Assets != null;
        }

        /// <summary>
        /// Does the Rich Presence have a valid party?
        /// </summary>
        /// <returns></returns>
        public bool HasParty()
        {
            return this.Party != null && this.Party.ID != null;
        }

        /// <summary>
        /// Does the Rich Presence have valid secrets?
        /// </summary>
        /// <returns></returns>
        public bool HasSecrets()
        {
            return Secrets != null && (Secrets.JoinSecret != null || Secrets.SpectateSecret != null);
        }

        #endregion


        /// <summary>
        /// Attempts to call <see cref="StringTools.GetNullOrString(string)"/> on the string and return the result, if its within a valid length.
        /// </summary>
        /// <param name="str">The string to check</param>
        /// <param name="result">The formatted string result</param>
        /// <param name="useBytes">True if you need to validate the string by byte length</param>
        /// <param name="length">The maximum number of bytes/characters the string can take up</param>
        /// <param name="encoding">The encoding to count the bytes with, optional</param>
        /// <returns>True if the string fits within the number of bytes</returns>
        internal static bool ValidateString(string str, out string result, bool useBytes, int length, Encoding encoding = null)
        {
            result = str;
            if (str == null)
                return true;

            //Trim the string, for the best chance of fitting
            var s = str.Trim();

            //Make sure it fits
            if (useBytes && !s.WithinLength(length, encoding) || s.Length > length)
                return false;

            //Make sure its not empty
            result = s.GetNullOrString();
            return true;
        }
        
        /// <summary>
        /// Validates URLs.
        /// </summary>
        /// <param name="url">The URL to check</param>
        /// <returns>True if the URL is valid</returns>
        internal static bool ValidateUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return true;

            //Check if the URL is valid
            if (!Uri.TryCreate(url, UriKind.Absolute, out _))
                return false;

            return true;
        }

        /// <summary>
        /// Operator that converts a presence into a boolean for null checks.
        /// </summary>
        /// <param name="presence"></param>
        public static implicit operator bool(BaseRichPresence presence)
        {
            return presence != null;
        }

        /// <summary>
        /// Checks if the other rich presence differs from the current one
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        internal virtual bool Matches(RichPresence other)
        {
            if (other == null)
                return false;

            if (State != other.State ||
                StateUrl != other.StateUrl ||
                Details != other.Details ||
                DetailsUrl != other.DetailsUrl ||
                Type != other.Type)
                return false;

            //Checks if the timestamps are different
            if (Timestamps != null)
            {
                if (other.Timestamps == null ||
                    other.Timestamps.StartUnixMilliseconds != Timestamps.StartUnixMilliseconds ||
                    other.Timestamps.EndUnixMilliseconds != Timestamps.EndUnixMilliseconds)
                    return false;
            }
            else if (other.Timestamps != null)
            {
                return false;
            }

            //Checks if the secrets are different
            if (Secrets != null)
            {
                if (other.Secrets == null ||
                    other.Secrets.JoinSecret != Secrets.JoinSecret ||
                    other.Secrets.SpectateSecret != Secrets.SpectateSecret)
                    return false;
            }
            else if (other.Secrets != null)
            {
                return false;
            }

            //Checks if the timestamps are different
            if (Party != null)
            {
                if (other.Party == null ||
                    other.Party.ID != Party.ID ||
                    other.Party.Max != Party.Max ||
                    other.Party.Size != Party.Size ||
                    other.Party.Privacy != Party.Privacy)
                    return false;
            }
            else if (other.Party != null)
            {
                return false;
            }

            //Checks if the assets are different
            if (Assets != null)
            {
                if (other.Assets == null ||
                    other.Assets.LargeImageKey != Assets.LargeImageKey ||
                    other.Assets.LargeImageText != Assets.LargeImageText ||
                    other.Assets.LargeImageUrl != Assets.LargeImageUrl ||
                    other.Assets.SmallImageKey != Assets.SmallImageKey ||
                    other.Assets.SmallImageText != Assets.SmallImageText ||
                    other.Assets.SmallImageUrl != Assets.SmallImageUrl)
                    return false;
            }
            else if (other.Assets != null)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Converts this BaseRichPresence to RichPresence
        /// </summary>
        /// <returns></returns>
        public RichPresence ToRichPresence()
        {
            var presence = new RichPresence();
            presence.State = State;
            presence.StateUrl = StateUrl;
            presence.Details = Details;
            presence.DetailsUrl = DetailsUrl;
            presence.Type = Type;
            presence.StatusDisplay = StatusDisplay;

            presence.Party = !HasParty() ? Party : null;
            presence.Secrets = !HasSecrets() ? Secrets : null;

            if (HasAssets())
            {
                presence.Assets = new Assets()
                {
                    SmallImageKey = Assets.SmallImageKey,
                    SmallImageText = Assets.SmallImageText,
                    SmallImageUrl = Assets.SmallImageUrl,

                    LargeImageKey = Assets.LargeImageKey,
                    LargeImageText = Assets.LargeImageText,
                    LargeImageUrl = Assets.LargeImageUrl
                };
            }

            if (HasTimestamps())
            {
                presence.Timestamps = new Timestamps();
                if (Timestamps.Start.HasValue) presence.Timestamps.Start = Timestamps.Start;
                if (Timestamps.End.HasValue) presence.Timestamps.End = Timestamps.End;
            }

            return presence;
        }
    }

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
        public bool HasButtons()
        {
            return Buttons != null && Buttons.Length > 0;
        }


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
        /// Sets the state URL of the Rich Presence. See also <seealso cref="BaseRichPresence.StateUrl"/>.
        /// </summary>
        /// <param name="stateUrl">State URL when clicking on the state text.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithStateUrl(string stateUrl)
        {
            StateUrl = stateUrl;
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
        /// Sets the details URL of the Rich Presence. See also <seealso cref="BaseRichPresence.DetailsUrl"/>.
        /// </summary>
        /// <param name="detailsUrl">Details URL when clicking on the details text.</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithDetailsUrl(string detailsUrl)
        {
            DetailsUrl = detailsUrl;
            return this;
        }

        /// <summary>
        /// Sets the type of the Rich Presence. See also <seealso cref="ActivityType"/>.
        /// </summary>
        /// <param name="type">The status type</param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithType(ActivityType type)
        {
            Type = type;
            return this;
        }
        /// <summary>
        /// Sets the display type for the status. See also <seealso cref="StatusDisplayType"/>.
        /// </summary>
        /// <param name="statusDisplay"></param>
        /// <returns>The modified Rich Presence.</returns>
        public RichPresence WithStatusDisplay(StatusDisplayType statusDisplay)
        {
            StatusDisplay = statusDisplay;
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

		/// <summary>
		/// Sets the Rich Presence's buttons.
		/// </summary>
		/// <param name="topButton">The top button to display</param>
		/// <param name="bottomButton">The optional bottom button to display</param>
		/// <returns>The modified Rich Presence.</returns>
		public RichPresence WithButtons(Button topButton, Button bottomButton = null) 
        {
            if (topButton != null && bottomButton != null)
            {
                Buttons = new Button[] { topButton, bottomButton };
            }
            else if (topButton == null && bottomButton == null)
            {
                Buttons = default;
            } 
            else 
            { 
                Buttons = new Button[] { topButton ?? bottomButton };
            }

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
                State = this._state != null ? _state.Clone() as string : null,
                StateUrl = this._stateUrl != null ? _stateUrl.Clone() as string : null,
                Details = this._details != null ? _details.Clone() as string : null,
                DetailsUrl = this._detailsUrl != null ? _detailsUrl.Clone() as string : null,
                Type = this.Type,
                StatusDisplay = this.StatusDisplay,

                Buttons = !HasButtons() ? null : this.Buttons.Clone() as Button[],
                Secrets = !HasSecrets() ? null : new Secrets
                {
                    //MatchSecret = this.Secrets.MatchSecret?.Clone() as string,
                    JoinSecret = this.Secrets.JoinSecret != null ? this.Secrets.JoinSecret.Clone() as string : null,
                    SpectateSecret = this.Secrets.SpectateSecret != null ? this.Secrets.SpectateSecret.Clone() as string : null
                },

                Timestamps = !HasTimestamps() ? null : new Timestamps
                {
                    Start = this.Timestamps.Start,
                    End = this.Timestamps.End
                },

                Assets = !HasAssets() ? null : new Assets
                {
                    LargeImageKey = this.Assets.LargeImageKey != null ? this.Assets.LargeImageKey.Clone() as string : null,
                    LargeImageText = this.Assets.LargeImageText != null ? this.Assets.LargeImageText.Clone() as string : null,
                    LargeImageUrl = this.Assets.LargeImageUrl != null ? this.Assets.LargeImageUrl.Clone() as string : null,
                    SmallImageKey = this.Assets.SmallImageKey != null ? this.Assets.SmallImageKey.Clone() as string : null,
                    SmallImageText = this.Assets.SmallImageText != null ? this.Assets.SmallImageText.Clone() as string : null,
                    SmallImageUrl = this.Assets.SmallImageUrl != null ? this.Assets.SmallImageUrl.Clone() as string : null,
                },

                Party = !HasParty() ? null : new Party
                {
                    ID = this.Party.ID,
                    Size = this.Party.Size,
                    Max = this.Party.Max,
                    Privacy = this.Party.Privacy,
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
            this._state = presence.State;
            this._stateUrl = presence.StateUrl;
            this._details = presence.Details;
            this._detailsUrl = presence.DetailsUrl;
            this.Type = presence.Type;
            this.StatusDisplay = presence.StatusDisplay;
            this.Party = presence.Party;
            this.Timestamps = presence.Timestamps;
            this.Secrets = presence.Secrets;

            //If they have assets, we should merge them
            if (presence.HasAssets())
            {
                //Make sure we actually have assets too
                if (!this.HasAssets())
                {
                    //We dont, so we will just use theirs
                    this.Assets = presence.Assets;
                }
                else
                {
                    //We do, so we better merge them!
                    this.Assets.Merge(presence.Assets);
                }
            }
            else
            {
                //They dont have assets, so we will just set ours to null
                this.Assets = null;
            }

            return this;
        }

        internal override bool Matches(RichPresence other)
        {
            if (!base.Matches(other)) return false;

            //Check buttons
            if (Buttons == null ^ other.Buttons == null) return false;
            if (Buttons != null)
            {
                if (Buttons.Length != other.Buttons.Length) return false;
                for (int i = 0; i < Buttons.Length; i++)
                {
                    var a = Buttons[i];
                    var b = other.Buttons[i];
                    if (a.Label != b.Label || a.Url != b.Url)
                        return false;
                }
            }

            return true;
        }

        #endregion

        /// <summary>
        /// Operator that converts a presence into a boolean for null checks.
        /// </summary>
        /// <param name="presesnce"></param>
        public static implicit operator bool(RichPresence presesnce)
        {
            return presesnce != null;
        }
    }

    /// <summary>
    /// A rich presence that has been parsed from the pipe as a response.
    /// </summary>
    internal sealed class RichPresenceResponse : BaseRichPresence
    {
        /// <summary>
        /// ID of the client
        /// </summary>
        [JsonProperty("application_id")]
        public string ClientID { get; private set; }

        /// <summary>
        /// Name of the bot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

    }
}
