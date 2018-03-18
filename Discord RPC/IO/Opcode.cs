namespace DiscordRPC.IO
{
	//TODO: Make Internal
	public enum Opcode : uint
	{
		Handshake = 0,
		Frame = 1,
		Close = 2,
		Ping = 3,
		Pong = 4
	}
}
