using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// The secrets used for Joining. Secrets are obfuscated data of your choosing. They could be match ids, player ids, lobby ids, etc.
	/// <para>To keep security on the up and up, Discord requires that you properly hash/encode/encrypt/put-a-padlock-on-and-swallow-the-key-but-wait-then-how-would-you-open-it your secrets.</para>
	/// <para>You should send discord data that someone else's game client would need to join their friend. If you can't or don't want to support those actions, you don't need to send secrets.</para>
	/// <para>Visit the <see href="https://discordapp.com/developers/docs/rich-presence/how-to#secrets">Rich Presence How-To</see> for more information.</para>
	/// </summary>
	[Serializable]
	public class Secrets
	{
		/// <summary>
		/// The unique match code to distinguish different games/lobbies. Use <see cref="Secrets.CreateSecret(Random)"/> to get an appropriately sized secret. 
		/// <para>This cannot be null and must be supplied for the  Join / Spectate feature to work.</para>
		/// <para>Max Length of 128 characters</para>
		/// </summary>
		[Obsolete("This feature has been deprecated my Mason in issue #152 on the offical library. Was originally used as a Notify Me feature, it has been replaced with Join / Spectate.", true)]
		[JsonIgnore]
		public string MatchSecret;

		/// <summary>
		/// The secret data that will tell the client how to connect to the game to play. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
		/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
		/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
		/// </para>
		/// <para>Max Length of 128 characters</para>
		/// </summary>
		[JsonProperty("join", NullValueHandling = NullValueHandling.Ignore)]
		public string Join
		{
			get => _joinSecret;
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _joinSecret, false, 128))
					throw new StringOutOfRangeException(128);
			}
		}
		private string _joinSecret;

		/// <summary>Alias of Join</summary>
		/// <remarks>This was made obsolete as the property name contains redundant information.</remarks>
		[System.Obsolete("Property name is redundant and replaced with Join.")]
		[JsonIgnore]
		public string JoinSecret
		{
			get => Join;
			set => Join = value;
		}

		/// <summary>
		/// The secret data that will tell the client how to connect to the game to spectate. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
		/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
		/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
		/// </para>
		/// <para>Max Length of 128 characters</para>
		/// </summary>
		[System.Obsolete("Spectating is no longer supported by Discord.")]
		[JsonIgnore]
		public string SpectateSecret { get; set; }
		#region Statics

		/// <summary>
		/// The encoding the secret generator is using
		/// </summary>
		public static Encoding Encoding => Encoding.UTF8;

		/// <summary>
		/// The length of a secret in bytes.
		/// </summary>
		public const int SecretLength = 128;

		/// <summary>
		/// Creates a new secret. This is NOT a cryptographic function and should NOT be used for sensitive information. This is mainly provided as a way to generate quick IDs.
		/// </summary>
		/// <param name="random">The random to use</param>
		/// <param name="length">The length of the secret to generate. Defaults to <see cref="SecretLength"/>.</param>
		/// <returns>Returns a string with random characters from <see cref="Encoding"/></returns>
		public static string CreateSecret(Random random, int length = SecretLength)
		{
			if (length < 1 || length > SecretLength)
				throw new ArgumentOutOfRangeException(nameof(length), "Secret length must be between 1 and 128 characters.");

			//Prepare an array and fill it with random bytes
			// THIS IS NOT SECURE! DO NOT USE THIS FOR PASSWORDS!
			byte[] bytes = new byte[length];
			random.NextBytes(bytes);

			//Return the encoding. Probably should remove invalid characters but cannot be fucked.
			return Encoding.GetString(bytes);
		}


		/// <summary>
		/// Creates a secret word using more readable friendly characters. Useful for debugging purposes. This is not a cryptographic function and should NOT be used for sensitive information.
		/// </summary>
		/// <param name="random">The random used to generate the characters</param>
		/// <param name="length">The length of the secret to generate. Defaults to <see cref="SecretLength"/>.</param>
		/// <returns>Returns a string with random alphanumeric characters</returns>
		public static string CreateFriendlySecret(Random random, int length = SecretLength)
		{
			if (length < 1 || length > SecretLength)
				throw new ArgumentOutOfRangeException(nameof(length), "Secret length must be between 1 and 128 characters.");

			//Characters to use in the secret
			string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

			StringBuilder builder = new StringBuilder();
			for (int i = 0; i < length; i++)
				builder.Append(charset[random.Next(charset.Length)]);

			return builder.ToString();
		}
		#endregion
	}
}
