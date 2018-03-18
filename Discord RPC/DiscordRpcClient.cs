using DiscordRPC.Message;
using DiscordRPC.Registry;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Commands;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// A Discord RPC Client which is used to send Rich Presence updates and receive Join / Spectate events.
	/// </summary>
	public class DiscordRpcClient : IDisposable
	{
		#region Properties
		/// <summary>
		/// Gets a value indicating if the RPC Client has registered a URI scheme. If this is false, Join / Spectate events will fail.
		/// </summary>
		public bool HasRegisteredUriScheme { get; private set; }

		/// <summary>
		/// Gets the Application ID of the RPC Client.
		/// </summary>
		public string ApplicationID { get; private set; }

		/// <summary>
		/// Gets the Steam ID of the RPC Client. This value can be null if none was supplied.
		/// </summary>
		public string SteamID { get; private set; }

		/// <summary>
		/// Gets the ID of the process used to run the RPC Client. Discord tracks this process ID and waits for its termination.
		/// </summary>
		public int ProcessID { get; private set; }
		#endregion

		/// <summary>
		/// The pipe the discord client is on, ranging from 0 to 9. Use -1 to scan through all pipes.
		/// <para>This property can be used for testing multiple clients. For example, if a Discord Client was on pipe 0, the Discord Canary is most likely on pipe 1.</para>
		/// </summary>
		public int TargetPipe { get { return _pipe; } }
		private int _pipe = -1;
		private RpcConnection connection;

		/// <summary>
		/// The current presence that the client has.
		/// </summary>
		public RichPresence CurrentPresence { get { return _presence; } }
		private RichPresence _presence;

		/// <summary>
		/// The current configuration the connection is using. Only becomes available after a ready event.
		/// </summary>
		public Configuration Configuration { get { return _configuration; } }
		private Configuration _configuration;


		//TODO: Include events

		#region Initialization

		/// <summary>
		/// Creates a new Discord RPC Client without using any uri scheme. This will disable the Join / Spectate functionality.
		/// </summary>
		/// <param name="applicationID"></param>
		public DiscordRpcClient(string applicationID) : this(applicationID, -1) { }

		/// <summary>
		/// Creates a new Discord RPC Client without using any uri scheme. This will disable the Join / Spectate functionality.
		/// </summary>
		/// <param name="applicationID"></param>	
		/// <param name="pipe">The pipe to connect too. -1 for first available pipe.</param>
		public DiscordRpcClient(string applicationID, int pipe) : this(applicationID, null, false, pipe) { }


		/// <summary>
		/// Creates a new Discord RPC Client using the default uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="registerUriScheme">Should a URI scheme be registered for Join / Spectate functionality? If false, the Join / Spectate functionality will be disabled.</param>
		public DiscordRpcClient(string applicationID, bool registerUriScheme) : this(applicationID,  registerUriScheme, -1) { }

		/// <summary>
		/// Creates a new Discord RPC Client using the default uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="registerUriScheme">Should a URI scheme be registered for Join / Spectate functionality? If false, the Join / Spectate functionality will be disabled.</param>
		/// <param name="pipe">The pipe to connect too. -1 for first available pipe.</param>
		public DiscordRpcClient(string applicationID, bool registerUriScheme, int pipe) : this(applicationID, null, registerUriScheme, pipe) { }

		/// <summary>
		/// Creates a new Discord RPC Client using the steam uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="steamID">The steam ID of the app. This is used to launch Join / Spectate through steam URI scheme instead of manual launching</param>
		/// <param name="pipe">The pipe to connect too. -1 for first available pipe.</param>	
		public DiscordRpcClient(string applicationID, string steamID, bool registerUriScheme) : this(applicationID, steamID, registerUriScheme, -1) { }

		/// <summary>
		/// Creates a new Discord RPC Client using the steam uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="steamID">The steam ID of the app. This is used to launch Join / Spectate through steam URI scheme instead of manual launching</param>
		/// <param name="registerUriScheme">Should a URI scheme be registered for Join / Spectate functionality? If false, the Join / Spectate functionality will be disabled.</param>
		/// <param name="pipe">The pipe to connect too. -1 for first available pipe.</param>
		public DiscordRpcClient(string applicationID, string steamID, bool registerUriScheme, int pipe)
		{

			//Store our values
			ApplicationID = applicationID;
			SteamID = steamID;
			HasRegisteredUriScheme = registerUriScheme;
			ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;
			_pipe = pipe;

			//If we are to register the URI scheme, do so.
			//The UriScheme.RegisterUriScheme function takes steamID as a optional parameter, its null by default.
			//   this means it will handle a null steamID for us :)
			if (registerUriScheme)
				UriScheme.RegisterUriScheme(applicationID, steamID);

			//Create the RPC client
			connection = new RpcConnection(ApplicationID, ProcessID, TargetPipe);
			connection.AttemptConnection();
		}

		#endregion

		#region Message Handling
		/// <summary>
		/// Deques all messages from discord and invokes appropriate events.
		/// </summary>
		public void Invoke()
		{
			//TODO: Make Invoke() go through each message from the pipe and invoke an event
			throw new NotImplementedException();
		}

		/// <summary>
		/// Gets a single message from the queue. This may return null if none are availble. This will process the message and update internal state before handing it over.
		/// </summary>
		/// <returns></returns>
		public IMessage Dequeue()
		{
			//Dequeue the message and do some preprocessing
			IMessage message = connection.DequeueMessage();
			HandleMessage(message);

			//return the message
			return message;
		}

		/// <summary>
		/// Dequeues all messages from the Discord queue. This will be a empty array of size 0 if none are availble. This will process the messages and update internal state before handing it over.
		/// </summary>
		/// <returns></returns>
		public IMessage[] DequeueAll()
		{
			//Dequeue all the messages and process them
			IMessage[] messages = connection.DequeueMessages();
			for (int i = 0; i < messages.Length; i++) HandleMessage(messages[i]);

			//Return it
			return messages;
		}

		private void HandleMessage(IMessage message)
		{
			if (message == null) return;
			switch (message.Type)
			{
				//We got a update, so we will update our current presence
				case MessageType.PresenceUpdate:
					var pm = message as PresenceMessage;
					if (pm != null) _presence = pm.Presence;
					break;

				//Update our configuration
				case MessageType.Ready:
					var rm = message as ReadyMessage;
					if (rm != null) _configuration = rm.Configuration;
					break;

				//Update the request's CDN for the avatar helpers
				case MessageType.JoinRequest:
					if (Configuration != null)
					{
						//Update the User object within the join request if the current Cdn
						var jrm = message as JoinRequestMessage;
						if (jrm != null) jrm.User.CdnEndpoint = Configuration.CdnHost;
					}
					break;

				//We got a message we dont know what to do with.
				default:
					break;
			}
		}
		#endregion

		/// <summary>
		/// Respond to a Join Request. Give TRUE to allow the user to join, otherwise false. All requests will timeout after 30 seconds, so be sure to <see cref="Dequeue"/> frequently enough.
		/// </summary>
		/// <param name="request">The request that is being responded too.</param>
		/// <param name="acceptRequest">Is the request accepted?</param>
		public void Respond(JoinRequestMessage request, bool acceptRequest)
		{
			connection.EnqueueCommand(new RespondCommand() { Accept = acceptRequest, UserID = request.User.ID.ToString() });
		}

		/// <summary>
		/// Sets the Rich Presences
		/// </summary>
		/// <param name="presence">The rich presence to send to discord</param>
		public void SetPresence(RichPresence presence)
		{
			//Enqueue the presence command to be sent
			_presence = presence;
			connection.EnqueueCommand(new PresenceCommand() { PID = this.ProcessID, Presence = presence?.Clone() });
		}

		/// <summary>
		/// Clears the Rich Presence. Use this just before disposal to prevent ghosting.
		/// </summary>
		public void ClearPresence()
		{
			//Just a wrapper function for sending null
			SetPresence(null);
		}

		/// <summary>
		/// Subscribes to an event from the server. Used for Join / Spectate feature. If you have not registered your application, this feature is unavailable.
		/// </summary>
		/// <param name="type"></param>
		public void Subscribe(EventType type)
		{
			if (!HasRegisteredUriScheme)
				throw new Exception("Cannot subscribe to an event as this application has not registered a URI scheme.");

			//Add the subscribe command to be sent when the connection is able too
			connection.EnqueueCommand(new SubscribeCommand() { Event = type, IsUnsubscribe = false });
		}
		
		/// <summary>
		/// Subscribes to an event from the server. Used for Join / Spectate feature. If you have not registered your application, this feature is unavailable.
		/// </summary>
		/// <param name="type"></param>
		public void Unubscribe(EventType type)
		{
			if (!HasRegisteredUriScheme)
				throw new Exception("Cannot unsubscribe to an event as this application has not registered a URI scheme.");

			//Add the subscribe command to be sent when the connection is able too
			connection.EnqueueCommand(new SubscribeCommand() { Event = type, IsUnsubscribe = true });
		}

		/// <summary>
		/// Causes the Rich Presence Connection to force a reconnection.
		/// </summary>
		public void Reconnect()
		{
			connection.Reconnect();
		}

		/// <summary>
		/// Disconnects the server, forcing a new connection to be established on next <see cref="SetPresence(RichPresence)"/>
		/// </summary>
		public void Close()
		{
			//connectionPipe.Close();
			connection.Close();
		}

		/// <summary>
		/// Terminates the connection to Discord and disposes of the object.
		/// </summary>
		public void Dispose()
		{
			//connectionPipe.Close();
			connection.Close();
		}

	}
}
