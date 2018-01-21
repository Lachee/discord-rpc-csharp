using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExampleInterface : MonoBehaviour {

	public UnityPresence presence = new UnityPresence();
	public InputField state, details, largeImage, smallImage, partyMaxSize;
	public Slider partySize;
	public Text partySizeText;

	private int GetPartyMaxSize() { return int.Parse(partyMaxSize.text); }
	private int GetPartySize(int max)
	{
		return Mathf.RoundToInt(partySize.value * max);
	}

	public void UpdatePartySizeText()
	{
		partySizeText.text = GetPartySize(GetPartyMaxSize()) + " / ";
	}

	public void Apply()
	{
		//Copy over all the values from the UI
		presence.state = state.text;
		presence.details = details.text;

		presence.assets.largeKey = largeImage.text;
		presence.assets.smallKey = smallImage.text;

		presence.party.maxSize = GetPartyMaxSize();
		presence.party.identifer = presence.party.maxSize > 0 ? GetHashCode().ToString() : "";
		presence.party.size = GetPartySize(presence.party.maxSize);
		
		//Update the presence
		DiscordManager.SetPresence(presence);
		Debug.Log("Sent Presence!");
	}

	public void Clear()
	{
		//Clear the presence
		DiscordManager.ClearPresence();
		Debug.Log("Cleared the presence!");
	}
}
