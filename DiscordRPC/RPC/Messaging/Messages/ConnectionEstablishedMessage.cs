namespace DiscordRPC.RPC.Messaging.Messages
{
    /// <summary>
    /// The connection to the discord client was successful. This is called before <see cref="MessageType.Ready"/>.
    /// </summary>
    public class ConnectionEstablishedMessage : IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type => MessageType.ConnectionEstablished;

        /// <summary>
        /// The pipe we ended up connecting too
        /// </summary>
        public int ConnectedPipe { get; internal set; }
    }
}