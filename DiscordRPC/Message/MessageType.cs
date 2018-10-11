
namespace DiscordRPC.Message
{
	/// <summary>
	/// Type of message.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// The Discord Client is ready to send and receive messages.
		/// </summary>
		Ready,

		/// <summary>
		/// The connection to the Discord Client is lost. The connection will remain close and unready to accept messages until the Ready event is called again.
		/// </summary>
		Close,

		/// <summary>
		/// A error has occured during the transmission of a message. For example, if a bad Rich Presence payload is sent, this event will be called explaining what went wrong.
		/// </summary>
		Error,

		/// <summary>
		/// The Discord Client has updated the presence.
		/// </summary>
		PresenceUpdate,

		/// <summary>
		/// The Discord Client has subscribed to an event.
		/// </summary>
		Subscribe,

		/// <summary>
		/// The Discord Client has unsubscribed from an event.
		/// </summary>
		Unsubscribe,
		
		/// <summary>
		/// The Discord Client wishes for this process to join a game.
		/// </summary>
		Join,

		/// <summary>
		/// The Discord Client wishes for this process to spectate a game. 
		/// </summary>
		Spectate,

		/// <summary>
		/// Another discord user requests permission to join this game.
		/// </summary>
		JoinRequest,

		/// <summary>
		/// The connection to the discord client was succesfull. This is called before <see cref="Ready"/>.
		/// </summary>
		ConnectionEstablished,

		/// <summary>
		/// Failed to establish any connection with discord. Discord is potentially not running?
		/// </summary>
		ConnectionFailed
	}
}
