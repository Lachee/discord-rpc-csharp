﻿using UnityEngine;

[System.Serializable]
public class DiscordPresence
{
	[Header("Basic Details")]

	/// <summary>
	/// The details about the game. Appears underneath the game name
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The details about the game")]
	public string details = "Playing a game";

	/// <summary>
	/// The current state of the game (In Game, In Menu etc). Appears next to the party size
	/// </summary>
	[CharacterLimit(128)]
	[Tooltip("The current state of the game (In Game, In Menu). It appears next to the party size.")]
	public string state = "In Game";

	[Header("Time Details")]

	/// <summary>
	/// The time the game started. 0 if the game hasn't started
	/// </summary>
	[Tooltip("The time the game started. Leave as 0 if the game has not yet started.")]
	public DiscordTimestamp startTime = 0;

	/// <summary>
	/// The time the game will end in. 0 to ignore endtime.
	/// </summary>
	[Tooltip("Time the game will end. Leave as 0 to ignore it.")]
	public DiscordTimestamp endTime = 0;

	[Header("Presentation Details")]

	/// <summary>
	/// The images used for the presence.
	/// </summary>
	[Tooltip("The images used for the presence")]
	public DiscordAsset smallAsset;
	public DiscordAsset largeAsset;

	[Header("Button Details")]

	/// <summary>
	/// The buttons used for the presence.
	/// </summary>
	[Tooltip("The buttons used for the presence")]
	public DiscordButton[] buttons;

	[Header("Party Details")]

	/// <summary>
	/// The current party
	/// </summary>
	[Tooltip("The current party. Identifier must not be empty")]
	public DiscordParty party = new DiscordParty("", 0, 0);

	/// <summary>
	/// The current secrets for the join / spectate feature.
	/// </summary>
	[Tooltip("The current secrets for the join / spectate feature.")]
	public DiscordSecrets secrets = new DiscordSecrets();

	/// <summary>
	/// Creates a new Presence object
	/// </summary>
	public DiscordPresence() { }

	/// <summary>
	/// Creats a new Presence object, copying values of the Rich Presence
	/// </summary>
	/// <param name="presence">The rich presence, often received by discord.</param>
	public DiscordPresence(DiscordRPC.RichPresence presence)
	{
		if (presence != null)
		{
			this.state = presence.State;
			this.details = presence.Details;

			this.party = presence.HasParty() ? new DiscordParty(presence.Party) : new DiscordParty();
			this.secrets = presence.HasSecrets() ? new DiscordSecrets(presence.Secrets) : new DiscordSecrets();

			if (presence.HasAssets())
			{
				this.smallAsset = new DiscordAsset()
				{
					image = presence.Assets.SmallImageKey,
					tooltip = presence.Assets.SmallImageText,
					snowflake = presence.Assets.SmallImageID.GetValueOrDefault(0)
				};


				this.largeAsset = new DiscordAsset()
				{
					image = presence.Assets.LargeImageKey,
					tooltip = presence.Assets.LargeImageText,
					snowflake = presence.Assets.LargeImageID.GetValueOrDefault(0)
				};
			}
			else
			{
				this.smallAsset = new DiscordAsset();
				this.largeAsset = new DiscordAsset();
			}

			if (presence.HasButtons())
			{
				this.buttons = new DiscordButton[presence.Buttons.Length];

				for (int i = 0; i < presence.Buttons.Length; i++)
				{
					this.buttons[i] = new DiscordButton()
					{
						label = presence.Buttons[i].Label,
						url = presence.Buttons[i].Url
					};
				}
			}
			else
			{
				this.buttons = new DiscordButton[0];
			}


			if (presence.HasTimestamps())
			{
				//This could probably be made simpler
				this.startTime = presence.Timestamps.Start.HasValue ? new DiscordTimestamp((long)presence.Timestamps.StartUnixMilliseconds.Value) : DiscordTimestamp.Invalid;
				this.endTime = presence.Timestamps.End.HasValue ? new DiscordTimestamp((long)presence.Timestamps.EndUnixMilliseconds.Value) : DiscordTimestamp.Invalid;
			}
		}
		else
		{
			this.state = "";
			this.details = "";
			this.party = new DiscordParty();
			this.secrets = new DiscordSecrets();
			this.smallAsset = new DiscordAsset();
			this.largeAsset = new DiscordAsset();
			this.buttons = new DiscordButton[0];
			this.startTime = DiscordTimestamp.Invalid;
			this.endTime = DiscordTimestamp.Invalid;
		}

	}

	/// <summary>
	/// Converts this object into a new instance of a rich presence, ready to be sent to the discord client.
	/// </summary>
	/// <returns>A new instance of a rich presence, ready to be sent to the discord client.</returns>
	public DiscordRPC.RichPresence ToRichPresence()
	{
		var presence = new DiscordRPC.RichPresence();
		presence.State		= this.state;
		presence.Details	= this.details;
		
		presence.Party		= !this.party.IsEmpty() ? this.party.ToRichParty() : null;
		presence.Secrets	= !this.secrets.IsEmpty() ? this.secrets.ToRichSecrets() : null;

		if ((smallAsset != null && !smallAsset.IsEmpty()) || (largeAsset != null && !largeAsset.IsEmpty()))
		{
			presence.Assets = new DiscordRPC.Assets()
			{
				SmallImageKey = smallAsset.image,
				SmallImageText = smallAsset.tooltip,

				LargeImageKey = largeAsset.image,
				LargeImageText = largeAsset.tooltip
			};			
		}

		if (startTime.IsValid() || endTime.IsValid())
		{
			presence.Timestamps = new DiscordRPC.Timestamps();
			if (startTime.IsValid()) presence.Timestamps.Start = startTime.GetDateTime();
			if (endTime.IsValid()) presence.Timestamps.End = endTime.GetDateTime();
		}

		if (buttons.Length > 0)
		{
			presence.Buttons = new DiscordRPC.Button[buttons.Length];

			for (int i = 0; i < buttons.Length; i++)
			{
				presence.Buttons[i] = new DiscordRPC.Button
				{
					Label = buttons[i].label,
					Url = buttons[i].url
				};
			}
		}

		return presence;
	}

	public static explicit operator DiscordRPC.RichPresence(DiscordPresence presence)
	{
		return presence.ToRichPresence();
	}

	public static explicit operator DiscordPresence(DiscordRPC.RichPresence presence)
	{
		return new DiscordPresence(presence);
	}
}
