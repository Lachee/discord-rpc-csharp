using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Entities
{
    /// <summary>
    /// OAuth2 Authorization
    /// </summary>
    public class Authorization
    {
        /// <summary>
        /// The access token
        /// </summary>
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        /// <summary>
        /// The refresh token
        /// </summary>
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }

        /// <summary>
        /// How long the token will last for in seconds
        /// </summary>
        [JsonProperty("expires_in")]
        public double ExpiresIn { get; set; }

        /// <summary>
        /// The time the token will expire at
        /// </summary>
        [JsonIgnore]
        public DateTime ExpiresAt { get { return Created + TimeSpan.FromSeconds(ExpiresIn); } }

        /// <summary>
        /// The time the token was created
        /// </summary>
        [JsonIgnore]
        public DateTime Created { get; private set; }

        /// <summary>
        /// Creates a new authorization object
        /// </summary>
        internal Authorization()
        {
            Created = DateTime.Now;
        }

        internal Authorization Clone()
        {
            return new Authorization()
            {
                Created = this.Created,
                ExpiresIn = this.ExpiresIn,
                RefreshToken = this.RefreshToken,
                AccessToken = this.AccessToken
            };
        }
    }
}
