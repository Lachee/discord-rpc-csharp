using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Example_RichPresence : MonoBehaviour {

	public InputField inputDetails, inputState;
	public Toggle inputStartTime;
	public InputField inputEndTime;

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
	}

	public void UpdatePresence()
	{
		presence.state = inputState.text;
		presence.details = inputDetails.text;
		presence.startTime = inputStartTime.isOn ? new DiscordTimestamp(Time.realtimeSinceStartup) : DiscordTimestamp.Invalid;

		float duration = float.Parse(inputEndTime.text);
		presence.endTime = duration > 0 ? new DiscordTimestamp(Time.realtimeSinceStartup + duration) : DiscordTimestamp.Invalid;
	}
}
