using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Entities
{
    /// <summary>
    /// oAuth2 Application details
    /// </summary>
    public class Application
    {
        /// <summary>
        /// Snowflake ID of the application (the client id).
        /// </summary>
        [JsonProperty("id")]
        public ulong Id { get; private set; }

        /// <summary>
        /// The name of the application
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; private set; }

        /// <summary>
        /// The description of the application
        /// </summary>
        [JsonProperty("description")]
        public string Description { get; private set; }

        /// <summary>
        /// Icon Hash of the application's logo. See User's avatar.
        /// </summary>
        [JsonProperty("icon")]
        public string Icon { get; private set; }

        /// <summary>
        /// Array of RPC origin urls.
        /// </summary>
        [JsonProperty("rpc_origins")]
        public string[] RpcOrigins { get; private set; }
    }
}
