using DiscordRPC;
using DiscordRPC.Helper;
using System;

namespace DiscordRPC.Example
{
	class Program
	{
		static void Main(string[] args)
		{
			RichPresence presence = new RichPresence()
			{
				Details = "Testing Cleanup",
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
				Party = new Party()
				{
					ID = "someuniqueid",
					Size = 1,
					Max = 10
				},

				//Create a match secret, using the secret generator provided by the library
				//MatchSecret = Secret.CreateSecret(),
				SpectateSecret = "somesecret",// Secret.CreateSecret()
			};


			//Creates a new Discord RPC Client
			using (DiscordRpcClient client = new DiscordRpcClient("424087019149328395", true))
			{
				//Send the presence
				client.SetPresence(presence);

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
						case "apply":
							client.SetPresence(presence);
							break;

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
}
