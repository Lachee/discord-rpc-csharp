using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// Object representing a Discord user. This is used for join requests.
	/// </summary>
	public class User
	{
		/// <summary>
		/// Possible formats for avatars
		/// </summary>
		public enum AvatarFormat
		{
			/// <summary>
			/// Portable Network Graphics format (.png)
			/// <para>Losses format that supports transparent avatars. Most recommended for stationary formats with wide support from many libraries.</para>
			/// </summary>
			PNG,

			/// <summary>
			/// Joint Photographic Experts Group format (.jpeg)
			/// <para>The format most cameras use. Lossy and does not support transparent avatars.</para>
			/// </summary>
			JPEG,

			/// <summary>
			/// WebP format (.webp)
			/// <para>Picture only version of WebM. Pronounced "weeb p".</para>
			/// </summary>
			WebP,

            /// <summary>
            /// Graphics Interchange Format (.gif)
            /// <para>Animated avatars that Discord Nitro users are able to use. If the user doesn't have an animated avatar, then it will just be a single frame gif.</para>
            /// </summary>
            GIF                 //Gif, as in gift. 
        }

		/// <summary>
		/// Possible square sizes of avatars.
		/// </summary>
		public enum AvatarSize
		{
			/// <summary> 16 x 16 pixels.</summary>
			x16 = 16,
			/// <summary> 32 x 32 pixels.</summary>
			x32 = 32,
			/// <summary> 64 x 64 pixels.</summary>
			x64 = 64,
			/// <summary> 128 x 128 pixels.</summary>
			x128 = 128,
			/// <summary> 256 x 256 pixels.</summary>
			x256 = 256,
			/// <summary> 512 x 512 pixels.</summary>
			x512 = 512,
			/// <summary> 1024 x 1024 pixels.</summary>
			x1024 = 1024,
			/// <summary> 2048 x 2048 pixels.</summary>
			x2048 = 2048
		}

		/// <summary>
		/// The snowflake ID of the user. 
		/// </summary>
		[JsonProperty("id")]
		public ulong ID { get; private set; }

		/// <summary>
		/// The username of the player.
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; private set; }

		/// <summary>
		/// The discriminator of the user.
		/// </summary>
		/// <remarks>If the user has migrated to unique a <see cref="Username"/>, the discriminator will always be 0.</remarks>
		[JsonProperty("discriminator"), Obsolete("Discord no longer uses discriminators.")]
		public int Discriminator { get; private set; }

		/// <summary>
		/// The display name of the user
		/// </summary>
		/// <remarks>This will be empty if the user has not set a global display name.</remarks>
		[JsonProperty("global_name")]
		public string DisplayName { get; private set; }

		/// <summary>
		/// The avatar hash of the user. Too get a URL for the avatar, use the <see cref="GetAvatarURL(AvatarFormat, AvatarSize)"/>. This can be null if the user has no avatar. The <see cref="GetAvatarURL(AvatarFormat, AvatarSize)"/> will account for this and return the discord default.
		/// </summary>
		[JsonProperty("avatar")]
		public string Avatar { get; private set; }

        /// <summary>
        /// The flags on a users account, often represented as a badge.
        /// </summary>
        [JsonProperty("flags", NullValueHandling = NullValueHandling.Ignore)]
        public Flag Flags { get; private set; }

        /// <summary>
        /// A flag on the user account
        /// </summary>
        [Flags]
        public enum Flag
        {
            /// <summary>No flag</summary>
            None = 0,

            /// <summary>Staff of Discord.</summary>
            Employee = 1 << 0,

            /// <summary>Partners of Discord.</summary>
            Partner = 1 << 1,

            /// <summary>Original HypeSquad which organise events.</summary>
            HypeSquad = 1 << 2,

            /// <summary>Bug Hunters that found and reported bugs in Discord.</summary>
            BugHunter = 1 << 3,

            //These 2 are mistery types
            //A = 1 << 4,
            //B = 1 << 5,

            /// <summary>The HypeSquad House of Bravery.</summary>
            HouseBravery = 1 << 6,

            /// <summary>The HypeSquad House of Brilliance.</summary>
            HouseBrilliance = 1 << 7,

            /// <summary>The HypeSquad House of Balance (the best one).</summary>
            HouseBalance = 1 << 8,

            /// <summary>Early Supporter of Discord and had Nitro before the store was released.</summary>
            EarlySupporter = 1 << 9,

            /// <summary>Apart of a team.
            /// <para>Unclear if it is reserved for members that share a team with the current application.</para>
            /// </summary>
            TeamUser = 1 << 10
        }

        /// <summary>
        /// The premium type of the user.
        /// </summary>
        [JsonProperty("premium_type", NullValueHandling = NullValueHandling.Ignore)]
        public PremiumType Premium { get; private set; }

        /// <summary>
        /// Type of premium
        /// </summary>
        public enum PremiumType
        {
            /// <summary>No subscription to any forms of Nitro.</summary>
            None = 0,

            /// <summary>Nitro Classic subscription. Has chat perks and animated avatars.</summary>
            NitroClassic = 1,

            /// <summary>Nitro subscription. Has chat perks, animated avatars, server boosting, and access to free Nitro Games.</summary>
            Nitro = 2
        }

		/// <summary>
		/// The endpoint for the CDN. Normally cdn.discordapp.com.
		/// </summary>
		public string CdnEndpoint { get; private set; }

        /// <summary>
        /// Creates a new User instance.
        /// </summary>
        internal User()
        {
            CdnEndpoint = "cdn.discordapp.com";
        }

		/// <summary>
		/// Updates the URL paths to the appropriate configuration
		/// </summary>
		/// <param name="configuration">The configuration received by the OnReady event.</param>
		internal void SetConfiguration(Configuration configuration)
		{
			this.CdnEndpoint = configuration.CdnHost;
		}


		/// <summary>
		/// Gets a URL that can be used to download the user's avatar. If the user has not yet set their avatar, it will return the default one that discord is using. The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.
		/// </summary>
		/// <remarks>The file returned will be 128px x 128px</remarks>
		/// <param name="format">The format of the target avatar</param>
		/// <returns>URL to the discord CDN for the particular avatar</returns>
		public string GetAvatarURL(AvatarFormat format)
			=> GetAvatarURL(format, AvatarSize.x128);

		/// <summary>
		/// Gets a URL that can be used to download the user's avatar. If the user has not yet set their avatar, it will return the default one that discord is using. The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.
		/// </summary>
		/// <param name="format">The format of the target avatar</param>
		/// <param name="size">The optional size of the avatar you wish for.</param>
		/// <returns>URL to the discord CDN for the particular avatar</returns>
		public string GetAvatarURL(AvatarFormat format, AvatarSize size)
		{
			//Prepare the endpoint
			string endpoint = $"/avatars/{ID}/{Avatar}";

			//The user has no avatar, so we better replace it with the default
			if (string.IsNullOrEmpty(Avatar))
			{
				//Make sure we are only using PNG
				if (format != AvatarFormat.PNG)
					throw new BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");

				// Get the default avatar for the user.
				int index = (int)((ID >> 22) % 6);

#pragma warning disable CS0618 // Disable the obsolete warning as we know the discriminator is obsolete and we are validating it here.
                if (Discriminator > 0)
				    index = Discriminator % 5;
#pragma warning restore CS0618

                endpoint = $"/embed/avatars/{index}";
			}

			//Finish of the endpoint
			return string.Format("https://{0}{1}{2}?size={3}", this.CdnEndpoint, endpoint, GetAvatarExtension(format), (int)size);
		}

		/// <summary>
		/// Returns the file extension of the specified format.
		/// </summary>
		/// <param name="format">The format to get the extention off</param>
		/// <returns>Returns a period prefixed file extension.</returns>
		public string GetAvatarExtension(AvatarFormat format)
		{
			return "." + format.ToString().ToLowerInvariant();
		}

		/// <summary>
		/// Formats the user into a displayable format. If the user has a <see cref="DisplayName"/>, then this will be used.
		/// <para>If the user still has a discriminator, then this will return the form of `Username#Discriminator`.</para>
		/// </summary>
		/// <returns>String of the user that can be used for display.</returns>
		public override string ToString()
		{
			if (!string.IsNullOrEmpty(DisplayName))
				return DisplayName;

#pragma warning disable CS0618 // Disable the obsolete warning as we know the discriminator is obsolete and we are validating it here.
			if (Discriminator != 0)
				return Username + "#" + Discriminator.ToString("D4");
#pragma warning restore CS0618

            return Username;
		}
	}
}
