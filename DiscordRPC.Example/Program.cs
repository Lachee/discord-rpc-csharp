using DiscordRPC;
using DiscordRPC.Helper;
using DiscordRPC.Message;
using System;
using System.Text;
using System.Threading;

namespace DiscordRPC.Example
{
	class Program
	{
		/// <summary>
		/// The pipe Discord is located on. If set to -1, the client will scan for the first available pipe.
		/// </summary>
		private static int DiscordPipe = -1;

		/// <summary>
		/// The level of logging to use.
		/// </summary>
		private static Logging.LogLevel DiscordLogLevel = Logging.LogLevel.Warning;

		/// <summary>
		/// The current presence to send to discord.
		/// </summary>
		private static RichPresence presence = new RichPresence();

		/// <summary>
		/// Is the main loop currently running?
		/// </summary>
		private static bool isRunning = true;

		private static StringBuilder word = new StringBuilder();
		
		//Main Loop
		static void Main(string[] args)
		{
			//Creates a new Discord RPC Client. Below are some of the ways to register:
			//using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", null, true, DiscordPipe, new IO.NativeNamedPipeClient()))	//This will create a new client with the specified pipe client
			//using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", null, true, DiscordPipe))									//This will create a new client on the specified pipe
			//using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", null, true))												//This will create a new client with a SteamID (null if no steam)
			using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", true))														//This will create a new client that will register itself a URI scheme (for join / spectate)
			{
				//Set the logger. This way we can see the output of the client.
				client.Logger = new Logging.ConsoleLogger() { Level = DiscordLogLevel };
				client.Logger = new Logging.FancyConsoleLogger() { Level = DiscordLogLevel };

				//Register to the events we care about. We are registering to everyone just to show off the events
				client.OnReady += OnReady;
				client.OnClose += OnClose;
				client.OnError += OnError;

				client.OnConnectionEstablished += OnConnectionEstablished;
				client.OnConnectionFailed += OnConnectionFailed;

				client.OnPresenceUpdate += OnPresenceUpdate;

				client.OnSubscribe += OnSubscribe;
				client.OnUnsubscribe += OnUnsubscribe;

				client.OnJoin += OnJoin;
				client.OnSpectate += OnSpectate;
				client.OnJoinRequested += OnJoinRequested;

				//Initialize the connection. This must be called ONLY once.
				//It must be called before any updates are sent or received from the discord client.
				client.Initialize();

				//Set some new presence to tell Discord we are in a game.
				// If the connection is not yet available, this will be queued until a Ready event is called, 
				// then it will be sent. All messages are queued until Discord is ready to receive them.
				client.SetPresence(presence);

				//Subscribe to the join / spectate feature.
				//These require the RegisterURI to be true.
				client.Subscribe(EventType.Join);			//This will alert us if discord wants to join a game
				client.Subscribe(EventType.Spectate);		//This will alert us if discord wants to spectate a game
				client.Subscribe(EventType.JoinRequest);	//This will alert us if someone else wants to join our game.

				//Start our main loop. In a normal game you probably don't have to do this step.
				// Just make sure you call .Invoke() or some other dequeing event to receive your events.
				MainLoop(client);
			}
		}

		static void MainLoop(DiscordRpcClient client)
		{
			/*
			 * Enter a infinite loop, polling the Discord Client for events.
			 * In game termonology, this will be equivalent to our main game loop. 
			 * If you were making a GUI application without a infinite loop, you could implement
			 * this with timers.
			*/
			isRunning = true;
			while (client != null && isRunning)
			{
				//We will invoke the client events. 
				// In a game situation, you would do this in the Update.
				if (client != null)
					client.Invoke();

				//Try to read any keys if available
				if (Console.KeyAvailable)
				{
					var key = Console.ReadKey(true);
					if (key.Key == ConsoleKey.Enter)
					{
						Console.Write("\r                                                                            \r");
						ExecuteCommand(word.ToString());
						word.Clear();
					}
					else
					{
						Console.Write(key.KeyChar);
						word.Append(key.KeyChar);
					}
				}

				//This can be what ever value you want, as long as it is faster than 30 seconds.
				//Console.Write("+");
				Thread.Sleep(100);
			}

			Console.WriteLine("Press any key to terminate");
			Console.ReadKey();
		}

		static void ExecuteCommand(string word)
		{
			Console.WriteLine("Sending Presence");
		}

		#region Events

		#region State Events
		private static void OnReady(object sender, ReadyMessage args)
		{
			//This is called when we are all ready to start receiving and sending discord events. 
			// It will give us some basic information about discord to use in the future.
			
			//It can be a good idea to send a inital presence update on this event too, just to setup the inital game state.
			Console.WriteLine("On Ready. RPC Version: {0}", args.Version);
		}
		private static void OnClose(object sender, CloseMessage args)
		{
			//This is called when our client has closed. The client can no longer send or receive events after this message.
			// Connection will automatically try to re-establish and another OnReady will be called (unless it was disposed).
			Console.WriteLine("Lost Connection with client because of '{0}'", args.Reason);
		}
		private static void OnError(object sender, ErrorMessage args)
		{
			//Some error has occured from one of our messages. Could be a malformed presence for example.
			// Discord will give us one of these events and its upto us to handle it
			Console.WriteLine("Error occured within discord. ({1}) {0}", args.Message, args.Code);
		}
		#endregion

		#region Pipe Connection Events
		private static void OnConnectionEstablished(object sender, ConnectionEstablishedMessage args)
		{
			//This is called when a pipe connection is established. The connection is not ready yet, but we have at least found a valid pipe.
			Console.WriteLine("Pipe Connection Established. Valid on pipe #{0}", args.ConnectedPipe);
		}
		private static void OnConnectionFailed(object sender, ConnectionFailedMessage args)
		{
			//This is called when the client fails to establish a connection to discord. 
			// It can be assumed that Discord is unavailable on the supplied pipe.
			Console.WriteLine("Pipe Connection Failed. Could not connect to pipe #{0}", args.FailedPipe);
		}
		#endregion

		private static void OnPresenceUpdate(object sender, PresenceMessage args)
		{
			//This is called when the Rich Presence has been updated in the discord client.
			// Use this to keep track of the rich presence and validate that it has been sent correctly.
			Console.WriteLine("Rich Presence Updated. Playing {0}", args.Presence == null ? "Nothing (NULL)" : args.Presence.State);
		}

		#region Subscription Events
		private static void OnSubscribe(object sender, SubscribeMessage args)
		{
			//This is called when the subscription has been made succesfully. It will return the event you subscribed too.
			Console.WriteLine("Subscribed: {0}", args.Event);
		}
		private static void OnUnsubscribe(object sender, UnsubscribeMessage args)
		{
			//This is called when the unsubscription has been made succesfully. It will return the event you unsubscribed from.
			Console.WriteLine("Unsubscribed: {0}", args.Event);
		}
		#endregion

		#region Join / Spectate feature
		private static void OnJoin(object sender, JoinMessage args)
		{
			/*
			 * This is called when the Discord Client wants to join a online game to play.
			 * It can be triggered from a invite that your user has clicked on within discord or from an accepted invite.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
			Console.WriteLine("Joining Game '{0}'", args.Secret);
		}

		private static void OnSpectate(object sender, SpectateMessage args)
		{   /*
			 * This is called when the Discord Client wants to join a online game to watch and spectate.
			 * It can be triggered from a invite that your user has clicked on within discord.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
			Console.WriteLine("Spectating Game '{0}'", args.Secret);
		}

		private static void OnJoinRequested(object sender, JoinRequestMessage args)
		{
			/*
			 * This is called when the Discord Client has received a request from another external Discord User to join your game.
			 * You should trigger a UI prompt to your user sayings 'X wants to join your game' with a YES or NO button. You can also get
			 *  other information about the user such as their avatar (which this library will provide a useful link) and their nickname to
			 *  make it more personalised. You can combine this with more API if you wish. Check the Discord API documentation.
			 *  
			 *  Once a user clicks on a response, call the Respond function, passing the message, to respond to the request.
			 *  A example is provided below.
			 *  
			 * This feature requires the RegisterURI to be true on the client.
			*/

			//We have received a request, dump a bunch of information for the user
			Console.WriteLine("'{0}' has requested to join our game.", args.User.Username);
			Console.WriteLine(" - User's Avatar: {0}", args.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x2048));
			Console.WriteLine(" - User's Descrim: {0}", args.User.Descriminator);
			Console.WriteLine(" - User's Snowflake: {0}", args.User.ID);
			Console.WriteLine();

			//Ask the user if they wish to accept the join request.
			Console.Write("Do you give this user permission to join? [Y / n]: ");
			bool accept = Console.ReadKey().Key == ConsoleKey.Y; Console.WriteLine();

			//Tell the client if we accept or not.
			DiscordRpcClient client = (DiscordRpcClient)sender;
			client.Respond(args, accept);

			//All done.
			Console.WriteLine(" - Sent a {0} invite to the client {1}", accept ? "ACCEPT" : "REJECT", args.User.Username);
		}
		#endregion

		#endregion
		
	}
}
