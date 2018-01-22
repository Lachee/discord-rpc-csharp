using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Example enum of all assets available
/// </summary>
public enum AvailableImages
{
	None,
	ExampleImage,
	Default
}

public class ExampleInterface : MonoBehaviour {

	//Prepare all the UI elements
	public InputField state, details, largeImage, partyMaxSize, partyID, gameLength;
	public Slider partySize;
	public Text partySizeText;

	//Create a new unity presence
	public UnityPresence presence = new UnityPresence();

	//An example on how a enum can be used to store the names of assets
	public AvailableImages smallImage = AvailableImages.Default;

	//Called when the Apply button is pressed
	public void Apply()
	{
		//Copy over all the values from the UI
		presence.details = details.text;					//This is the text under the "Playing Game" (Cpature The Hill)
		presence.state = state.text;						//This is the text next to the party size and under tails (In Game)
		
		/*
		 * I personally recommend using a enum for the assets keys and call the supplied
		 *	string.ToSnakeCase() function to convert them to a standardised formatting
		 */
		presence.assets.largeKey = largeImage.text;			         //This is the image code for the large square image. 
		if (smallImage != AvailableImages.None)
			presence.assets.smallKey = smallImage.ToSnakeCase();	//This is the image code for the small circle image.

		/*
		 * Sets a party. It is recommended to use a constructor instead of this method to keep consistency.
		 */
		presence.party.identifer = partyID.text;						//A unqiue ID for th party. Leave blank if the user isn't in a party
		presence.party.size = GetPartySize(presence.party.maxSize);		//The size of the current party (Must be 1 or greater)
		presence.party.maxSize = GetPartyMaxSize();                     //The max size of the party (must be greater than or equal to size)
		
		//presence.party = new UnityPresence.Party("", 0, 0);				//Empty Party
		//presence.party = new UnityPresence.Party("someid:", 360, 420);	//Partially Full Party

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
