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
				Console.WriteLine("Connected!");
				Console.ReadKey();

				Console.WriteLine("Presence Sent!");
				/*
				rpc.UpdatePresence(new RichPresence()
				{
					State = "Solo",
					Details = "Playing Scrubs"
				});
				*/
				Console.ReadKey();
			}
		}
	}
}
