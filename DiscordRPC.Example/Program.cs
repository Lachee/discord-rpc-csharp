using DiscordRPC;
using DiscordRPC.Helper;
using DiscordRPC.Message;
using System;
using System.Threading;

namespace DiscordRPC.Example
{
	class Program
	{

		static void Main(string[] args)
		{
			RichPresence presence = new RichPresence()
			{
				Details = "Testing .NET 3.5",
				State = "Testing",
				Timestamps = new Timestamps()
				{
					Start = DateTime.UtcNow,
				},
				Assets = new Assets()
				{
					LargeImageKey = "image_large_2",
					LargeImageText = " Some Text ",
					SmallImageKey = "image_small_1",
					SmallImageText = null
				},
			};

			//Some helper functions
			int pipe = 0;
			bool makesecret = false;

			do
			{
				//Read what number port we want to use
				Console.WriteLine("What port would you like to use: ");
			}
			while (!int.TryParse(Console.ReadLine(), out pipe));

			//Read if we want to tbe secretivie
			Console.Write("Would you like to make a secret? [Y/n]: ");
			makesecret = Console.ReadKey().Key == ConsoleKey.Y; Console.WriteLine();
			if (makesecret)
			{
				presence.Party = new Party()
				{
					ID = Secret.CreateFriendlySecret(),
					Size = 1,
					Max = 10
				};

				presence.Secrets = new Secrets()
				{
					JoinSecret = Secret.CreateFriendlySecret(),
					MatchSecret = Secret.CreateFriendlySecret(),
					SpectateSecret = Secret.CreateFriendlySecret()
				};
			}

			//Creates a new Discord RPC Client
			using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", true, pipe))
			{
				//Set the loggers
				client.Logger = new Logging.ConsoleLogger() { Level = Logging.LogLevel.Error };

				//Initialize the connection
				client.Initialize();

				//Send the presence
				client.SetPresence(presence);

				//CommandInterface(client);
				PollInterface(client);
			}
		}


		static void PollInterface(DiscordRpcClient client)
		{
			//Add listeners to all the events
			client.OnReady += OnReady;
			client.OnClose += OnClose;
			client.OnError += OnError;
			client.OnPresenceUpdate += OnPresenceUpdate;
			client.OnSubscribe += OnSubscribe;
			client.OnUnsubscribe += OnUnsubscribe;
			client.OnJoin += OnJoin;
			client.OnSpectate += OnSpectate;
			client.OnJoinRequested += OnJoinRequested;

			//Subscribe to the join / spectate feature.
			client.Subscribe(EventType.Join);
			client.Subscribe(EventType.Spectate);
			client.Subscribe(EventType.JoinRequest);

			//Enter a continuous loop, pooling the invoke.
			bool dorun = true;
			while (dorun)
			{
				//Invoke the clients events
				client.Invoke();

				//This can be what ever value you want, as long as it is faster than 30 seconds.
				Thread.Sleep(100);
			}
		}

		private static void OnReady(object sender, ReadyMessage args)
		{
			Console.WriteLine("On Ready: {0}", args.Version);
		}

		private static void OnClose(object sender, CloseMessage args)
		{
			Console.WriteLine("Lost Connection with client: {0}", args.Reason);
		}

		private static void OnError(object sender, ErrorMessage args)
		{
			Console.WriteLine("Error occured: ({1}) {0}", args.Message, args.Code);
		}

		private static void OnPresenceUpdate(object sender, PresenceMessage args)
		{
			Console.WriteLine("Rich Presence Updated: {0}", args.Presence.State);
		}

		private static void OnSubscribe(object sender, SubscribeMessage args)
		{
			Console.WriteLine("Subscribed: {0}", args.Event);
		}

		private static void OnUnsubscribe(object sender, UnsubscribeMessage args)
		{
			Console.WriteLine("Unsubscribed: {0}", args.Event);
		}
		
		private static void OnJoin(object sender, JoinMessage args)
		{
			Console.WriteLine("Joining Game '{0}'....", args.Secret);
			Console.WriteLine(" - Failed: Actual Game Not Implemented.");
		}

		private static void OnSpectate(object sender, SpectateMessage args)
		{
			Console.WriteLine("Spectating Game '{0}'....", args.Secret);
			Console.WriteLine(" - Failed: Actual Game Not Implemented.");
		}

		private static void OnJoinRequested(object sender, JoinRequestMessage args)
		{
			Console.WriteLine("{0} has requested to join our game.", args.User.Username);
			Console.WriteLine(" - User's Avatar: {0}", args.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x2048));
			Console.WriteLine(" - User's Descrim: {0}", args.User.Descriminator);
			Console.WriteLine(" - User's Snowflake: {0}", args.User.ID);
			Console.WriteLine();

			Console.Write("Do you give this user permission to join? [Y / n]: ");
			bool accept = Console.ReadKey().Key == ConsoleKey.Y; Console.WriteLine();

			DiscordRpcClient client = (DiscordRpcClient)sender;
			client.Respond(args, accept);
			Console.WriteLine(" - Sent a {0} invite to the client {1}", accept ? "ACCEPT" : "REJECT", args.User.Username);
		}		
	}
}
