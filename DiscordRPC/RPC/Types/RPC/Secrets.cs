using System;
using System.Text;
using DiscordRPC.Core.Exceptions;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Types.RPC
{
    /// <summary>
    /// The secrets used for Join / Spectate. Secrets are obfuscated data of your choosing. They could be match ids, player ids, lobby ids, etc.
    /// <para>To keep security on the up and up, Discord requires that you properly hash/encode/encrypt/put-a-padlock-on-and-swallow-the-key-but-wait-then-how-would-you-open-it your secrets.</para>
    /// <para>You should send discord data that someone else's game client would need to join or spectate their friend. If you can't or don't want to support those actions, you don't need to send secrets.</para>
    /// <para>Visit the <see href="https://discordapp.com/developers/docs/rich-presence/how-to#secrets">Rich Presence How-To</see> for more information.</para>
    /// </summary>
    [Serializable]
    public class Secrets
    {
        /// <summary>
        /// The encoding the secret generator is using
        /// </summary>
        public static Encoding Encoding => Encoding.UTF8;

        /// <summary>
        /// The length of a secret in bytes.
        /// </summary>
        public static int SecretLength => 128;
        
        /// <summary>
        /// The unique match code to distinguish different games/lobbies. Use <see cref="Secrets.CreateSecret(Random)"/> to get an appropriately sized secret. 
        /// <para>This cannot be null and must be supplied for the  Join / Spectate feature to work.</para>
        /// <para>Max Length of 128 Bytes</para>
        /// </summary>
        [Obsolete("This feature has been deprecated my Mason in issue #152 on the official library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.")]
        [JsonProperty("match", NullValueHandling = NullValueHandling.Ignore)]
        public string MatchSecret
        {
            get => _matchSecret;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _matchSecret, 128, Encoding.UTF8))
                    throw new StringOutOfRangeException(128);
            }
        }
        private string _matchSecret;

        /// <summary>
        /// The secret data that will tell the client how to connect to the game to play. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
        /// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
        /// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
        /// </para>
        /// <para>Max Length of 128 Bytes</para>
        /// </summary>
        [JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
        public string JoinSecret
        {
            get => _joinSecret;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _joinSecret, 128, Encoding.UTF8))
                    throw new StringOutOfRangeException(128);
            }
        }
        private string _joinSecret;

        /// <summary>
        /// The secret data that will tell the client how to connect to the game to spectate. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
        /// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
        /// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
        /// </para>
        /// <para>Max Length of 128 Bytes</para>
        /// </summary>
        [JsonProperty("spectate", NullValueHandling = NullValueHandling.Ignore)]
        public string SpectateSecret
        {
            get => _spectateSecret;
            set
            {
                if (!BaseRichPresence.ValidateString(value, out _spectateSecret, 128, Encoding.UTF8))
                    throw new StringOutOfRangeException(128);
            }
        }
        private string _spectateSecret;
        
        /// <summary>
        /// Creates a new secret. This is NOT a cryptographic function and should NOT be used for sensitive information. This is mainly provided as a way to generate quick IDs.
        /// </summary>
        /// <param name="random">The random to use</param>
        /// <returns>Returns a <see cref="SecretLength"/> sized string with random characters from <see cref="Encoding"/></returns>
        public static string CreateSecret(Random random)
        {
            // Prepare an array and fill it with random bytes
            // THIS IS NOT SECURE! DO NOT USE THIS FOR PASSWORDS!
            var bytes = new byte[SecretLength];
            random.NextBytes(bytes);

            // Return the encoding. Probably should remove invalid characters but cannot be fucked.
            return Encoding.GetString(bytes);
        }


        /// <summary>
        /// Creates a secret word using more readable friendly characters. Useful for debugging purposes. This is not a cryptographic function and should NOT be used for sensitive information.
        /// </summary>
        /// <param name="random">The random used to generate the characters</param>
        /// <returns></returns>
        public static string CreateFriendlySecret(Random random)
        {
            const string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var secret = "";

            for (var i = 0; i < SecretLength; i++) secret += charset[random.Next(charset.Length)];

            return secret;
        }
    }
}