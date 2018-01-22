using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleInterface : MonoBehaviour {

	//Prepare all the UI elements
	public InputField state, details, largeImage, smallImage, partyMaxSize, partyID, gameLength;
	public Slider partySize;
	public Text partySizeText;

	//Create a new unity presence
	public UnityPresence presence = new UnityPresence();

	//Called when the Apply button is pressed
	public void Apply()
	{
		//Copy over all the values from the UI
		presence.state = state.text;
		presence.details = details.text;

		presence.assets.largeKey = largeImage.text;
		presence.assets.smallKey = smallImage.text;

		presence.party.identifer = partyID.text;
		presence.party.maxSize = GetPartyMaxSize();
		presence.party.identifer = presence.party.maxSize > 0 ? GetHashCode().ToString() : "";
		presence.party.size = GetPartySize(presence.party.maxSize);

		//Example of setting the time. Any unit could be used.
		//presence.timestamps.start = new UnityPresence.Time();
		//presence.timestamps.start = System.DateTime.UtcNow;
		presence.startTime = Time.realtimeSinceStartup;

		//Set the time the game will end it (if it will end)
		if (!string.IsNullOrEmpty(gameLength.text)) 
			presence.endTime = Time.realtimeSinceStartup + float.Parse(gameLength.text);
		
		//Update the presence
		DiscordManager.SetPresence(presence);
		Debug.Log("Sent Presence!");
	}

	//Called when the Clear button is pressed
	public void Clear()
	{
		//Clear the presence
		DiscordManager.ClearPresence();
		Debug.Log("Cleared the presence!");
	}

	//The max party size
	private int GetPartyMaxSize() { return int.Parse(partyMaxSize.text); }

	//The party size from the sliders
	private int GetPartySize(int max)
	{
		return Mathf.RoundToInt(partySize.value * max);
	}

	//The slider moved, we better update the text
	public void UpdatePartySizeText()
	{
		partySizeText.text = GetPartySize(GetPartyMaxSize()) + " / ";
	}

}
