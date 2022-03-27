namespace DiscordRPC.RPC.Messaging.Messages
{
    /// <summary>
    /// Called when the Discord Client wishes for this process to spectate a game. D -> C. 
    /// </summary>
    public class SpectateMessage : JoinMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type => MessageType.Spectate;
    }
}