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
			DoDiscord().Wait();
		}

		private static async Task DoDiscord()
		{
			try
			{
				//Read the key from a file
				string key = System.IO.File.ReadAllText("discord.key");

				//Create the presence
				presence = new RichPresence()
				{
					State = "                s",
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
					}
				};


				Console.WriteLine("Establishing Client...");
				using (DiscordClient rpc = new DiscordClient(key))
				{
					DiscordClient.OnLog += (f, objs) => Console.WriteLine("LOG: {0}", string.Format(f, objs));
					rpc.OnError += (s, e) => Console.WriteLine("ERR: An error has occured! ({0}) {1}", e.ErrorCode, e.Message);

					while (true)
					{
						Console.Write("Command Line: ");
						string command = Console.ReadLine();
						string[] parts = command.Split(new char[] { ' ' }, 2);

						switch (parts[0])
						{
							case "size":
								int? size = Parse(parts[1]);
								if (size.HasValue) presence.Party.Size = size.Value;
								break;

							case "maxsize":
								int? maxsize = Parse(parts[1]);
								if (maxsize.HasValue) presence.Party.Max = maxsize.Value;
								break;

							case "partyid":
								presence.Party.ID = parts[1];
								break;

							case "details":
								presence.Details = parts[1];
								break;

							case "state":
								presence.State = parts[1];
								break;

							case "largeimg":
								presence.Assets.LargeImageKey = parts[1];
								break;

							case "largetxt":
								presence.Assets.LargeImageText = parts[1];
								break;

							case "smallimg":
								presence.Assets.SmallImageKey = parts[1];
								break;

							case "smalltxt":
								presence.Assets.SmallImageText = parts[1];
								break;

							case "end":
								int? time = Parse(parts[1]);
								if (time.HasValue)
									presence.Timestamps.End = DateTime.UtcNow + new TimeSpan(0, 0, 0, time.Value, 0);
								else
									presence.Timestamps.End = null;

								break;

							case "cend":
								presence.Timestamps.End = null;
								break;

							case "start":
								presence.Timestamps.Start = DateTime.UtcNow;
								break;

							case "cstart":
								presence.Timestamps.Start = null;
								break;

							case "clear":
								await rpc.ClearPresence();
								break;

							case "update":
								await rpc.UpdatePresence();
								break;

							case "apply":
								await rpc.SetPresence(presence);
								break;

							default:
								Console.WriteLine("Unkown Command");
								break;

						}
					}
				}

			}
			catch (Exception e)
			{
				Console.WriteLine("Exception! {0}", e.Message);
				return;
			}
		}
		
		private static int? Parse(string value)
		{
			int i;
			if (!int.TryParse(value, out i))
			{
				Console.WriteLine("'{0}' is a invalid integer.", value);
				return null;
			}

			return i;
		}
				
	}
}
