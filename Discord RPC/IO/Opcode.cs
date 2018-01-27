namespace DiscordRPC.IO
{
	internal enum Opcode
	{
		Handshake = 0,
		Frame = 1,
		Close = 2,
		Ping = 3,
		Pong = 4
	}
}
