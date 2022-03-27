using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC.Logging;
using DiscordRPC.Logging.Loggers;
using DiscordRPC.Entities;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Events;
using DiscordRPC.Entities;

namespace DiscordRPC.Example
{
    partial class Program
    {
        static async void IssueJoinLogic()
        {
            // == Create the client
            var random = new Random();
            var client = new DiscordRpcClient("424087019149328395", pipe: 0)
            {
                Logger = new ConsoleLogger(LogLevel.Info, true)
            };

            // == Subscribe to some events
            client.ReadyEvent += (sender, msg) => {  Console.WriteLine("Connected to discord with user {0}", msg.User.Username); };
            client.PresenceUpdateEvent += (sender, msg) => { Console.WriteLine("Presence has been updated! ");  };

            //Setup the join event
            client.Subscribe(EventType.Join | EventType.JoinRequest);
            client.RegisterUriScheme();

            //= Request Event
            client.JoinRequestedEvent += (sender, msg) =>
            {
                Console.WriteLine("Someone wants to join us: {0}", msg.User.Username);
            };

            //= Join Event
            client.JoinEvent += (sender, msg) =>
            {
                Console.WriteLine("Joining this dude: {0}", msg.Secret);
            };

            // == Initialize
            client.Initialize();

            //Set the presence
            client.SetPresence(new RichPresence()
            {
                State = "Potato Pitata",
                Details = "Testing Join Feature",
                Party = new Party()
                {
                    Id = Secrets.CreateFriendlySecret(random),
                    Size = 1,
                    Max = 4,
                    Privacy = PrivacySetting.Public
                },
                Secrets = new Secrets()
                {
                    JoinSecret = Secrets.CreateFriendlySecret(random),
                }
            });

            // == At the very end we need to dispose of it
            Console.ReadKey();
            client.Dispose();
        }

    }
}
