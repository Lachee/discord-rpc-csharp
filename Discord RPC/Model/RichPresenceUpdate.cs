using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Model
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class RichPresenceUpdate : IPayload
	{
		private RichPresence _presence;

		public int PID { get; set; }

		public string State { get { return _presence.state; } }
		public string Details { get { return _presence.details; } }
		

	}
	
}
