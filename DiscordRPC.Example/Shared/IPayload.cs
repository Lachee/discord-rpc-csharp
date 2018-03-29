using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Example.Shared
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	interface IPayload
	{
		[JsonIgnore]
		Opcode OP { get; }

		string Serialize();
		void Deserialize(string json);
	}
}
