using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordRPC.Examples.JoinRequest
{
	public class InviteUI : MonoBehaviour
	{
		public DiscordUser user;
		public RawImage avatar;
		public Text username;

		public DiscordRPC.Message.JoinRequestMessage message;

		public void SetMessage(DiscordRPC.Message.JoinRequestMessage message)
		{
			this.message = message;
			this.user = message.User;

			Debug.Log("Invite Received for "  + message.User);

			//Update the username
			username.text = message.User.Username + "#" + message.User.Discriminator;

			//Update the avatar
			user.CacheAvatar(this, size: DiscordAvatarSize.x128, callback: (u, texture) =>
			{
				Debug.Log("Downloaded Texture for Invite");
				avatar.texture = texture;
			});
		}

		public void OnResponsePressed(bool approved)
		{
			DiscordManager.instance.Respond(message, approved);
			Destroy(gameObject);
		}

	}
}