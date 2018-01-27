using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC
{
	enum Command
	{
		/// <summary>
		/// event dispatch
		/// </summary>
		Dispatch,

		/// <summary>
		/// Called to set the activity
		/// </summary>
		SetActivity,

		/// <summary>
		/// used to authorize a new client with your app
		/// </summary>
		Authorize,

		/// <summary>
		/// 	used to authenticate an existing client with your app
		/// </summary>
		Authenticate,

		/// <summary>
		/// 	used to retrieve guild information from the client
		/// </summary>
		GetGuild,

		/// <summary>
		/// used to retrieve a list of guilds from the client
		/// </summary>
		GetGuilds,

		/// <summary>
		/// 	used to retrieve channel information from the client
		/// </summary>
		GetChannel,

		/// <summary>
		/// used to retrieve a list of channels for a guild from the client
		/// </summary>
		GetChannels,

		/// <summary>
		/// used to subscribe to an RPC event
		/// </summary>
		Subscribe,

		/// <summary>
		/// used to unsubscribe from an RPC event
		/// </summary>
		Unsubscribe,

		/// <summary>
		/// used to change voice settings of users in voice channels
		/// </summary>
		SetUserVoiceSettings,

		/// <summary>
		/// used to join or leave a voice channel, group dm, or dm
		/// </summary>
		SelectVoiceChannel,

		/// <summary>
		/// used to get the current voice channel the client is in
		/// </summary>
		GetSelectedVoiceChannel,

		/// <summary>
		/// used to join or leave a text channel, group dm, or dm
		/// </summary>
		SelectTextChannel,

		/// <summary>
		/// used to retrieve the client's voice settings
		/// </summary>
		GetVoiceSettings,

		/// <summary>
		/// used to set the client's voice settings
		/// </summary>
		SetVoiceSettings,

		/// <summary>
		/// used to capture a keyboard shortcut entered by the user RPC Events
		/// </summary>
		CaptureShortcut
	}
}
