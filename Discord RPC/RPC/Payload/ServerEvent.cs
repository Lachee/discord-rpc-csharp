using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC.Payload
{
	/// <summary>
	/// See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-events for documentation
	/// </summary>
	enum ServerEvent
	{
		/// <summary>
		/// Sent when the server is ready to accept messages
		/// </summary>
		Ready,

		/// <summary>
		/// Sent when something bad has happened
		/// </summary>
		Error,

		/// <summary>
		/// Join Event 
		/// </summary>
		ActivityJoin,

		/// <summary>
		/// Spectate Event
		/// </summary>
		ActivitySpectate,

		/// <summary>
		/// Request Event
		/// </summary>
		ActivityJoinRequest,

		#region RPC Protocols
		//Old things that are obsolete
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GuildStatus,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		GuildCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		ChannelCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceChannelSelect,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceStateCreated,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceStateUpdated,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceStateDelete,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceSettingsUpdate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		VoiceConnectionStatus,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SpeakingStart,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		SpeakingStop,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		MessageCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		MessageUpdate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		MessageDelete,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		NotificationCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
		CaptureShortcutChange
		#endregion
	}
}
