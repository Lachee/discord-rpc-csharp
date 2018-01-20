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
		static void Main(string[] args)
		{
			//Read the key from a file
			string key = System.IO.File.ReadAllText("discord.key");

			Console.WriteLine("Establishing Client...");
			using (DiscordClient rpc = new DiscordClient(key))
			{
				DiscordClient.OnLog += (f, objs) => Console.WriteLine("LOG: {0}", string.Format(f, objs));
				rpc.OnError += (s, e) => Console.WriteLine("ERR: An error has occured! ({0}) {1}", e.ErrorCode, e.Message);
				
				while (true)
				{
					Console.Write("Details: ");
					string details = Console.ReadLine();
					

					rpc.SetPresence(new RichPresence()
					{
						Details = details,
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
							ID = rpc.GetHashCode().ToString(),
							Size = 1,
							Max = 4
						},

						Timestamps = new Timestamps()
						{
							Start = DateTime.UtcNow
						}
					});
				}
			}
		}
	}
}
