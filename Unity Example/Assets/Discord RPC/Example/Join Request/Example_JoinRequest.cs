using DiscordRPC.Message;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordRPC.Examples.JoinRequest
{
	public class Example_JoinRequest : MonoBehaviour
	{
		public DiscordInviteUI prefabInviteUI;
		public Transform inviteHolder;

		private void Start()
		{
			DiscordManager.current.events.OnJoinRequest.AddListener(OnJoinRequest);
		}

		//This event is subscribed to the Discord Manager using the inspector
		public void OnJoinRequest(JoinRequestMessage message)
		{
            Debug.LogError("JOIN REQUEST");

			//Insantiate the UI element
			var invite = Instantiate(prefabInviteUI, inviteHolder);
			invite.SetMessage(message);
		}
	}
}
