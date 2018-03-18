using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// Object representing a Discord user. This is used for join requests.
	/// </summary>
	public class User
	{
		/// <summary>
		/// Possible formats for avatars
		/// </summary>
		public enum AvatarFormat
		{
			PNG,
			JPEG,
			WebP,
			GIF
		}

		/// <summary>
		/// Possible square sizes of avatars.
		/// </summary>
		public enum AvatarSize
		{
			x16,
			x32,
			x64,
			x128,
			x256,
			x512,
			x1024,
			x2048
		}

		/// <summary>
		/// The snowflake ID of the user requesting to join
		/// </summary>
		[JsonProperty("id")]
		public ulong ID { get; set; }

		/// <summary>
		/// The username of the user trying to join
		/// </summary>
		[JsonProperty("username")]
		public string Username { get; set; }

		/// <summary>
		/// The descriminator of the user trying to join
		/// </summary>
		[JsonProperty("discriminator")]
		public int Descriminator { get; set; }

		/// <summary>
		/// The avatar hash of the user trying to join. Too get a URI for the avatar, use the <see cref="GetAvatarURL(AvatarFormat)"/>. This can be null if the user has no avatar. The <see cref="GetAvatarURL(AvatarFormat)"/> will account for this and return the discord default.
		/// </summary>
		[JsonProperty("avatar")]
		public string Avatar { get; set; }

		/// <summary>
		/// The endpoint for the CDN
		/// </summary>
		internal string CdnEndpoint { get { return _cdn; } set { _cdn = value; } }
		private string _cdn = "cdn.discordapp.com";

		/// <summary>
		/// Gets a URL that can be used to download the user's avatar. If the user has not yet set their avatar, it will return the default one that discord is using. The default avatar only supports the <see cref="AvatarFormat.PNG"/> format.
		/// </summary>
		/// <param name="format">The format of the target avatar</param>
		/// <param name="size">The optional size of the avatar you wish for. Defaults to x128.</param>
		/// <returns></returns>
		public string GetAvatarURL(AvatarFormat format, AvatarSize size = AvatarSize.x128)
		{
			//Prepare the endpoint
			string endpoint = $"/avatars/{ID}/{Avatar}";

			//The user has no avatar, so we better replace it with the default
			if (string.IsNullOrEmpty(Avatar))
			{
				//Make sure we are only using PNG
				if (format != AvatarFormat.PNG)
					throw new Exception("The user has no avatar and the requested format " + format.ToString() + " is not supported. (Only supports PNG).");

				//Get the endpoint
				int descrim = Descriminator % 5;
				endpoint = $"/embed/avatars/{descrim}";
			}

			//Finish of the endpoint
			string s = size.ToString().Trim('x');
			return "https://" + CdnEndpoint + endpoint + GetAvatarExtension(format) + "?size=" + s;
		}

		/// <summary>
		/// Returns the file extension of the specified format
		/// </summary>
		/// <param name="format">The format to get the extention off</param>
		/// <returns>Returns a period prefixed file extension.</returns>
		public string GetAvatarExtension(AvatarFormat format)
		{
			return "." + format.ToString().ToLowerInvariant();
		}
	}
}
