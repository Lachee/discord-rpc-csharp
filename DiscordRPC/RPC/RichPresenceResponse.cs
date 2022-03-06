using Newtonsoft.Json;

namespace DiscordRPC.RPC
{
    /// <summary>
    /// A rich presence that has been parsed from the pipe as a response.
    /// </summary>
    internal sealed class RichPresenceResponse : BaseRichPresence
    {
        /// <summary>
        /// ID of the client
        /// </summary>
        [JsonProperty("application_id")]
        public string ClientId { get; private set; }

        /// <summary>
        /// Name of the bot
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }
    }
}