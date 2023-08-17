using System.Text.Json.Serialization;

namespace DiscordRPC.IO
{
    internal class Handshake
    {
        /// <summary>
        /// Version of the IPC API we are using
        /// </summary>
        [JsonPropertyName("v")]
        public int Version { get; set; }

        /// <summary>
        /// The ID of the app.
        /// </summary>
        [JsonPropertyName("client_id")]
        public string ClientID { get; set; }
    }
}
