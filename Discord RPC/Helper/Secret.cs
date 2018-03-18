using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Helper
{
	public static class Secret
	{
		/// <summary>
		/// The random used by lazy functions such as <see cref="CreateSecret"/>. Usefull for quick generation of data but not recommended.
		/// </summary>
		public static Random Random { get; set; }


		/// <summary>
		/// The encoding the secret generator is using
		/// </summary>
		public static Encoding Encoding { get { return Encoding.UTF8; } }

		/// <summary>
		/// The number of bytes a secret will be
		/// </summary>
		public static int SecretLength { get { return 128; } }

		/// <summary>
		/// Creates a new secret. This is not a cryptographic function and should NOT be used for sensitive information.
		/// </summary>
		/// <param name="random">The random to use</param>
		/// <returns>Returns a <see cref="SecretLength"/> sized string with random characters from <see cref="Encoding"/></returns>
		public static string CreateSecret(Random random)
		{
			//Prepare an array and fill it with random bytes
			// THIS IS NOT SECURE! DO NOT USE THIS FOR PASSWORDS!
			byte[] bytes = new byte[SecretLength];
			random.NextBytes(bytes);

			//Return the encoding. Probably should remove invalid characters but cannot be fucked.
			return Encoding.GetString(bytes);
		}

		/// <summary>
		/// Creates a new secret using the <see cref="Random"/>. Not recommended, but useful for quick generation. This is not a cryptographic function and should NOT be used for sensitive information. 
		/// </summary>
		/// <returns>Returns a <see cref="SecretLength"/> sized string with random characters from <see cref="Encoding"/></returns>
		public static string CreateSecret()
		{
			if (Random == null) Random = new Random();
			return CreateSecret(Random);
		}

		/// <summary>
		/// Creates a secret word using more readable friendly characters. Useful for debugging purposes. This is not a cryptographic function and should NOT be used for sensitive information.
		/// </summary>
		/// <param name="random">The random used to generate the characters</param>
		/// <returns></returns>
		public static string CreateFriendlySecret(Random random)
		{
			string charset = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			string secret = "";

			for (int i = 0; i < SecretLength; i++)
				secret += charset[random.Next(charset.Length)];

			return secret;
		}

		/// <summary>
		/// Creates a secret word using more readable friendly characters using the <see cref="Random"/>. Not recommended, but useful for quick generation and for debugging purposes. This is not a cryptographic function and should NOT be used for sensitive information.
		/// </summary>
		/// <param name="random">The random used to generate the characters</param>
		/// <returns></returns>
		public static string CreateFriendlySecret()
		{
			if (Random == null) Random = new Random();
			return CreateFriendlySecret(Random);
		}
	}
}
