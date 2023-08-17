﻿using System.Text.Json.Serialization;

namespace DiscordRPC.Message
{
    /// <summary>
    /// Called when some other person has requested access to this game. C -> D -> C.
    /// </summary>
    public class JoinRequestMessage : IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type { get { return MessageType.JoinRequest; } }

        /// <summary>
        /// The discord user that is requesting access.
        /// </summary>
        [JsonPropertyName("user")]
        public User User { get; internal set; }
    }
}
