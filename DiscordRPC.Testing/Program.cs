using DiscordRPC.RPC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Testing
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
					LargeImageKey = "default_large",
					LargeImageText = "  ",
					SmallImageKey = "default_small",
					SmallImageText = "  "
				},
				Party = new Party()
				{
					ID = "someuniqueid",
					Size = 1,
					Max = 10
				}
			};


			//Creates a new Discord RPC Client
			using (DiscordRpcClient client = new DiscordRpcClient("259970131059408897", true))
			{
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

						//Close the server
						case "close":
							Console.WriteLine("Closing Server");
							client.Close();
							break;

						//Some unkown command happened
						default:
							Console.WriteLine("Unkown Command");
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
