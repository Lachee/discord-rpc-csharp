namespace DiscordRPC.RPC
{
    /// <summary>
    /// State of the RPC connection
    /// </summary>
    internal enum RpcState
    {
        /// <summary>
        /// Disconnected from the discord client
        /// </summary>
        Disconnected,
		
        /// <summary>
        /// Connecting to the discord client. The handshake has been sent and we are awaiting the ready event
        /// </summary>
        Connecting,

        /// <summary>
        /// We are connect to the client and can send and receive messages.
        /// </summary>
        Connected
    }
}