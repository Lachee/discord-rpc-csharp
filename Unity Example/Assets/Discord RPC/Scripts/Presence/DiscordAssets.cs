using UnityEngine;

[System.Serializable]
public class DiscordAsset
{
	/// <summary>
	/// The key of the image to be displayed.
	/// <para>Max 32 Bytes.</para>
	/// </summary>
	[CharacterLimit(32)]
	[Tooltip("The key of the image to be displayed in the large square.")]
	public string image;

	/// <summary>
	/// The tooltip of the image.
	/// <para>Max 128 Bytes.</para>
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The tooltip of the large image.")]
	public string tooltip;

	[Tooltip("Snowflake ID of the image.")]
	public ulong snowflake;

	/// <summary>
	/// Is the asset object empty?
	/// </summary>
	/// <returns></returns>
	public bool IsEmpty()
	{
		return string.IsNullOrEmpty(image) && string.IsNullOrEmpty(tooltip);
	}
}