using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordRPC.Example
{
    partial class Program
    {
        static void Issue104()
        {
            // == Create the client
            var client = new DiscordRpcClient("424087019149328395", pipe: discordPipe)
            {
                Logger = new Logging.ConsoleLogger(logLevel, true)
            };

            // == Subscribe to some events
            client.OnReady += (sender, msg) =>
            {
                //Create some events so we know things are happening
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
            };

            client.OnPresenceUpdate += (sender, msg) =>
            {
                //The presence has updated
                Console.WriteLine("Presence has been updated! ");
            };

            // == Initialize
            client.Initialize();

            // Set the first presence
            Console.WriteLine("=================== SET 1 ===================");
            client.SetPresence(new RichPresence()
            {
                Details = "A Basic Example",
                State = "In Game",
                Timestamps = Timestamps.FromTimeSpan(10)
            });
            

            // Wait arbitary amount of time
            Thread.Sleep(3000);

            //Set the presence again, but with UpdateState
            Console.WriteLine("=================== SET 2 ===================");
            client.UpdateState("Another Status");

            // == At the very end we need to dispose of it
            Console.ReadKey();
            client.Dispose();
        }

    }
}
