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


			using (var rpc = new RpcConnection("259970131059408897"))
			{
				bool isRunning = true;
				while (isRunning)
				{
					//Read the command
					Console.Write("Command Line: ");
					string command = Console.ReadLine();
					string[] parts = command.Split(new char[] { ' ' }, 2);

					switch (parts[0])
					{
						//Set the presence
						case "apply":
							rpc.SetPresence(presence);
							break;

						default:
							Console.WriteLine("Unkown Command");
							break;

					}
				}
			}
		}
	}
}
