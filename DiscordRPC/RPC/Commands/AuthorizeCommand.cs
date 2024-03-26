using System;
using System.Collections.Generic;
using System.Text;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Commands
{
    internal class AuthorizeCommand : ICommand
    {
        /// <summary>
		/// OAuth2 application id
		/// </summary>
		[JsonProperty("client_id")]
        public string clientID { get; set; }

        /// <summary>
        /// scopes to authorize
        /// </summary>
        [JsonProperty("scopes")]
        public string[] scopes = new string[1] { "identify" };

        /// <summary>
        /// scopes to authorize
        /// </summary>
        [JsonProperty("prompt")]
        public string prompt = "none";

        public IPayload PreparePayload(long nonce)
        {
            return new ArgumentPayload(this, nonce)
            {
                Command = Command.Authorize,
            };
        }
    }

    [JsonObject(MemberSerialization = MemberSerialization.OptIn)]
    [Serializable]
    internal class AuthorizeResponse
    {
        /// <summary>
		/// The OAuth2 authorization code
		/// </summary>
        [JsonProperty("code")]
		public string Code { get; set; }
    }
}
