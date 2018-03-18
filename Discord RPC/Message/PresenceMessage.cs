

namespace DiscordRPC.Message
{
	/// <summary>
	/// Representation of the message received by discord when the presence has been updated.
	/// </summary>
	public class PresenceMessage : IMessage
	{
		public override MessageType Type { get { return MessageType.PresenceUpdate; } }

		private PresenceMessage() { }
		internal PresenceMessage(RichPresenceResponse rpr)
		{
			Presence = (RichPresence)rpr;
			Name = rpr.Name;
			ApplicationID = rpr.ClientID;
		}

		/// <summary>
		/// The rich presence Discord has set
		/// </summary>
		public RichPresence Presence { get; set; }

		/// <summary>
		/// The name of the application Discord has set it for
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The ID of the application discord has set it for
		/// </summary>
		public string ApplicationID { get; set; }
	}
}
