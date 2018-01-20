using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.RPC
{
	/// <summary>
	/// See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-events for documentation
	/// </summary>
	enum SubscriptionEvent
	{
		Ready,
		Error,
		GuildStatus,
		GuildCreate,
		ChannelCreate,
		VoiceChannelSelect,
		VoiceStateCreated,
		VoiceStateUpdated,
		VoiceStateDelete,
		VoiceSettingsUpdate,
		VoiceConnectionStatus,
		SpeakingStart,
		SpeakingStop,
		MessageCreate,
		MessageUpdate,
		MessageDelete,
		NotificationCreate,
		CaptureShortcutChange,

		//NEW EVENTS
		ActivityJoin,
		ActivitySpectate,
		ActivityJoinRequest
	}
}
