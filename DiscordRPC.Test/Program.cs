using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DiscordRPC.IO;

namespace DiscordRPC.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("Connecting...");
			using (DiscordRPC rpc = new DiscordRPC())
			{
				Console.WriteLine("Connected!");
				Console.ReadKey();

				Console.WriteLine("Presence Sent!");
				rpc.UpdatePresence(new RichPresence()
				{
					State = "Solo",
					Details = "Playing Scrubs"
				});
				Console.ReadKey();
			}
		}
	}
}
