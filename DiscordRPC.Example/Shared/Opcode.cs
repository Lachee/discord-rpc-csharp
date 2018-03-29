using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Example.Shared
{
	enum Opcode : uint
	{
		Disconnected = 0x00,
		Connected = 0x01,

		UserNickUpdate = 0x10,
		UserDiscordUpdate = 0x11,
		RoomNameUpdate = 0x20,
		RoomSecretUpdate = 0x21,
		RoomChange = 0x22,

		Error = 0x30,
		Message = 0x31,
	}
}
