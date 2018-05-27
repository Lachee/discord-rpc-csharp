using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DiscordUser
{
	/// <summary>
	/// The current location of the avatar caches
	/// </summary>
	public static string CacheDirectory { get { return _cacheDirectory; } set { _cacheDirectory = value; } }
	private static string _cacheDirectory ="Discord RPC/Cache/";

	/// <summary>
	/// The format to download and cache avatars in. By default, PNG is used.
	/// </summary>
	public static DiscordAvatarFormat AvatarFormat { get; set; } = DiscordAvatarFormat.PNG;

	/// <summary>
	/// The username of the Discord user
	/// </summary>
	public string username { get { return _username; } }
	[SerializeField] private string _username;

	/// <summary>
	/// The discriminator of the Discord user
	/// </summary>
	public int discriminator { get { return _discriminator; } }
	[SerializeField] private int _discriminator;

	/// <summary>
	/// The discriminator in a nicely formatted string.
	/// </summary>
	public string discrim { get { return "#" + _discriminator.ToString("D4"); } }

	/// <summary>
	/// The unique snowflake ID of the Discord user
	/// </summary>
	public ulong ID { get { return _snowflake; } }
	[SerializeField] private ulong _snowflake;

	/// <summary>
	/// The hash of the users avatar. Used to generate the URL's
	/// </summary>
	public string avatarHash { get { return _avatarHash; } }
	[SerializeField] private string _avatarHash;

	/// <summary>
	/// The current avatar cache
	/// </summary>
	public Texture2D avatar {  get { return _avatar; } }
	[SerializeField] private Texture2D _avatar;

	/// <summary>
	/// The size of the currently cached avatar
	/// </summary>
	public DiscordAvatarSize cacheSize { get { return _cacheSize; } }
	[SerializeField] private DiscordAvatarSize _cacheSize;

	/// <summary>
	/// The format of the currently cached avatar
	/// </summary>
	public DiscordAvatarFormat cacheFormat { get { return _cacheFormat; } }
	[SerializeField] private DiscordAvatarFormat _cacheFormat;

	/// <summary>
	/// The current path of the cache image
	/// </summary>
	public string cachePath { get { return _cachePath; } }
	[SerializeField] private string _cachePath;

	/// <summary>
	/// The current URL for the discord avatars
	/// </summary>
	[SerializeField] private string _cdnEndpoint = "";

#if UNITY_EDITOR
#pragma warning disable 0414
	[HideInInspector]
	[SerializeField]
	private bool e_foldout = true;
#pragma warning restore 0414
#endif

	public DiscordUser()
	{
		_username = "Clyde";
		_discriminator = 1;
		_snowflake = 0;
		_avatarHash = "";
		_avatar = null;
		_cacheSize = DiscordAvatarSize.x128;
		_cacheFormat = DiscordUser.AvatarFormat;
		_cdnEndpoint = "cdn.discordapp.com";
	}

	public DiscordUser(DiscordRPC.User user)
	{
		_username = user.Username;
		_discriminator = user.Discriminator;
		_snowflake = user.ID;
		_avatarHash = user.Avatar;
		_cdnEndpoint = user.CdnEndpoint;
	}

	/// <summary>
	/// An event that is triggered when the avatar finishes downloading.
	/// </summary>
	/// <param name="user">The user the avatar belongs too</param>
	/// <param name="avatar">The avatar that was downloaded</param>
	public delegate void AvatarDownloadCallback(DiscordUser user, Texture2D avatar);
	
	/// <summary>
	/// Gets the user avatar as a Texture2D and starts it with the supplied monobehaviour. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// </summary>
	/// <param name="coroutineCaller">The target object that will start the coroutine</param>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns></returns>
	public void CacheAvatar(MonoBehaviour coroutineCaller, DiscordAvatarSize size = DiscordAvatarSize.x128,  AvatarDownloadCallback callback = null)
	{
		coroutineCaller.StartCoroutine(CacheAvatarCoroutine(size, callback));
	}  
	
	/// <summary>
	/// Gets the user avatar as a Texture2D as a enumerator. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// </summary>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns></returns>
	public IEnumerator CacheAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null)
	{
		if (string.IsNullOrEmpty(_avatarHash))
		{
			yield return CacheDefaultAvatarCoroutine(size, callback);
		}
		else
		{

			//Generate the path name
			string filename = string.Format("{0}-{1}{2}.{3}", discriminator, avatarHash, size.ToString(), DiscordUser.AvatarFormat.ToString().ToLowerInvariant());

			string cache = Path.Combine(Application.dataPath, CacheDirectory);
			string path = Path.Combine(cache, filename);

			//The holder texture is null, so we should create new one
			Texture2D avatarTexture = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);

			//Check if the file exists
			if (File.Exists(path))
			{
				//Load the image
				var bytes = File.ReadAllBytes(path);
				avatarTexture.LoadImage(bytes);
			}
			else
			{
				using (WWW www = new WWW(GetAvatarURL(DiscordUser.AvatarFormat, size)))
				{
					//Download the texture
					yield return www;

					//Update the holder
					www.LoadImageIntoTexture(avatarTexture);

					//Create the directory if it doesnt already exist
					if (!Directory.Exists(cache))
						Directory.CreateDirectory(cache);

					//Encode the image
					byte[] bytes;
					switch (DiscordUser.AvatarFormat)
					{
						default:
						case DiscordAvatarFormat.PNG:
							bytes = avatarTexture.EncodeToPNG();
							break;

						case DiscordAvatarFormat.JPEG:
							bytes = avatarTexture.EncodeToJPG();
							break;
					}

					//Save the image
					File.WriteAllBytes(path, bytes);
				}
			}

			//Apply our avatar and update our cache
			_avatar = avatarTexture;
			_cacheFormat = DiscordUser.AvatarFormat;
			_cachePath = path;
			_cacheSize = size;

			//Execute the callback (if any)
			if (callback != null)
				callback.Invoke(this, avatarTexture);
		}
	}

	/// <summary>
	/// Gets the default avatar for the given user. Will check the cache first, and if none are available it will then download the default from discord.
	/// </summary>
	/// <param name="size">The size of the target avatar</param>
	/// <param name="callback">The callback that will be made when the picture finishes downloading.</param>
	/// <returns></returns>
	public IEnumerator CacheDefaultAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null)
	{
		int discrim = discriminator % 5;

		string filename = string.Format("default-{0}{1}.png", discrim, size.ToString());
		string cache = Path.Combine(Application.dataPath, CacheDirectory);
		string path = Path.Combine(cache, filename);

		//The holder texture is null, so we should create new one
		Texture2D avatarTexture = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);

		//Check if the file exists
		if (File.Exists(path))
		{
			//Load the image
			byte[] bytes = File.ReadAllBytes(path);
			avatarTexture.LoadImage(bytes);
		}
		else
		{
			string url = string.Format("https://{0}/embed/avatars/{1}.png?size={2}", _cdnEndpoint, discrim, (int)size);
			using (WWW www = new WWW(url))
			{
				//Download the texture
				yield return www;

				//Update the holder
				www.LoadImageIntoTexture(avatarTexture);

				//Create the directory if it doesnt already exist
				if (!Directory.Exists(cache))
					Directory.CreateDirectory(cache);

				byte[] bytes = avatarTexture.EncodeToPNG();
				File.WriteAllBytes(path, bytes);
			}
		}

		//Apply our avatar and update our cache
		_avatar = avatarTexture;
		_cacheFormat = DiscordAvatarFormat.PNG;
		_cachePath = path;
		_cacheSize = size;
		
		//Execute the callback (if any)
		if (callback != null)
			callback.Invoke(this, _avatar);
	}

	private string GetAvatarURL(DiscordAvatarFormat format, DiscordAvatarSize size)
	{
		//Prepare the endpoint
		string endpoint = "/avatars/" + _snowflake + "/" + _avatarHash;

		//The user has no avatar, so we better replace it with the default
		if (string.IsNullOrEmpty(_avatarHash))
		{
			//Make sure we are only using PNG
			if (format != DiscordAvatarFormat.PNG)
				throw new System.BadImageFormatException("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");

			//Get the endpoint
			int descrim = _discriminator % 5;
			endpoint = "/embed/avatars/" + descrim;
		}

		//Finish of the endpoint
		return string.Format("https://{0}{1}.{2}?size={3}", this._cdnEndpoint, endpoint, format.ToString().ToLower(), (int)size);
	}

	public override string ToString() { return "@" + username  + discrim + " (" + _snowflake + ")"; }

	/// <summary>
	/// Implicit casting from a DiscordRPC.User to a DiscordUser
	/// </summary>
	/// <param name="user"></param>
	public static implicit operator DiscordUser(DiscordRPC.User user) { return new DiscordUser(user); }
}

/// <summary>
/// The format of the discord avatars in the cache
/// </summary>
public enum DiscordAvatarFormat
{
	/// <summary>
	/// Portable Network Graphics format (.png)
	/// <para>Losses format that supports transparent avatars. Most recommended for stationary formats with wide support from many libraries.</para>
	/// </summary>
	PNG,

	/// <summary>
	/// Joint Photographic Experts Group format (.jpeg)
	/// <para>The format most cameras use. Lossy and does not support transparent avatars.</para>
	/// </summary>
	JPEG
}

/// <summary>
/// Possible square sizes of avatars.
/// </summary>
public enum DiscordAvatarSize
{
	/// <summary> 16 x 16 pixels.</summary>
	x16 = 16,
	/// <summary> 32 x 32 pixels.</summary>
	x32 = 32,
	/// <summary> 64 x 64 pixels.</summary>
	x64 = 64,
	/// <summary> 128 x 128 pixels.</summary>
	x128 = 128,
	/// <summary> 256 x 256 pixels.</summary>
	x256 = 256,
	/// <summary> 512 x 512 pixels.</summary>
	x512 = 512,
	/// <summary> 1024 x 1024 pixels.</summary>
	x1024 = 1024,
	/// <summary> 2048 x 2048 pixels.</summary>
	x2048 = 2048
}