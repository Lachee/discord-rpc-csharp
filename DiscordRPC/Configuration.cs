using System.Text.Json.Serialization;

namespace DiscordRPC
{
    /// <summary>
    /// Configuration of the current RPC connection
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// The Discord API endpoint that should be used.
        /// </summary>
        [JsonPropertyName("api_endpoint")]
        public string ApiEndpoint { get; set; }

        /// <summary>
        /// The CDN endpoint
        /// </summary>
        [JsonPropertyName("cdn_host")]
        public string CdnHost { get; set; }

        /// <summary>
        /// The type of environment the connection on. Usually Production.
        /// </summary>
        [JsonPropertyName("environment")]
        public string Environment { get; set; }
    }
}
