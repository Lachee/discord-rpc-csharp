using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiscordInviteUI : MonoBehaviour
{
	public DiscordUser user;
	public RawImage avatar;
	public Text username;
	public Image ignore;

	public float ignoreTime = 15f;
	private float _ignoreStartTime = 0;
	private bool _incrementIgnoreTimer = false;

	public DiscordRPC.Message.JoinRequestMessage message;

	private void Start()
	{

		_ignoreStartTime = Time.time;
		_incrementIgnoreTimer = true;
	}

	public void SetMessage(DiscordRPC.Message.JoinRequestMessage message)
	{
		this.message = message;
		this.user = message.User;

		Debug.Log("Invite Received for "  + message.User);

		//Update the username
		username.text = message.User.Username + "#" + message.User.Discriminator;

		//Update the avatar
		//user.CacheAvatar(this, size: DiscordAvatarSize.x128, callback: (u, texture) =>	//Old depreciated way of doing this
		user.GetAvatar(this, size: DiscordAvatarSize.x128, callback: (u, texture) =>		//New way of doing this, with clearer function names
		{
			Debug.Log("Downloaded Texture for Invite");
			avatar.texture = texture;
		});

		//Set our ignore start time
		_ignoreStartTime = Time.time;
		_incrementIgnoreTimer = true;
	}

	private void Update()
	{
		if (!_incrementIgnoreTimer || !ignore) return;

		//Calc the fill
		float ignoreFill = (Time.time - _ignoreStartTime) / ignoreTime;

		//Set the fill
		ignore.fillAmount = Mathf.Clamp(ignoreFill, 0f, 1f);

		//If fill is greater than 110%, end
		if (ignoreFill > 1.1f)
		{
			OnResponsePressed(false);
			_incrementIgnoreTimer = false;
		}
	}

	public void OnResponsePressed(bool approved)
	{
		DiscordManager.instance.Respond(message, approved);
		Destroy(gameObject);
	}

}