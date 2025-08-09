
using System;
using System.Threading.Tasks;

namespace DiscordRPC.Example
{
    public class Basic : IExample
    {
        private DiscordRpcClient client;

        public void Setup()
        {
            client = new DiscordRpcClient("424087019149328395")
            {
                Logger = new Logging.ConsoleLogger(Logging.LogLevel.Info, true)
            };

            client.OnReady += (sender, msg) =>
            {
                //Create some events so we know things are happening
                Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
                Console.WriteLine("Avatar: {0}", msg.User.GetAvatarURL(User.AvatarFormat.WebP));
                Console.WriteLine("Decoration: {0}", msg.User.GetAvatarDecorationURL());
            };

            client.OnPresenceUpdate += (sender, msg) =>
            {
                //The presence has updated
                Console.WriteLine("Presence has been updated! ");
            };

            client.Initialize();
        }

        public Task Run()
        {
            client.SetPresence(new RichPresence()
            {
                Details = "A Basic Example",
                State = "In Game",
                Assets = new Assets()
                {
                    LargeImageKey = "image_large",
                    LargeImageText = "Lachee's Discord IPC Library",
                    SmallImageKey = "image_small"
                },
                Buttons = new Button[]
                {
                    new Button() { Label = "Fish", Url = "https://lachee.dev/" }
                }
            });

            return Task.CompletedTask;
        }

        public void Teardown()
        {
            client?.Dispose();
        }
    }
}