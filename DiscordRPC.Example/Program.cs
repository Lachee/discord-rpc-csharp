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
			Console.WriteLine("Would you like to make a secret? [Y/n]: ");
			makesecret = Console.ReadKey().Key == ConsoleKey.Y;
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
				//Send the presence
				client.SetPresence(presence);

				//CommandInterface(client);
				PollInterface(client);
			}
		}


		static void PollInterface(DiscordRpcClient client)
		{
			client.Subscribe(EventType.Join);
			client.Subscribe(EventType.Spectate);
			client.Subscribe(EventType.JoinRequest);

			bool dorun = true;
			while (dorun)
			{
				Thread.Sleep(100);

				IMessage message = client.Dequeue();
				if (message == null) continue;
				
				switch(message.Type)
				{
					case MessageType.Close:
						var close = message as CloseMessage;
						Console.WriteLine(">> Closing: {0}", close.Reason);
						break;

					case MessageType.Error:
						var err = message as ErrorMessage;
						Console.WriteLine(">> Error Occured: ({0}) {1}", err.Code, err.Message);
						break;

					case MessageType.Ready:
						var ready = message as ReadyMessage;
						Console.WriteLine(">> Discord is Ready!");
						break;

					case MessageType.Subscribe:
						var sub = message as SubscribeMessage;
						Console.WriteLine(">> Subscribed to {0}", sub.Event);
						break;

					case MessageType.PresenceUpdate:
						var pres = message as PresenceMessage;
						Console.WriteLine(">> Presence Updated! State: {0}", pres.Presence.State);
						break;

					case MessageType.Spectate:
						var spectate = message as SpectateMessage;
						Console.WriteLine(">> Joining game to spectate: {0}", spectate.Secret);
						break;

					case MessageType.Join:
						var join = message as JoinMessage;
						Console.WriteLine(">> Joining game to play: {0}", join.Secret);
						break;

					case MessageType.JoinRequest:

						var request = message as JoinRequestMessage;

						Console.WriteLine(">> {0} has requested access to our game. Do you accept? [Y/n]:", request.User.Username);
						bool response = Console.ReadKey().Key == ConsoleKey.Y;

						Console.WriteLine(">>>> Responding to {0} with {1}", "{0}", response, request.User.Username);
						client.Respond(request, response);
						break;

					default:
						Console.WriteLine(">> Unhandled Message: {0}", message.Type);
						break;

				}
			}
		}


		static string ReadCommand()
		{
			string cmd = "";
			while (Console.KeyAvailable)
				cmd += Console.ReadKey(true).KeyChar;

			return cmd;
		}

		static void CommandInterface(DiscordRpcClient client)
		{

			//https://discordapp.com/developers/docs/topics/rpc#subscribe
			//https://github.com/discordapp/discord-rpc/blob/master/documentation/hard-mode.md
			client.Subscribe(EventType.Join);
			client.Subscribe(EventType.Spectate);
			client.Subscribe(EventType.JoinRequest);

			//This is our main loop. This can be what ever your application uses to stay alive, doesn't matter
			bool isRunning = true;
			while (isRunning)
			{
				//Read a command from the console and split it up to parts.
				Console.Write("Command Line: ");
				string command = Console.ReadLine();
				string[] parts = command.Split(new char[] { ' ' }, 2);

				//Switch based of the command. I am lazy, sue me.
				switch (parts[0])
				{
					//Set the presence
					//case "apply":
					//	client.SetPresence(presence);
					//	break;

					case "clear":
						client.ClearPresence();
						break;

					case "reconnect":
						client.Reconnect();
						break;

					//Close the server
					case "close":
						Console.WriteLine("Closing Server");
						client.Close();
						break;

					//Some unkown command happened
					default:
					case "help":
						Console.WriteLine("apply, clear, close, reconnect, help, exit");
						break;

					//Exit the loop
					case "exit":
						isRunning = false;
						break;

				}
			}
		}
	}
}
