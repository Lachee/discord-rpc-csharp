namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when the IPC has closed.
	/// </summary>
	public class CloseMessage : IMessage
	{
		/// <summary>
		/// The type of message
		/// </summary>
		public override MessageType Type { get { return MessageType.Close; } }

		/// <summary>
		/// The reason for the close
		/// </summary>
		public string Reason { get; internal set; }

		/// <summary>
		/// The closure code
		/// </summary>
		public int Code { get; internal set; }

		internal CloseMessage() { }
		internal CloseMessage(string reason) { this.Reason = reason; }
	}
}
