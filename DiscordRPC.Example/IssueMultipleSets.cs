using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC.Logging.Loggers;
using DiscordRPC.Entities;
using DiscordRPC.Entities;
using DiscordRPC.Logging;

namespace DiscordRPC.Example
{
    partial class Program
    {
        static async void IssueMultipleSets()
        {
            // == Create the client
            var client = new DiscordRpcClient("424087019149328395", pipe: discordPipe)
            {
                Logger = new ConsoleLogger(LogLevel.Info, true)
            };

            // == Subscribe to some events
            client.ReadyEvent += (sender, msg) =>
            {
                //Create some events so we know things are happening
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
            };

            client.PresenceUpdateEvent += (sender, msg) =>
            {
                //The presence has updated
                Console.WriteLine("Presence has been updated! ");
            };

            // == Initialize
            client.Initialize();
            int attempt = 0;

            while (true && !Console.KeyAvailable)
            {

                attempt++;

                Console.WriteLine("Setting: {0}", attempt);
                client.SetPresence(new RichPresence()
                {
                    Details = "Test",
                    State = attempt.ToString() + " attempt",
                    Timestamps = Timestamps.Now,
                    Assets = new Assets()
                    {
                        LargeImageKey = $"image_large_" + (attempt % 4),
                        LargeImageText = "Fish Sticks",

                    }
                });

                await Task.Delay(15000);
                //Thread.Sleep(15000);

                client.ClearPresence();

                await Task.Delay(1500);
                //Thread.Sleep(1500);

            }

            Console.WriteLine("EXITED");

            // == At the very end we need to dispose of it
            Console.ReadKey();
            client.Dispose();
        }

    }
}
