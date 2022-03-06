using DiscordRPC.RPC.Types.RPC;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Messaging.Messages
{
    /// <summary>
    /// Called when the Discord Client wishes for this process to join a game. D -> C.
    /// </summary>
    public class JoinMessage : IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type => MessageType.Join;

        /// <summary>
        /// The <see cref="Secrets.JoinSecret" /> to connect with. 
        /// </summary>
        [JsonProperty("secret")]
        public string Secret { get; internal set; }		
    }
}