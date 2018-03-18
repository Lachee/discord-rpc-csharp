using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DiscordRPC.RPC.Payload;

namespace DiscordRPC.RPC.Commands
{
	class SubscribeCommand : ICommand
	{
		public EventType Event { get; set; }
		public bool IsUnsubscribe { get; set; }

		public ServerEvent GetServerEvent()
		{
			switch(Event) {
				default:
				case EventType.Join:
					return ServerEvent.ActivityJoin;

				case EventType.JoinRequest:
					return ServerEvent.ActivityJoinRequest;

				case EventType.Spectate:
					return ServerEvent.ActivitySpectate;
			}
		}

		public IPayload PreparePayload(long nonce)
		{
			return new EventPayload(nonce)
			{
				Command = IsUnsubscribe ? Command.Unsubscribe : Command.Subscribe,
				Event = GetServerEvent()
			};
		}
	}
}
