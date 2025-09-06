namespace DiscordRPC.Message
{
	/// <summary>
	/// Too many failed connection tries. You must reinitialize the client.
	/// </summary>
	public class TooManyConnectionTriesMessage : IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public override MessageType Type { get { return MessageType.TooManyConnectionTries; } }
	}
}
