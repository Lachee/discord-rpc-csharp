using System;

namespace DiscordRPC.RPC.Events
{
    /// <summary>
    /// The type of event received by the RPC. A flag type that can be combined.
    /// </summary>
    [Flags]
    public enum EventType
    {
        /// <summary>
        /// No event
        /// </summary>
        None = 0,

        /// <summary>
        /// Called when the Discord Client wishes to enter a game to spectate
        /// </summary>
        Spectate = 0x1, 

        /// <summary>
        /// Called when the Discord Client wishes to enter a game to play.
        /// </summary>
        Join = 0x2,

        /// <summary>
        /// Called when another Discord Client has requested permission to join this game.
        /// </summary>
        JoinRequest = 0x4
    }
}