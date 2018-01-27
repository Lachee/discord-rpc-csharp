using DiscordRPC.RPC;
using System;

namespace DiscordRPC.Test
{
	class Program
	{
		static void Main(string[] args)
		{
			using (var rpc = new RpcConnection("259970131059408897"))
			{
				rpc.Initialize();
				Console.ReadKey();

				/*
				while (true)
				{
				*/
			}
		}
				
	}
}
