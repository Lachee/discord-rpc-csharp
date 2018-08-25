using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordRPC.Examples.RichPresence
{
	public class Example_ReadyListener : MonoBehaviour
	{

		public void OnReady(DiscordRPC.Message.ReadyMessage evt)
		{
			Debug.Log("Received Ready!");
			evt.User.GetAvatar(this, DiscordAvatarSize.x1024, (user, texture) =>
			{
				var renderer = GetComponent<Renderer>();
				renderer.material.mainTexture = texture;
			});
		}
	}
}