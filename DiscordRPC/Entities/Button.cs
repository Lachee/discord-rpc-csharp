using System;
using System.Text;
using DiscordRPC.Exceptions;
using Newtonsoft.Json;

namespace DiscordRPC.Entities
{
    /// <summary>
    /// A Rich Presence button.
    /// </summary>
    public class Button
    {
        /// <summary>
        /// Text shown on the button
        /// <para>Max 32 bytes.</para>
        /// </summary>
        [JsonProperty("label")]
        public string Label
        {
            get => _label;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _label, 32, Encoding.UTF8))
                    throw new StringOutOfRangeException(32);
            }
        }
        private string _label;

        /// <summary>
        /// The URL opened when clicking the button.
        /// <para>Max 512 bytes.</para>
        /// </summary>
        [JsonProperty("url")]
        public string Url
        {
            get => _url;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _url, 512, Encoding.UTF8))
                    throw new StringOutOfRangeException(512);

                if (!Uri.TryCreate(_url, UriKind.Absolute, out _))
                    throw new ArgumentException("Url must be a valid URI");
            }
        }
        private string _url;
    }
}