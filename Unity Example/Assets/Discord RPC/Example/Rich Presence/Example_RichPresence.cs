using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiscordRPC.Examples.RichPresence
{
	public class Example_RichPresence : MonoBehaviour
	{

		[Header("Details")]
		public InputField inputDetails, inputState;

		[Header("Time")]
		public Toggle inputStartTime;
		public InputField inputEndTime;

		[Header("Images")]
		public InputField inputLargeKey;
		public InputField inputLargeTooltip;
		public InputField inputSmallKey;
		public InputField inputSmallTooltip;


		public DiscordPresence presence;

		private void Start()
		{
			//Update the values
			UpdateFields(presence);

			//Register to a presence change
			DiscordManager.instance.events.OnPresenceUpdate.AddListener((message) =>
			{
				Debug.Log("Received a new presence! Current App: " + message.ApplicationID + ", " + message.Name);
				UpdateFields(new DiscordPresence(message.Presence));
			});
		}

		public void SendPresence()
		{
			UpdatePresence();
			DiscordManager.instance.SetPresence(presence);
		}

		public void UpdateFields(DiscordPresence presence)
		{
			this.presence = presence;
			inputState.text = presence.state;
			inputDetails.text = presence.details;


			inputLargeTooltip.text = presence.largeAsset.tooltip;
			inputLargeKey.text = presence.largeAsset.image;

			inputSmallTooltip.text = presence.smallAsset.tooltip;
			inputSmallKey.text = presence.smallAsset.image;
		}

		public void UpdatePresence()
		{
			presence.state = inputState.text;
			presence.details = inputDetails.text;
			presence.startTime = inputStartTime.isOn ? new DiscordTimestamp(Time.realtimeSinceStartup) : DiscordTimestamp.Invalid;

			presence.largeAsset = new DiscordAsset()
			{
				image = inputLargeKey.text,
				tooltip = inputLargeTooltip.text
			};
			presence.smallAsset = new DiscordAsset()
			{
				image = inputSmallKey.text,
				tooltip = inputSmallTooltip.text
			};

			float duration = float.Parse(inputEndTime.text);
			presence.endTime = duration > 0 ? new DiscordTimestamp(Time.realtimeSinceStartup + duration) : DiscordTimestamp.Invalid;
		}
	}
}