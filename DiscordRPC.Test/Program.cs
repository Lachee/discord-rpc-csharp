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

			Console.WriteLine("Connecting...");
			using (DiscordClient rpc = new DiscordClient(key))
			{
				DiscordClient.OnLog += (f, objs) => Console.WriteLine("LOG: {0}", string.Format(f, objs));
				rpc.OnError += (s, e) =>
				{
					Console.WriteLine("An error has occured! ({0}) {1}", e.ErrorCode, e.Message);
				};

				Console.WriteLine("Connected!");
				while(true)
				{
					Console.WriteLine("Press a key to update connection");
					Console.ReadKey();
					Console.WriteLine();
					rpc.SetPresence(new RichPresence()
					{
						Details = "Frank Walker from National Tiles",
						State = "Async Sucks",
						Instance = true,
						Assets = new Assets()
						{
							LargeImageKey = "large_default",
							LargeImageText = "Where's Perry?",
							SmallImageKey = "small_default",
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
							Start = 100,
							End = 200
						}
					});

					rpc.UpdateConnection();
				}
			}
		}
	}
}
