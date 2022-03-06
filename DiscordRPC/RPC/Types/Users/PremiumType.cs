namespace DiscordRPC.RPC.Types.Users
{
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
}