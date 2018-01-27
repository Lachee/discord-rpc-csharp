using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC
{
	/// <summary>
	/// See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-errors for documentation
	/// </summary>
	public enum ErrorCode
	{
		//Pipe Error Codes
		Success = 0,
		PipeException = 1,
		ReadCorrupt = 2,

		//Custom Error Code
		NotImplemented = 10,

		//Discord RPC error codes
		UnkownError = 1000,
		InvalidPayload = 4000,
		InvalidCommand = 4002,
		InvalidGuild = 4003,
		InvalidEvent = 4004,
		InvalidChannel = 4005,
		InvalidPermissions = 4006,
		InvalidClientID = 4007,
		InvalidOrigin = 4008,
		InvalidToken = 4009,
		InvalidUser = 4010,
		OAuth2Error = 5000,
		SelectChannelTimeout = 5001,
		GetGuildTimeout = 5002,
		SelectVoiceForceRequired = 5003,
		CaptureShortcutAlreadyListening = 5004
	}
}
