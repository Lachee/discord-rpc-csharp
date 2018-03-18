using DiscordRPC.RPC.Payload;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called as validation of a subscribe
	/// </summary>
	public class SubscribeMessage : IMessage
	{
		public override MessageType Type { get { return MessageType.Subscribe; } }
		public EventType Event { get; internal set; }

		public SubscribeMessage() { }
		internal SubscribeMessage(ServerEvent evt)
		{
			switch (evt)
			{
				default:
				case ServerEvent.ActivityJoin:
					Event = EventType.Join;
					break;

				case ServerEvent.ActivityJoinRequest:
					Event = EventType.JoinRequest;
					break;

				case ServerEvent.ActivitySpectate:
					Event = EventType.Spectate;
					break;
			}
		}
	}
}
