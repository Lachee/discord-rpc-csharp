using UnityEngine;

[System.Serializable]
public class DiscordParty
{
	/// <summary> 
	/// A unique ID for the player's current party / lobby / group. If this is not supplied, they player will not be in a party and the rest of the information will not be sent. 
	/// <para>Max 128 Bytes</para>
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The unique ID of the party. Leave empty for no party.")]
	public string identifer;

	/// <summary>
	/// The size of the party.
	/// </summary>
	[Tooltip("The current size of the party.")]
	public int size;

	/// <summary>
	/// The max size of the party. Cannot be smaller than size.
	/// </summary>
	[Tooltip("The max size of the party.  Cannot be smaller than size.")]
	public int maxSize;

	/// <summary>
	/// Creates a new instance of the party
	/// </summary>
	/// <param name="id">ID of the party</param>
	/// <param name="size">Size of the party</param>
	/// <param name="max">Max Size of the party</param>
	public DiscordParty(string id, int size, int max)
	{
		this.identifer = id;
		this.size = size;
		this.maxSize = max;
	}

	/// <summary>
	/// Creates a new empty instance of the party
	/// </summary>
	public DiscordParty() { }

	/// <summary>
	/// Returns true if the party is not valid and has no ID.
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty()
	{
		return size <= 0 || maxSize <= 0 || string.IsNullOrEmpty(identifer);
	}

	/// <summary>
	/// Creates new instances of the party, using the <see cref="DiscordRPC.Party"/> as the base.
	/// </summary>
	/// <param name="party">The base to use the values from</param>
	public DiscordParty(DiscordRPC.Party party)
	{
		this.identifer = party.ID;
		this.size = party.Size;
		this.maxSize = party.Max;
	}

	/// <summary>
	/// Converts this object into the DiscordRPC equivilent.
	/// </summary>
	/// <returns></returns>
	public DiscordRPC.Party ToRichParty()
	{
		//We are not a valid party
		if (string.IsNullOrEmpty(identifer))
			return null;

		//return the party
		return new DiscordRPC.Party()
		{
			ID = identifer,
			Max = maxSize,
			Size = size
		};
	}

	/// <summary>
	/// Generates a random party identifier
	/// </summary>
	/// <returns></returns>
	public static string GenerateRandomIdentifer()
	{
		const string valid = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		const int chunkSize = 16;

		var builder = new System.Text.StringBuilder();
		for (int i = 0; i < 128; i++)
		{
			if (i > 0 && i % chunkSize == 0)
			{
				builder.Append("-");
			}
			else
			{
				char c = valid[Random.Range(0, valid.Length)];
				builder.Append(c);
			}
		}

		return builder.ToString();
	}
}