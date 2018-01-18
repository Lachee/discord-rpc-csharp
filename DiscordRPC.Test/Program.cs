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
			using (DiscordRPC rpc = new DiscordRPC())
			{
				Console.WriteLine("Connected!");
				Console.ReadKey();
			}
		}
	}
}
