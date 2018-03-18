using Newtonsoft.Json;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Created when a error occurs within the ipc and it is sent to the client.
	/// </summary>
	public class ErrorMessage : IMessage
	{
		public override MessageType Type { get { return MessageType.Error; } }

		[JsonProperty("code")]
		public ErrorCode Code { get; internal set; }

		[JsonProperty("message")]
		public string Message { get; internal set; }

	}

	/// <summary>
	/// The error message received by discord. See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-errors for documentation
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
