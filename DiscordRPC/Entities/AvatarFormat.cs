namespace DiscordRPC.Entities
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
        GIF
    }
}