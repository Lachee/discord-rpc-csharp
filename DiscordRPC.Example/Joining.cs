
using System;
using System.Threading;
using System.Threading.Tasks;
using DiscordRPC.Message;

namespace DiscordRPC.Example
{
    class Joining : IExample
    {
        private DiscordRpcClient client;
        private string lobbyId = "1234567890"; // Example lobby ID
        private CancellationTokenSource waitingForJoinCancellationTokenSource;
        private int partySize = 2; // Initial party size, can be updated as users join

        public void Setup(Options opts)
        {
            Random random = new Random();
            waitingForJoinCancellationTokenSource = new CancellationTokenSource();

			client = new DiscordRpcClient(opts.ClientId, pipe: opts.Pipe)
			{
				Logger = new Logging.ConsoleLogger(opts.LogLevel, true)
			};

			// Register the URI scheme. 
			// This is how Discord will launch your game when a user clicks on a join or spectate button.
			client.RegisterUriScheme();

            // Listen to some events
            client.Subscribe(EventType.Join | EventType.JoinRequest);   // Tell Unity we want to handle these events ourselves.
            client.OnJoinRequested += OnJoinRequested;                  // Another Discord user has requested to join our game
            client.OnJoin += OnJoin;                                    // Our Discord client wants to join a specific lobby

            // Setup the initial presence
            client.SetPresence(new()
            {
                Details = "Party Example",
                State = "In Game",

                // Set the party information. This is how Discord will know about your game.
                Party = new()
                {
                    ID = lobbyId,
                    Privacy = Party.PrivacySetting.Private, // < Users will have to Ask to join
                    Size = partySize,
                    Max = 4,
                },

                // Set the secrets. This is how a launching app will figure out how to connect to the game.
                Secrets = new()
                {
                    Join = $"join:{lobbyId}"
                },
            });

            client.Initialize();
        }

        public async Task Run()
        {
            Random random = new Random();


            // Wait for a join request or spectate request
            await Task.Delay(-1, cancellationToken: waitingForJoinCancellationTokenSource.Token);
        }

        public void Teardown()
        {
            // Not require as we aree just immediately disposing, but you should always clean up your events.
            client.OnJoinRequested -= OnJoinRequested;
            client.OnJoin -= OnJoin;

            // Dispose of everything
            waitingForJoinCancellationTokenSource?.Dispose();
            client?.Dispose();
        }


        private void OnJoin(object sender, JoinMessage args)
        {
            /*
			 * This is called when the Discord Client wants to join a online game to play.
			 * It can be triggered from a invite that your user has clicked on within discord or from an accepted invite.
			 * 
			 * The secret should be some sort of encrypted data that will give your game the nessary information to connect.
			 * For example, it could be the Game ID and the Game Password which will allow you to look up from the Master Server.
			 * Please avoid using IP addresses within these fields, its not secure and defeats the Discord security measures.
			 * 
			 * This feature requires the RegisterURI to be true on the client.
			*/
            Console.WriteLine("Joining Game '{0}'", args.Secret);
        }

        private void OnJoinRequested(object sender, JoinRequestMessage args)
        {
            /*
			 * This is called when the Discord Client has received a request from another external Discord User to join your game.
			 * You should trigger a UI prompt to your user sayings 'X wants to join your game' with a YES or NO button. You can also get
			 *  other information about the user such as their avatar (which this library will provide a useful link) and their nickname to
			 *  make it more personalised. You can combine this with more API if you wish. Check the Discord API documentation.
			 *  
			 *  Once a user clicks on a response, call the Respond function, passing the message, to respond to the request.
			 *  A example is provided below.
			 *  
			 * This feature requires the RegisterURI to be true on the client.
			*/

            //We have received a request, dump a bunch of information for the user
            var user = args.User;
            Console.WriteLine("'{0}' has requested to join our game.", user.ToString());
            Console.WriteLine(" - User's Avatar: {0}", user.GetAvatarURL());
            Console.WriteLine(" - User's Username: {0}", user.Username);
            Console.WriteLine(" - User's Snowflake: {0}", user.ID);
            Console.WriteLine();

            //Ask the user if they wish to accept the join request.
            Console.Write("Do you give this user permission to join? [Y / n]: ");
            bool accept = Console.ReadKey().Key == ConsoleKey.Y; Console.WriteLine();

            //Tell the client if we accept or not.
            client.Respond(user, accept);
            Console.WriteLine(" - Sent a {0} invite to the client {1}", accept ? "ACCEPT" : "REJECT", user.Username);

            // Update our presence to reflect the new party size
            if (accept)
                client.UpdatePartySize(++partySize);

        }
    }
}