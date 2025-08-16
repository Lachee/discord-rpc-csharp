using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;

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
		/// Data for the avatar decoration which can be layered on top of the avatar.
		/// </summary>
		public struct AvatarDecorationData
		{
			/// <summary>
			/// The hash of the asset used for the decoration.
			/// </summary>
			[JsonProperty("asset")]
			public string Asset { get; private set; }
			/// <summary>
			/// The SKU of the decoration.
			/// </summary>
			[JsonProperty("skuId")]
			public string SKU { get; private set; }
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
		/// The hash of the user's avatar.
		/// To get a URL for the avatar, use the <see cref="GetAvatarURL(AvatarFormat, AvatarSize)"/>. 
		/// </summary>
		/// <remarks>
		/// If the user has a default Discord avatar, this value will be <c>null</c>. <see cref="GetAvatarURL(AvatarFormat, AvatarSize)"/> will still return the correct default avatar.
		/// </remarks>
		[JsonProperty("avatar")]
		public string Avatar { get; private set; }

		/// <summary>
		/// The avatar is animated
		/// </summary>
		public bool IsAvatarAnimated => Avatar != null && Avatar.StartsWith("a_");

		/// <summary>
		/// The SKU and hash of the users avatar decoration. 
		/// To get a URL for the decoration, use the <see cref="GetAvatarDecorationURL()"/>.
		/// </summary>
		[JsonProperty("avatar_decoration_data")]
		public AvatarDecorationData? AvatarDecoration { get; private set; }

		/// <summary>
		/// Whether the user belongs to an OAuth2 application.
		/// </summary>
		[JsonProperty("bot")]
		public bool Bot { get; private set; }

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

			/// <summary>Discord Employee</summary>
			Employee = 1 << 0,

			/// <summary>Partnered Server Owner</summary>
			Partner = 1 << 1,

			/// <summary>HypeSquad Events Member</summary>
			HypeSquad = 1 << 2,

			/// <summary>Bug Hunter Level 1</summary>
			BugHunter = 1 << 3,

			// Unknown Flags:
			// 1 << 4,
			// 1 << 5,

			/// <summary>House Bravery Member</summary>
			HouseBravery = 1 << 6,

			/// <summary>House Brilliance Member</summary>
			HouseBrilliance = 1 << 7,

			/// <summary>House Balance Member</summary>
			HouseBalance = 1 << 8,

			/// <summary>Early Nitro Supporter</summary>
			EarlySupporter = 1 << 9,

			/// <summary>User is a team</summary>
			TeamUser = 1 << 10,

			// Unknown Flags:
			// 1 << 11
			// 1 << 12
			// 1 << 13

			/// <summary>Bug Hunter Level 2</summary>
			BugHunterLevel2 = 1 << 14,

			// Unknown Flag:
			// 1 << 15

			/// <summary>Verified Bot</summary>
			VerifiedBot = 1 << 16,

			/// <summary>Early Verified Bot Developer</summary>
			VerifiedDeveloper = 1 << 17,

			/// <summary>Moderator Programs Alumni</summary>
			CertifiedModerator = 1 << 18,

			/// <summary>Bot uses only HTTP interactions and is shown in the online member list</summary>
			BotHttpInteractions = 1 << 19,

			// Unknown Flags:
			// 1 << 20
			// 1 << 21

			/// <summary>User is an Active Developer</summary>
			ActiveDeveloper = 1 << 22
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
			/// <summary>No subscription</summary>
			None = 0,

			/// <summary>Nitro Classic. The precursor to the <see cref="NitroBasic"/></summary>
			NitroClassic = 1,

			/// <summary>Nitro. Access to all premium features</summary>
			Nitro = 2,

			/// <summary>Nitro Basic. Access to mostpremium features</summary>
			NitroBasic = 3
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
		/// Gets a URL to the user's avatar PNG.
		/// <para>If the user does not have an avatar, a URL to a default avatar is returned.</para>
		/// </summary>
		/// <remarks>
		/// The file returned will be <c>128x128px</c>.
		/// </remarks>
		/// <returns>URL to the discord CDN for the particular avatar</returns>
		public string GetAvatarURL()
			=> GetAvatarURL(AvatarFormat.PNG, AvatarSize.x128);

		/// <summary>
		/// Gets a URL to the user's avatar with the specified format.
		/// <para>If the user does not have an avatar, a URL to a default avatar is returned.</para>
		/// </summary>
		/// <remarks>
		/// <para>The file returned will be <c>128x128px</c>.</para>
		/// <para>The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.</para>
		/// </remarks>
		/// <param name="format">The format of the target avatar</param>
		/// <returns>URL to the discord CDN for the particular avatar</returns>
		/// <exception cref="BadImageFormatException">The user has a default avatar but a format other than PNG is requested.</exception>
		public string GetAvatarURL(AvatarFormat format)
			=> GetAvatarURL(format, AvatarSize.x128);

		/// <summary>
		/// Gets a URL to the user's avatar with the specified format and size.
		/// <para>If the user does not have an avatar, a URL to a default avatar is returned.</para>
		/// </summary>
		/// <remarks>
		/// The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.
		/// </remarks>
		/// <param name="format">The format of the target avatar</param>
		/// <param name="size">The optional size of the avatar you wish for.</param>
		/// <returns>URL to the discord CDN for the particular avatar</returns>
		/// <exception cref="BadImageFormatException">The user has a default avatar but a format other than PNG is requested.</exception>
		public string GetAvatarURL(AvatarFormat format, AvatarSize size)
		{
			//Prepare the endpoint
			string endpoint = $"/avatars/{ID}/{Avatar}";

			//The user has no avatar, so we better replace it with the default
			if (string.IsNullOrEmpty(Avatar))
			{
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
			return string.Format("https://{0}{1}{2}?size={3}&animated=true", this.CdnEndpoint, endpoint, GetAvatarExtension(format), (int)size);
		}

		/// <summary>
		/// Gets a URL to the user's avatar decoration.
		/// </summary>
		/// <remarks>The Image will be a Animated PNG.</remarks>
		/// <returns>URL to the discord CDN for the current avatar decoration, otherwise <c>null</c>.</returns>
		public string GetAvatarDecorationURL()
			=> GetAvatarDecorationURL(AvatarFormat.PNG);

		/// <summary>		
		/// Gets a URL to the user's avatar decoration.
		/// </summary>
		/// <remarks>
		/// The avatar formats do not behave like User Avatars:
		/// <list type="bullet">
		/// <item><see cref="AvatarFormat.PNG"/> are Animated PNG</item>
		/// <item><see cref="AvatarFormat.GIF"/> will respond with <c>415 Unsupported Media Type</c></item>
		/// <item><see cref="AvatarFormat.WebP"/> are not animated</item>
		/// <item><see cref="AvatarFormat.JPEG"/> do not have transparency</item>
		/// </list>
		/// 
		/// Additionally, size is not support and makes no difference to the resulting image.
		/// </remarks>
		/// <param name="format">The format of the decoration</param>
		/// <returns>URL to the discord CDN for the current avatar decoration, otherwise <c>null</c>.</returns>
		public string GetAvatarDecorationURL(AvatarFormat format)
		{
			if (AvatarDecoration == null)
				return null;

			string endpoint = $"/avatar-decoration-presets/{AvatarDecoration.Value.Asset}";
			return string.Format("https://{0}{1}{2}", this.CdnEndpoint, endpoint, GetAvatarExtension(format));
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
