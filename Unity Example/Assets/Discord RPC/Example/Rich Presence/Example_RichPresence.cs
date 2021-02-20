using System;
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

		public DiscordManager discordManager;

		private void Start()
		{
			//Update the values
			UpdateFields();

			//Register to a presence change
			DiscordManager.current.events.OnPresenceUpdate.AddListener((message) =>
			{
				Debug.Log("Received a new presence! Current App: " + message.ApplicationID + ", " + message.Name);
				UpdateFields();
			});
		}

		public void SendPresence()
		{
			DiscordManager.UpdatePresence(discordManager, inputDetails.text, inputState.text, inputStartTime.isOn, false, Int32.Parse(inputEndTime.text), inputLargeKey.text, inputLargeTooltip.text, inputSmallKey.text, inputSmallTooltip.text);
			DiscordManager.current.SetPresence(discordManager.UnsavedPresence);
		}

		public void UpdateFields()
		{
			inputState.text = discordManager.UnsavedPresence.state;
			inputDetails.text = discordManager.UnsavedPresence.details;


			inputLargeTooltip.text = discordManager.UnsavedPresence.largeAsset.tooltip;
			inputLargeKey.text = discordManager.UnsavedPresence.largeAsset.image;

			inputSmallTooltip.text = discordManager.UnsavedPresence.smallAsset.tooltip;
			inputSmallKey.text = discordManager.UnsavedPresence.smallAsset.image;
		}
	}
}