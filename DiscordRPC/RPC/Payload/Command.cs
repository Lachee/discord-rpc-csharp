using DiscordRPC.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payload
{
	/// <summary>
	/// The possible commands that can be sent and received by the server.
	/// </summary>
	internal enum Command
	{
		/// <summary>
		/// event dispatch
		/// </summary>
		[EnumValue("DISPATCH")]
		Dispatch,

		/// <summary>
		/// Called to set the activity
		/// </summary>
		[EnumValue("SET_ACTIVITY")]
		SetActivity,

		/// <summary>
		/// used to subscribe to an RPC event
		/// </summary>
		[EnumValue("SUBSCRIBE")]
		Subscribe,

		/// <summary>
		/// used to unsubscribe from an RPC event
		/// </summary>
		[EnumValue("UNSUBSCRIBE")]
		Unsubscribe,

		/// <summary>
		/// Used to accept join requests.
		/// </summary>
		[EnumValue("SEND_ACTIVITY_JOIN_INVITE")]
		SendActivityJoinInvite,

		/// <summary>
		/// Used to reject join requests.
		/// </summary>
		[EnumValue("CLOSE_ACTIVITY_JOIN_REQUEST")]
		CloseActivityJoinRequest,

		/// <summary>
		/// used to authorize a new client with your app
		/// </summary>
		[EnumValue("AUTHORIZE")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		Authorize,

		/// <summary>
		/// used to authenticate an existing client with your app
		/// </summary>
		[EnumValue("AUTHENTICATE")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		Authenticate,

		/// <summary>
		/// used to retrieve guild information from the client
		/// </summary>
		[EnumValue("GET_GUILD")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetGuild,

		/// <summary>
		/// used to retrieve a list of guilds from the client
		/// </summary>
		[EnumValue("GET_GUILDS")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetGuilds,

		/// <summary>
		/// used to retrieve channel information from the client
		/// </summary>
		[EnumValue("GET_CHANNEL")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetChannel,

		/// <summary>
		/// used to retrieve a list of channels for a guild from the client
		/// </summary>
		[EnumValue("GET_CHANNELS")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetChannels,


		/// <summary>
		/// used to change voice settings of users in voice channels
		/// </summary>
		[EnumValue("SET_USER_VOICE_SETTINGS")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		SetUserVoiceSettings,

		/// <summary>
		/// used to join or leave a voice channel, group dm, or dm
		/// </summary>
		[EnumValue("SELECT_VOICE_CHANNEL")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		SelectVoiceChannel,

		/// <summary>
		/// used to get the current voice channel the client is in
		/// </summary>
		[EnumValue("GET_SELECTED_VOICE_CHANNEL")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetSelectedVoiceChannel,

		/// <summary>
		/// used to join or leave a text channel, group dm, or dm
		/// </summary>
		[EnumValue("SELECT_TEXT_CHANNEL")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		SelectTextChannel,

		/// <summary>
		/// used to retrieve the client's voice settings
		/// </summary>
		[EnumValue("GET_VOICE_SETTINGS")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		GetVoiceSettings,

		/// <summary>
		/// used to set the client's voice settings
		/// </summary>
		[EnumValue("SET_VOICE_SETTINGS")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		SetVoiceSettings,

		/// <summary>
		/// used to capture a keyboard shortcut entered by the user RPC Events
		/// </summary>
		[EnumValue("CAPTURE_SHORTCUT")]
		[Obsolete("This value is appart of the RPC API and is not supported by this library.")]
		CaptureShortcut
	}
}
