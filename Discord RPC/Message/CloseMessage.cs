

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when the IPC has closed.
	/// </summary>
	public class CloseMessage : IMessage
	{
		public override MessageType Type { get { return MessageType.Close; } }
		public string Reason { get; internal set; }

		public CloseMessage() { }
		internal CloseMessage(string reason) { this.Reason = reason; }
	}
}
