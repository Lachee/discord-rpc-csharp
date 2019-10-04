using DiscordRPC.Converters;
using System;
using System.Runtime.Serialization;

namespace DiscordRPC.RPC.Payload
{
    /// <summary>
    /// See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-events for documentation
    /// </summary>
    internal enum ServerEvent
	{

		/// <summary>
		/// Sent when the server is ready to accept messages
		/// </summary>
		[EnumValue("READY")]
		Ready,

		/// <summary>
		/// Sent when something bad has happened
		/// </summary>
		[EnumValue("ERROR")]
		Error,

		/// <summary>
		/// Join Event 
		/// </summary>
		[EnumValue("ACTIVITY_JOIN")]
		ActivityJoin,

		/// <summary>
		/// Spectate Event
		/// </summary>
		[EnumValue("ACTIVITY_SPECTATE")]
		ActivitySpectate,

		/// <summary>
		/// Request Event
		/// </summary>
		[EnumValue("ACTIVITY_JOIN_REQUEST")]
		ActivityJoinRequest,

#if INCLUDE_FULL_RPC
        //Old things that are obsolete
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("GUILD_STATUS")]
        GuildStatus,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("GUILD_CREATE")]
        GuildCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("CHANNEL_CREATE")]
        ChannelCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_CHANNEL_SELECT")]
        VoiceChannelSelect,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_STATE_CREATED")]
        VoiceStateCreated,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_STATE_UPDATED")]
        VoiceStateUpdated,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_STATE_DELETE")]
        VoiceStateDelete,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_SETTINGS_UPDATE")]
        VoiceSettingsUpdate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("VOICE_CONNECTION_STATUS")]
        VoiceConnectionStatus,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("SPEAKING_START")]
        SpeakingStart,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("SPEAKING_STOP")]
        SpeakingStop,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("MESSAGE_CREATE")]
        MessageCreate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("MESSAGE_UPDATE")]
        MessageUpdate,
		[Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("MESSAGE_DELETE")]
        MessageDelete,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("NOTIFICATION_CREATE")]
        NotificationCreate,
        [Obsolete("This value is appart of the RPC API and is not supported by this library.", true)]
        [EnumValue("CAPTURE_SHORTCUT_CHANGE")]
        CaptureShortcutChange
#endif
    }
}
