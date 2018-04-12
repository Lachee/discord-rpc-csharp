using UnityEngine;

[System.Serializable]
public struct DiscordSecrets
{
	/// <summary>
	/// The secret data that will tell the client how to connect to the game to play. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
	/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
	/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
	/// </para>
	/// <para>Max Length of 128 Bytes</para>
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The secret data that will tell the client how to connect to the game to play. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.")]
	public string joinSecret;


	/// <summary>
	/// The secret data that will tell the client how to connect to the game to spectate. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.
	/// <para>It is recommended to encrypt this information so its hard for people to replicate it. 
	/// Do <b>NOT</b> just use the IP address in this. That is a bad practice and can leave your players vulnerable!
	/// </para>
	/// <para>Max Length of 128 Bytes</para>
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The secret data that will tell the client how to connect to the game to spectate. This could be a unique identifier for a fancy match maker or player id, lobby id, etc.")]
	public string spectateSecret;
	
	/// <summary>
	/// Creates new instances of the secrets, using the <see cref="DiscordRPC.Secrets"/> as the base.
	/// </summary>
	/// <param name="secrets">The base to use the values from</param>
	public DiscordSecrets(DiscordRPC.Secrets secrets)
	{
		this.joinSecret = secrets.JoinSecret;
		this.spectateSecret = secrets.SpectateSecret;
	}


	/// <summary>
	/// Is the secret object empty?
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty()
	{
		return string.IsNullOrEmpty(joinSecret) && string.IsNullOrEmpty(spectateSecret);
	}

	/// <summary>
	/// Converts this object into the DiscordRPC equivilent.
	/// </summary>
	/// <returns></returns>
	public DiscordRPC.Secrets ToRichSecrets()
	{
		if (IsEmpty()) return null;
		return new DiscordRPC.Secrets()
		{
			JoinSecret = joinSecret,
			SpectateSecret = spectateSecret
		};
	}
}
