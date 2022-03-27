using System;

namespace DiscordRPC.RPC.Messaging
{
    /// <summary>
    /// Messages received from discord.
    /// </summary>
    public abstract class IMessage
    {
        /// <summary>
        /// The type of message received from discord
        /// </summary>
        public abstract MessageType Type { get; }

        /// <summary>
        /// The time the message was created
        /// </summary>
        public DateTime TimeCreated => _timeCreated;

        private readonly DateTime _timeCreated;

        /// <summary>
        /// Creates a new instance of the message
        /// </summary>
        protected IMessage()
        {
            _timeCreated = DateTime.Now;
        }
    }
}