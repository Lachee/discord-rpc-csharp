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

		private DiscordManager current;

		private void Start()
		{
			//Get the current instance of DiscordManager
			current = GameObject.Find("Discord Manager").GetComponent<DiscordManager>();


			//Update the values
			UpdateFields();

			//Register to a presence change
			current.events.OnPresenceUpdate.AddListener((message) =>
			{
				Debug.Log("Received a new presence! Current App: " + message.ApplicationID + ", " + message.Name);
				UpdateFields();
			});
		}

		public void SendPresence()
		{
			DiscordManager.UpdatePresence
			(
				details: inputDetails.text,
				state: inputState.text,						

				start: inputStartTime.isOn,
				endTime: Int32.Parse(inputEndTime.text), 

				largeKey: inputLargeKey.text, 
				largeText: inputLargeTooltip.text, 
				smallKey: inputSmallKey.text, 
				smallText: inputSmallTooltip.text
			);
			current.SetPresence(current.UnsavedPresence);
		}

		public void UpdateFields()
		{
			inputState.text = current.UnsavedPresence.state;
			inputDetails.text = current.UnsavedPresence.details;


			inputLargeTooltip.text = current.UnsavedPresence.largeAsset.tooltip;
			inputLargeKey.text = current.UnsavedPresence.largeAsset.image;

			inputSmallTooltip.text = current.UnsavedPresence.smallAsset.tooltip;
			inputSmallKey.text = current.UnsavedPresence.smallAsset.image;
		}
	}
}
