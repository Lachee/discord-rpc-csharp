using UnityEngine;

[System.Serializable]
public class DiscordAssets
{
	/// <summary>
	/// The key of the image to be displayed in the large square.
	/// </summary>
	[Header("Large Asset")]
	[Tooltip("The key of the image to be displayed in the large square.")]
	public string largeKey;

	/// <summary>
	/// The tooltip of the large image.
	/// </summary>
	[Tooltip("The tooltip of the large image.")]
	public string largeTooltip;

	/// <summary>
	/// The key of the image to be displayed in the small circle.
	/// </summary>
	[Header("Small Asset")]
	[Tooltip("The key of the image to be displayed in the small circle.")]
	public string smallKey;

	/// <summary>
	/// The tooltip of the small image.
	/// </summary>
	[Tooltip("The tooltip of the small image.")]
	public string smallTooltip;

	/// <summary>
	/// Creates a new instances of the assets with empty values.
	/// </summary>
	public DiscordAssets() { }

	/// <summary>
	/// Creates new instances of the assets, using the <see cref="DiscordRPC.Assets"/> as the base.
	/// </summary>
	/// <param name="assets">The base to use the values from</param>
	public DiscordAssets(DiscordRPC.Assets assets)
	{
		this.largeKey = assets.LargeImageKey;
		this.smallKey = assets.SmallImageKey;
		this.largeTooltip = assets.LargeImageText;
		this.smallTooltip = assets.SmallImageText;
	}

	/// <summary>
	/// Converts this object into the DiscordRPC equivilent.
	/// </summary>
	/// <returns></returns>
	public DiscordRPC.Assets ToRichAssets()
	{
		var assets = new DiscordRPC.Assets()
		{
			LargeImageKey = largeKey,
			LargeImageText = largeTooltip,
			SmallImageKey = smallKey,
			SmallImageText = smallTooltip
		};

		return assets;
	}
}