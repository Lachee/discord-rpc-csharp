using System;
using DiscordRPC.Core;
using Newtonsoft.Json;

namespace DiscordRPC.Entities
{
    /// <summary>
    /// Object representing a Discord user. This is used for join requests.
    /// </summary>
    public class User
    {
        /// <summary>
        /// The snowflake ID of the user. 
        /// </summary>
        [JsonProperty("id")]
        public ulong Id { get; private set; }

        /// <summary>
        /// The username of the player.
        /// </summary>
        [JsonProperty("username")]
        public string Username { get; private set; }

        /// <summary>
        /// The discriminator of the user.
        /// </summary>
        [JsonProperty("discriminator")]
        public int Discriminator { get; private set; }
        
        /// <summary>
        /// The avatar hash of the user. Too get a URL for the avatar, use the <see cref="GetAvatarUrl(AvatarFormat, AvatarSize)"/>. This can be null if the user has no avatar. The <see cref="GetAvatarUrl"/> will account for this and return the discord default.
        /// </summary>
        [JsonProperty("avatar")]
        public string Avatar { get; private set; }

        /// <summary>
        /// The flags on a users account, often represented as a badge.
        /// </summary>
        [JsonProperty("flags")]
        public UserFlag Flags { get; private set; }
        
        /// <summary>
        /// The premium type of the user.
        /// </summary>
        [JsonProperty("premium_type")]
        public PremiumType Premium { get; private set; }

        /// <summary>
        /// The endpoint for the CDN. Normally cdn.discordapp.com.
        /// </summary>
        private string CdnEndpoint { get; set; }
        
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
            CdnEndpoint = configuration.CdnHost;
        }
        
        /// <summary>
        /// Gets a URL that can be used to download the user's avatar. If the user has not yet set their avatar, it will return the default one that discord is using. The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.
        /// </summary>
        /// <param name="format">The format of the target avatar</param>
        /// <param name="size">The optional size of the avatar you wish for. Defaults to x128.</param>
        /// <returns></returns>
        public string GetAvatarUrl(AvatarFormat format, AvatarSize size = AvatarSize.x128)
        {
            // Prepare the endpoint
            var endpoint = $"/avatars/{Id}/{Avatar}";

            // The user has no avatar, so we better replace it with the default
            if (!string.IsNullOrEmpty(Avatar)) return $"https://{CdnEndpoint}{endpoint}{GetAvatarExtension(format)}?size={(int) size}";
            
            // Make sure we are only using PNG
            if (format != AvatarFormat.PNG)
                throw new BadImageFormatException($"The user has no avatar and the requested format {format} is not supported. (Only supports PNG).");

            // Get the endpoint
            var discriminator = Discriminator % 5;
            endpoint = $"/embed/avatars/{discriminator}";

            // Finish of the endpoint
            return $"https://{CdnEndpoint}{endpoint}{GetAvatarExtension(format)}?size={(int) size}";
        }

        /// <summary>
        /// Returns the file extension of the specified format.
        /// </summary>
        /// <param name="format">The format to get the extension off</param>
        /// <returns>Returns a period prefixed file extension.</returns>
        private static string GetAvatarExtension(AvatarFormat format) => $".{format.ToString().ToLowerInvariant()}";

        /// <summary>
        /// Formats the user into username#discriminator
        /// </summary>
        /// <returns></returns>
        public override string ToString() => $"{Username}#{Discriminator:D4}";
    }
}