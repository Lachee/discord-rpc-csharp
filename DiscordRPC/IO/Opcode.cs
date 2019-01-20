namespace DiscordRPC.IO
{
	/// <summary>
	/// The operation code that the <see cref="PipeFrame"/> was sent under. This defines the type of frame and the data to expect.
	/// </summary>
	public enum Opcode : uint
	{
		/// <summary>
		/// Initial handshake frame
		/// </summary>
		Handshake = 0,

		/// <summary>
		/// Generic message frame
		/// </summary>
		Frame = 1,

		/// <summary>
		/// Discord has closed the connection
		/// </summary>
		Close = 2,
		
		/// <summary>
		/// Ping frame (not used?)
		/// </summary>
		Ping = 3,

		/// <summary>
		/// Pong frame (not used?)
		/// </summary>
		Pong = 4
	}
}
