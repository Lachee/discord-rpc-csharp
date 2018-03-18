
namespace DiscordRPC.Message
{
	/// <summary>
	/// Type of message.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// The presence has been updated
		/// </summary>
		PresenceUpdate,

		/// <summary>
		/// A error message has been sent back
		/// </summary>
		Error,

		/// <summary>
		/// Called when some other person has requested access to this game. C -> D -> C.
		/// </summary>
		JoinRequest,

		/// <summary>
		/// Called when the Discord Client wishes for this process to join a game. D -> C.
		/// </summary>
		Join,

		/// <summary>
		/// Called when the Discord Client wishes for this process to spectate a game. D -> C. 
		/// </summary>
		Spectate,

		/// <summary>
		/// Confirmation of a subscription
		/// </summary>
		Subscribe,

		/// <summary>
		/// The server is ready
		/// </summary>
		Ready,

		/// <summary>
		/// The server has closed
		/// </summary>
		Close
	}
}
