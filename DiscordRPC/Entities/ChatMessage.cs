using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Entities
{
    public class ChatMessage
    {
        [JsonProperty("content")]
        public string Content { get; internal set; }

        [JsonProperty("id")]
        public ulong Id { get; internal set; }

        [JsonProperty("author")]
        public User Author { get; internal set; }

        [JsonProperty("bot")]
        public bool? Bot { get; internal set; }
        [JsonProperty("mentions")]
        public User[] Mentions { get; internal set; }
        [JsonProperty("nick")]
        public string Nick { get; internal set; }
    }
}
