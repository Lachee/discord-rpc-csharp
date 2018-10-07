using System.Collections;
using System.IO;
using UnityEngine;

[System.Serializable]
public class DiscordUser
{
    /// <summary>
    /// Caching Level. This is a flag.
    /// </summary>
    [System.Flags]
    public enum CacheLevelFlag
    {
        /// <summary>Disable all caching</summary>
        None = 0,

        /// <summary>Caches avatars by user id (required for caching to work).</summary>
        UserId = 1,

        /// <summary>Caches the avatars by avatar hash. </summary>
        Hash = 3, //UserId | 2,

        /// <summary>Caches the avatars by size. If off, only the largest size is stored. </summary>
        Size = 5, //UserId | 4
    }

    /// <summary>
    /// The current location of the avatar caches
    /// </summary>
    public static string CacheDirectory = null;

    /// <summary>
    /// The caching level used by the avatar functions. Note that the cache is never cleared. The cache level will help mitigate exessive file counts.
    /// <para><see cref="CacheLevelFlag.None"/> will cause no images to be cached and will be downloaded everytime they are fetched.</para>
    /// <para><see cref="CacheLevelFlag.Hash"/> will cache images based of their hash. Without this, the avatar will likely stay the same forever.</para>
    /// <para><see cref="CacheLevelFlag.Size"/> will cache images based of their size. Useful, but may result in multiples of the same file. Disabling this will cause all files to be x512.</para>
    /// </summary>
    public static CacheLevelFlag CacheLevel = CacheLevelFlag.None;

	/// <summary>
	/// The format to download and cache avatars in. By default, PNG is used.
	/// </summary>
	public static DiscordAvatarFormat AvatarFormat { get; set; }

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
	/// The current avatar cache. Will return null until <see cref="GetAvatarCoroutine(DiscordAvatarSize, AvatarDownloadCallback)"/> is called.
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
	public void GetAvatar(MonoBehaviour coroutineCaller, DiscordAvatarSize size = DiscordAvatarSize.x128,  AvatarDownloadCallback callback = null)
	{
		coroutineCaller.StartCoroutine(GetAvatarCoroutine(size, callback));
	}

	/// <summary>
	/// Gets the user avatar as a Texture2D as a enumerator. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// <para>If <see cref="CacheLevel"/> has <see cref="CacheLevelFlag.Size"/> set, then the size will be ignored and <see cref="DiscordAvatarSize.x512"/> will be used instead.</para>
    /// <para>If <see cref="CacheLevel"/> is <see cref="CacheLevelFlag.None"/>, then no files will be written for cache.</para>
    /// </summary>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns></returns>
	public IEnumerator GetAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null)
	{
        if (_avatar != null)
        {
            //Execute the callback (if any)
            if (callback != null)
                callback.Invoke(this, _avatar);

            //Stop here, we did all we need to do
            yield break;
        }

        if (string.IsNullOrEmpty(_avatarHash))
		{
			yield return GetDefaultAvatarCoroutine(size, callback);
		}
		else
		{
            //Prepare the cache path
            string path = null;

            //Build the formatting
            if (CacheLevel != CacheLevelFlag.None)
            {
                //Update the default cache just incase its null
                SetupDefaultCacheDirectory();

                string format = "{0}";
                if ((CacheLevel & CacheLevelFlag.Hash) == CacheLevelFlag.Hash) format += "-{1}";
                if ((CacheLevel & CacheLevelFlag.Size) == CacheLevelFlag.Size) format += "{2}"; else size = DiscordAvatarSize.x512;

                //Generate the path name
                string filename = string.Format(format + ".{3}", ID, avatarHash, size.ToString(), DiscordUser.AvatarFormat.ToString().ToLowerInvariant());
                path = Path.Combine(CacheDirectory, filename);
                Debug.Log("<color=#FA0B0F>Cache:</color> " + path);
            }

			//The holder texture is null, so we should create new one
			Texture2D avatarTexture = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);

			//Check if the file exists and we have caching enabled
			if (CacheLevel != CacheLevelFlag.None && File.Exists(path))
            {
                Debug.Log("<color=#FA0B0F>Read Cache:</color> " + path);

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

                    //Encode and cache the files
                    if (CacheLevel != CacheLevelFlag.None)
                    {
                        //Create the directory if it doesnt already exist
                        if (!Directory.Exists(CacheDirectory))
                            Directory.CreateDirectory(CacheDirectory);

                        Debug.Log("<color=#FA0B0F>Saving Cache:</color> " + path);

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
			}

			//Apply our avatar and update our cache
			_avatar = avatarTexture;
			_cacheFormat = DiscordUser.AvatarFormat;
			_cacheSize = size;

			//Execute the callback (if any)
			if (callback != null)
				callback.Invoke(this, avatarTexture);
		}
	}

    /// <summary>
    /// Gets the default avatar for the given user. Will check the cache first, and if none are available it will then download the default from discord.	
    /// <para>If <see cref="CacheLevel"/> has <see cref="CacheLevelFlag.Size"/> set, then the size will be ignored and <see cref="DiscordAvatarSize.x512"/> will be used instead.</para>
    /// <para>If <see cref="CacheLevel"/> is <see cref="CacheLevelFlag.None"/>, then no files will be written for cache.</para>
    /// </summary>
    /// <param name="size">The size of the target avatar</param>
    /// <param name="callback">The callback that will be made when the picture finishes downloading.</param>
    /// 
    /// <returns></returns>
    public IEnumerator GetDefaultAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null)
	{
        //Calculate the discrim number and prepare the cache path
        int discrim = discriminator % 5;
        string path = null;

        //Update the default cache just incase its null
        if (CacheLevel != CacheLevelFlag.None)
        {
            //Setup the dir
            SetupDefaultCacheDirectory();

            //should we cache the size?
            bool cacheSize = (CacheLevel & CacheLevelFlag.Size) == CacheLevelFlag.Size;
            if (!cacheSize) size = DiscordAvatarSize.x512;

            string filename = string.Format("default-{0}{1}.png", discrim, cacheSize ? size.ToString() : "");
            path = Path.Combine(CacheDirectory, filename);
        }

		//The holder texture is null, so we should create new one
		Texture2D avatarTexture = new Texture2D((int)size, (int)size, TextureFormat.RGBA32, false);

		//Check if the file exists
		if (CacheLevel != CacheLevelFlag.None && File.Exists(path))
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

                //We have been told to cache, so do so.
                if (CacheLevel != CacheLevelFlag.None)
                {
                    //Create the directory if it doesnt already exist
                    if (!Directory.Exists(CacheDirectory))
                        Directory.CreateDirectory(CacheDirectory);

                    byte[] bytes = avatarTexture.EncodeToPNG();
                    File.WriteAllBytes(path, bytes);
                }
			}
		}

		//Apply our avatar and update our cache
		_avatar = avatarTexture;
		_cacheFormat = DiscordAvatarFormat.PNG;
		_cacheSize = size;
		
		//Execute the callback (if any)
		if (callback != null)
			callback.Invoke(this, _avatar);
	}
    
    /// <summary>
    /// Updates the default directory for the cache
    /// </summary>
    private static void SetupDefaultCacheDirectory()
    {
        if (CacheDirectory == null)
            CacheDirectory = Application.dataPath + "/Discord Rpc/Cache";
    }

    #region Obsolete Functions
    [System.Obsolete("Now known as GetAvatar instead.")]
	public void CacheAvatar(MonoBehaviour coroutineCaller, DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null) { GetAvatar(coroutineCaller, size, callback); }

	[System.Obsolete("Now known as GetAvatarCoroutine instead.")]
	public IEnumerator CacheAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null) { return GetAvatarCoroutine(size, callback); }

	[System.Obsolete("Now known as GetAGetDefaultAvatarCoroutinevatar instead.")]
	public IEnumerator CacheDefaultAvatarCoroutine(DiscordAvatarSize size = DiscordAvatarSize.x128, AvatarDownloadCallback callback = null) { return GetDefaultAvatarCoroutine(size, callback); }
	#endregion

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

/// <summary>
/// Collection of extensions to the <see cref="DiscordRPC.User"/> class.
/// </summary>
public static class DiscordUserExtension
{
	/// <summary>
	/// Gets the user avatar as a Texture2D and starts it with the supplied monobehaviour. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// <para>An alias of <see cref="DiscordUser.CacheAvatar(MonoBehaviour, DiscordAvatarSize, AvatarDownloadCallback)"/> and will return the new <see cref="DiscordUser"/> instance.</para>
	/// </summary>
	/// <param name="coroutineCaller">The target object that will start the coroutine</param>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns>Returns the generated <see cref="DiscordUser"/> for this <see cref="DiscordRPC.User"/> object.</returns>
	public static DiscordUser GetAvatar(this DiscordRPC.User user, MonoBehaviour coroutineCaller, DiscordAvatarSize size = DiscordAvatarSize.x128, DiscordUser.AvatarDownloadCallback callback = null)
	{
		var du = new DiscordUser(user);
		du.GetAvatar(coroutineCaller, size, callback);
		return du;
	}

	/// <summary>
	/// Gets the user avatar as a Texture2D as a enumerator. It will first check the cache if the image exists, if it does it will return the image. Otherwise it will download the image from Discord and store it in the cache, calling the callback once done.
	/// <para>An alias of <see cref="DiscordUser.CacheAvatarCoroutine(DiscordAvatarSize, DiscordUser.AvatarDownloadCallback)"/> and will return the new <see cref="DiscordUser"/> instance in the callback.</para>
	/// </summary>
	/// <param name="size">The target size of the avatar. Default is 128x128</param>
	/// <param name="callback">The callback for when the texture completes. Default is no-callback, but its highly recommended to use a callback</param>
	/// <returns></returns>
	public static IEnumerator GetAvatarCoroutine(this DiscordRPC.User user, DiscordAvatarSize size = DiscordAvatarSize.x128, DiscordUser.AvatarDownloadCallback callback = null)
	{
		var du = new DiscordUser(user);
		return du.GetDefaultAvatarCoroutine(size, callback);
	}

	/// <summary>
	/// Gets the default avatar for the given user. Will check the cache first, and if none are available it will then download the default from discord.
	/// <para>An alias of <see cref="DiscordUser.CacheDefaultAvatarCoroutine(DiscordAvatarSize, DiscordUser.AvatarDownloadCallback)"/> and will return the new <see cref="DiscordUser"/> instance in the callback.</para>
	/// </summary>
	/// <param name="size">The size of the target avatar</param>
	/// <param name="callback">The callback that will be made when the picture finishes downloading.</param>
	/// <returns></returns>
	public static IEnumerator GetDefaultAvatarCoroutine(this DiscordRPC.User user, DiscordAvatarSize size = DiscordAvatarSize.x128, DiscordUser.AvatarDownloadCallback callback = null)
	{
		var du = new DiscordUser(user);
		return du.GetDefaultAvatarCoroutine(size, callback);
	}


}
