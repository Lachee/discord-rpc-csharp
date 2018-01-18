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
			ConnectionWindows conn = new ConnectionWindows();


			if (conn.Open())
			{
				Console.WriteLine("Success! Pipe: {0}", conn.PipeNumber);
			}
			else
			{
				Console.WriteLine("Failure!");
			}

			Console.ReadKey();
			conn.Close();

			Console.WriteLine("Goodbye!");
			Console.ReadKey();
		}
	}
}
