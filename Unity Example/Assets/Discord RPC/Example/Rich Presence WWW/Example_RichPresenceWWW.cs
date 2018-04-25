using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiscordRPC.Examples.RichPresenceWWW
{
	public class Example_RichPresenceWWW : MonoBehaviour
	{
		public string applicationID = DiscordManager.EXAMPLE_APPLICATION;
		public DiscordPresence presence;

		[ContextMenu("Send Presence Data")]
		public void SendPresence()
		{
			Debug.Log("Sending Presence Data");
			this.StartCoroutine(DiscordManager.SendPresence(applicationID, presence));
		}
	}
}
