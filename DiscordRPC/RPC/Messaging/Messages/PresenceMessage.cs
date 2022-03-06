namespace DiscordRPC.RPC.Messaging.Messages
{
    /// <summary>
    /// Representation of the message received by discord when the presence has been updated.
    /// </summary>
    public class PresenceMessage : IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public override MessageType Type => MessageType.PresenceUpdate;

        /// <summary>
        /// The rich presence Discord has set
        /// </summary>
        public BaseRichPresence Presence { get; internal set; }

        /// <summary>
        /// The name of the application Discord has set it for
        /// </summary>
        public string Name { get; internal set; }

        /// <summary>
        /// The ID of the application discord has set it for
        /// </summary>
        public string ApplicationId { get; internal set; }
        
        internal PresenceMessage() : this(null) { }
        internal PresenceMessage(RichPresenceResponse rpr)
        {
            if (rpr == null)
            {
                Presence = null;
                Name = "No Rich Presence";
                ApplicationId = "";
            }
            else
            {
                Presence = rpr;
                Name = rpr.Name;
                ApplicationId = rpr.ClientId;
            }
        }
    }
}