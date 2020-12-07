using DiscordRPC.Message;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DiscordRPC.Example
{
    partial class Program
    {
        [System.Obsolete("Experimental Feature")]
        static void AuthorizeDemo()
        {
            // == Create the client
            var client = new DiscordRpcClient("424087019149328395", pipe: discordPipe)
            {
                Logger = new Logging.ConsoleLogger(Logging.LogLevel.Info, true)
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

            //Authorize
            client.Authorize(File.ReadAllText("client_secret.key"), "http://localhost/test", new []{ "rpc", "messages.read" });
            client.OnAuthenticated += (sender, auth) => {
                Console.WriteLine("== AUTHENTICATED");
                client.AddChannelListener(381870553235193857L);
            };

            //This will be a seperate "OnMessageCreate" and not require this OnRpcMessage.
            client.OnRpcMessage += (sender, msg) =>
            {
                if (msg is MessageCreateMessage evt)
                {
                    Console.WriteLine("{0}: {1}", evt.Message.Author.Username, evt.Message.Content);
                }
            };

            // == At the very end we need to dispose of it
            Console.ReadKey();
            client.Dispose();
        }

    }
}
