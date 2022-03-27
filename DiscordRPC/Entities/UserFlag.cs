namespace DiscordRPC.Entities
{
    /// <summary>
    /// A flag on the user account
    /// </summary>
    public enum UserFlag
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

        // These 2 are mystery types
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
}