namespace DiscordRPC.RPC.Messaging
{
    /// <summary>
    /// The error message received by discord. See https://discordapp.com/developers/docs/topics/rpc#rpc-server-payloads-rpc-errors for documentation
    /// </summary>
    public enum ErrorCode
    {
        // Pipe Error Codes
        /// <summary> Pipe was Successful </summary>
        Success = 0,

        ///<summary>The pipe had an exception</summary>
        PipeException = 1,

        ///<summary>The pipe received corrupted data</summary>
        ReadCorrupt = 2,

        // Custom Error Code
        ///<summary>The functionality was not yet implemented</summary>
        NotImplemented = 10,

        // Discord RPC error codes
        ///<summary>Unknown Discord error</summary>
        UnknownError = 1000,

        ///<summary>Invalid Payload received</summary>
        InvalidPayload = 4000,

        ///<summary>Invalid command was sent</summary>
        InvalidCommand = 4002,
		
        /// <summary>Invalid event was sent </summary>
        InvalidEvent = 4004,

        /*
        InvalidGuild = 4003,
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
        */
    }
}