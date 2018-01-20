using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC;

namespace DiscordRPC.Test
{
	class Program
	{

		static RichPresence presence;
		static void Main(string[] args)
		{
			//Read the key from a file
			string key = System.IO.File.ReadAllText("discord.key");

			//Create the presence
			presence = new RichPresence()
			{
				Details = "Testing Library",
				State = "In Editor",
				Instance = true,
				Assets = new Assets()
				{
					LargeImageKey = "default_large",
					LargeImageText = "Where's Perry?",
					SmallImageKey = "default_small",
					SmallImageText = "THREADS RULE",
				},

				Party = new Party()
				{
					ID = "dickwaffles",
					Size = 1,
					Max = 5
				},

				Timestamps = new Timestamps()
				{
					Start = DateTime.UtcNow
				}
			};


			Console.WriteLine("Establishing Client...");
			using (DiscordClient rpc = new DiscordClient(key))
			{
				//DiscordClient.OnLog += (f, objs) => Console.WriteLine("LOG: {0}", string.Format(f, objs));
				rpc.OnError += (s, e) => Console.WriteLine("ERR: An error has occured! ({0}) {1}", e.ErrorCode, e.Message);
				
				while (true)
				{
					Console.Write("Command Line: ");
					string command = Console.ReadLine();
					string[] parts = command.Split(new char[] { ' ' }, 2);

					switch(parts[0])
					{
						case "update":
							rpc.UpdatePresence();
							break;

						case "details":
							presence.Details = parts[1];
							rpc.SetPresence(presence);
							break;

						case "state":
							presence.State = parts[1];
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
