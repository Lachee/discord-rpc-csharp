using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Events
{
    /// <summary>
    /// Called when the Discord Client is ready to send and receive messages.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnReadyEvent(object sender, ReadyMessage args);

    /// <summary>
    /// Called when connection to the Discord Client is lost. The connection will remain close and unready to accept messages until the Ready event is called again.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnCloseEvent(object sender, CloseMessage args);

    /// <summary>
    /// Called when a error has occured during the transmission of a message. For example, if a bad Rich Presence payload is sent, this event will be called explaining what went wrong.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnErrorEvent(object sender, ErrorMessage args);

    /// <summary>
    /// Called when the Discord Client has updated the presence.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnPresenceUpdateEvent(object sender, PresenceMessage args);

    /// <summary>
    /// Called when the Discord Client has subscribed to an event.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnSubscribeEvent(object sender, SubscribeMessage args);

    /// <summary>
    /// Called when the Discord Client has unsubscribed from an event.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnUnsubscribeEvent(object sender, UnsubscribeMessage args);

    /// <summary>
    /// Called when the Discord Client wishes for this process to join a game.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnJoinEvent(object sender, JoinMessage args);

    /// <summary>
    /// Called when the Discord Client wishes for this process to spectate a game.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnSpectateEvent(object sender, SpectateMessage args);

    /// <summary>
    /// Called when another discord user requests permission to join this game.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnJoinRequestedEvent(object sender, JoinRequestMessage args);


    /// <summary>
    /// The connection to the discord client was succesfull. This is called before <see cref="OnReadyEvent"/>.
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnConnectionEstablishedEvent(object sender, ConnectionEstablishedMessage args);

    /// <summary>
    /// Failed to establish any connection with discord. Discord is potentially not running?
    /// </summary>
    /// <param name="sender">The Discord client handler that sent this event</param>
    /// <param name="args">The arguments supplied with the event</param>
    public delegate void OnConnectionFailedEvent(object sender, ConnectionFailedMessage args);


    /// <summary>
    /// A RPC Message is received.
    /// </summary>
    /// <param name="sender">The handler that sent this event</param>
    /// <param name="msg">The raw message from the RPC</param>
    public delegate void OnRpcMessageEvent(object sender, IMessage msg);
}
