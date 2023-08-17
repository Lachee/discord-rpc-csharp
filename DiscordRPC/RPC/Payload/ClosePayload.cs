using System.Text.Json.Serialization;

namespace DiscordRPC.RPC.Payload
{
    internal class ClosePayload : IPayload
    {
        /// <summary>
        /// The close code the discord gave us
        /// </summary>
        [JsonPropertyName("code")]
        public int Code { get; set; }

        /// <summary>
        /// The close reason discord gave us
        /// </summary>
        [JsonPropertyName("message")]
        public string Reason { get; set; }

        [JsonConstructor]
        public ClosePayload()
            : base()
        {
            Code = -1;
            Reason = "";
        }
    }
}
