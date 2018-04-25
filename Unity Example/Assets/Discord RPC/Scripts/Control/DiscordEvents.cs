using DiscordRPC;
using DiscordRPC.Message;
using System;
using UnityEngine;
using UnityEngine.Events;

[Serializable]
public class DiscordEvents
{
	[Serializable]
	public class ReadyMessageEvent : UnityEvent<ReadyMessage> { }

	[Serializable]
	public class CloseMessageEvent : UnityEvent<CloseMessage> { }

	[Serializable]
	public class ErrorMessageEvent : UnityEvent<ErrorMessage> { }

	[Serializable]
	public class PresenceMessageEvent : UnityEvent<PresenceMessage> { }

	[Serializable]
	public class SubscribeMessageEvent : UnityEvent<SubscribeMessage> { }

	[Serializable]
	public class UnsubscribeMessageEvent : UnityEvent<UnsubscribeMessage> { }

	[Serializable]
	public class JoinMessageEvent : UnityEvent<JoinMessage> { }

	[Serializable]
	public class SpectateMessageEvent : UnityEvent<SpectateMessage> { }

	[Serializable]
	public class JoinRequestMessageEvent : UnityEvent<JoinRequestMessage> { }

	[Serializable]
	public class ConnectionEstablishedMessageEvent : UnityEvent<ConnectionEstablishedMessage> { }

	[Serializable]
	public class ConnectionFailedMessageEvent : UnityEvent<ConnectionFailedMessage> { }
	
	public ReadyMessageEvent OnReady;
	public CloseMessageEvent OnClose;
	public ErrorMessageEvent OnError;
	public PresenceMessageEvent OnPresenceUpdate;
	public SubscribeMessageEvent OnSubscribe;
	public UnsubscribeMessageEvent OnUnsubscribe;
	public JoinMessageEvent OnJoin;
	public SpectateMessageEvent OnSpectate;
	public JoinRequestMessageEvent OnJoinRequest;
	public ConnectionEstablishedMessageEvent OnConnectionEstablished;
	public ConnectionFailedMessageEvent OnConnectionFailed;
	
	public void RegisterEvents(DiscordRpcClient client)
	{
		client.OnReady += (s, args) => OnReady.Invoke(args); 
		client.OnClose += (s, args) => OnClose.Invoke(args);
		client.OnError += (s, args) => OnError.Invoke(args); 

		client.OnPresenceUpdate += (s, args) => OnPresenceUpdate.Invoke(args); 
		client.OnSubscribe += (s, args) => OnSubscribe.Invoke(args);
		client.OnUnsubscribe += (s, args) => OnUnsubscribe.Invoke(args);

		client.OnJoin += (s, args) => OnJoin.Invoke(args);
		client.OnSpectate += (s, args) => OnSpectate.Invoke(args);
		client.OnJoinRequested += (s, args) => OnJoinRequest.Invoke(args);

		client.OnConnectionEstablished += (s, args) => OnConnectionEstablished.Invoke(args);
		client.OnConnectionFailed += (s, args) => OnConnectionFailed.Invoke(args);
	}
}
