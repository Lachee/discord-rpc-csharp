using DiscordRPC.Entities;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Messaging.Messages
{
    /// <summary>
    /// Called when some other person has requested access to this game. C -> D -> C.
    /// </summary>
    public class JoinRequestMessage : IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type => MessageType.JoinRequest;

        /// <summary>
        /// The discord user that is requesting access.
        /// </summary>
        [JsonProperty("user")]
        public User User { get; internal set; }
    }
}