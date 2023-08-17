using System.Text.Json.Serialization;
using DiscordRPC.RPC.Payload;

namespace DiscordRPC.RPC.Commands
{
    internal class PresenceCommand : ICommand
    {
        /// <summary>
        /// The process ID
        /// </summary>
        [JsonPropertyName("pid")]
        public int PID { get; set; }

        /// <summary>
        /// The rich presence to be set. Can be null.
        /// </summary>
        [JsonPropertyName("activity")]
        public RichPresence Presence { get; set; }

        public IPayload PreparePayload(long nonce)
        {
            return new ArgumentPayload(this, nonce)
            {
                Command = Command.SetActivity
            };
        }
    }
}
