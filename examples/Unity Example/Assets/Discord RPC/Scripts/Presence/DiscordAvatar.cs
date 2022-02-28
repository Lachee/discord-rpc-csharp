using System;
using UnityEngine;
using System.Collections;
using System.IO;

[System.Obsolete("Avatar information is handled by the User object now")]
static class DiscordAvatar
{
	public delegate void AvatarDownloadCallback(DiscordRPC.User user, Texture2D avatar);

	/// <summary>
	/// The current location of the avatar caches
	/// </summary>
	[System.Obsolete]
	public static string Cache { get { return _cache; } set { _cache = value; } }
	private static string _cache = Application.dataPath + "Discord RPC/Cache/";

	/// <summary>
	/// The quality of JPEG encodings.
	/// </summary>
	public static int JpegQuality { get { return _jpegquality; } set { _jpegquality = value; } }
	private static int _jpegquality = 75;

	/// <summary>
	/// Starts a coroutine which gets the user avatar as a Texture2D. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// </summary>
	/// <param name="user">The user to retrieve the avatar form</param>
	/// <param name="reference">The MonoBehaviour which is starting the coroutine.</param>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="format">The format of the target texture that will be generated</param>
	/// <param name="isJPEG">Flag indicating if the avatar should be jpeg encoded, otherwise the PNG format will be used. Default is false</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	[System.Obsolete("Cast this object into a DiscordUser instead and use the functions provided.")]
	private static void GetAvatarTexture(this DiscordRPC.User user, MonoBehaviour reference,  DiscordRPC.User.AvatarSize size = DiscordRPC.User.AvatarSize.x128, TextureFormat format = TextureFormat.RGBA32, bool isJPEG = false, AvatarDownloadCallback callback = null)
	{
		reference.StartCoroutine(EnumerateAvatarTexture(user, size, format, isJPEG, callback));
	}

	/// <summary>
	/// Gets the user avatar as a Texture2D as a enumerator. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// </summary>
	/// <param name="user">The user to retrieve the avatar form</param>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="format">The format of the target texture that will be generated</param>
	/// <param name="isJPEG">Flag indicating if the avatar should be jpeg encoded, otherwise the PNG format will be used. Default is false</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns></returns>
	[System.Obsolete("Cast this object into a DiscordUser instead and use the functions provided.")]
	private static IEnumerator EnumerateAvatarTexture(this DiscordRPC.User user, DiscordRPC.User.AvatarSize size = DiscordRPC.User.AvatarSize.x128, TextureFormat format = TextureFormat.RGBA32, bool isJPEG = false, AvatarDownloadCallback callback = null)
	{
		if (string.IsNullOrEmpty(user.Avatar))
		{
			yield return GetDefaultAvatarTexture(user, size, callback);
		}
		else
		{

			//Generate the path name
			string filename = string.Format("{0}-{1}{2}.{3}", user.Discriminator, user.Avatar, size.ToString(), isJPEG ? "jpeg" : "png");
			string path = Path.Combine(_cache, filename);
			
			//The holder texture is null, so we should create new one
			Texture2D holder = new Texture2D((int)size, (int)size, format, false);

			//Check if the file exists
			if (File.Exists(path))
			{
				//Load the image
				var bytes = File.ReadAllBytes(path);
				holder.LoadImage(bytes);
			}
			else
			{

				using (WWW www = new WWW(user.GetAvatarURL(isJPEG ? DiscordRPC.User.AvatarFormat.JPEG : DiscordRPC.User.AvatarFormat.PNG, size)))
				{
					//Download the texture
					yield return www;

					//Update the holder
					www.LoadImageIntoTexture(holder);

					//Save the texture, making a cache for later
					if (!Directory.Exists(_cache)) Directory.CreateDirectory(_cache);
					var bytes = isJPEG ? holder.EncodeToJPG(JpegQuality) : holder.EncodeToPNG();
					File.WriteAllBytes(path, bytes);					
				}
			}

			//Execute the callback (if any)
			if (callback != null)
				callback.Invoke(user, holder);
		}
	}

	/// <summary>
	/// Gets the default avatar for the given user. Will check the cache first, and if none are available it will then download the default from discord.
	/// </summary>
	/// <param name="user">The user to retreive the default avatar from.</param>
	/// <param name="size">The size of the target avatar</param>
	/// <param name="callback">The callback that will be made when the picture finishes downloading.</param>
	/// <returns></returns>
	[System.Obsolete("Cast this object into a DiscordUser instead and use the functions provided.")]
	private static IEnumerator GetDefaultAvatarTexture(this DiscordRPC.User user, DiscordRPC.User.AvatarSize size = DiscordRPC.User.AvatarSize.x128, AvatarDownloadCallback callback = null)
	{
		int discrim = user.Discriminator % 5;

		string filename = string.Format("default-{0}{1}.png", discrim, size.ToString());
		string path = Path.Combine(_cache, filename);

		Texture2D texture;

		//Check if the file exists
		if (File.Exists(path))
		{
			//Load the image
			var bytes = File.ReadAllBytes(path);
			texture = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);
			texture.LoadImage(bytes);
		}
		else
		{
			string url = string.Format("https://{0}/embed/avatars/{1}.png?size={2}", user.CdnEndpoint, discrim, size);
			using (WWW www = new WWW(url))
			{
				//Download the texture
				yield return www;

				//Set the texture
				texture = www.texture;

				//Save the texture, making a cache for later
				if (!Directory.Exists(_cache)) Directory.CreateDirectory(_cache);
				var bytes = texture.EncodeToPNG();
				File.WriteAllBytes(path, bytes);
			}
		}

		//Execute the callback (if any)
		if (callback != null)
			callback.Invoke(user, texture);
	}
}
