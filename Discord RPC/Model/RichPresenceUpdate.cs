using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Model
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class RichPresenceUpdate : RichPresence, IPayload
	{
		public int PID { get; set; }

		public RichPresenceUpdate(RichPresence p, int pid)
		{
			this.State = p.State;
			this.Details = p.Details;
			this.Timestamps = p.Timestamps;
			this.LargeImageKey = p.LargeImageKey;
			this.LargeImageText = p.LargeImageText;
			this.SmallImageKey = p.SmallImageKey;
			this.SmallImageText = p.SmallImageText;
			this.Party = p.Party;
			this.MatchSecret = p.MatchSecret;
			this.JoinSecret = p.JoinSecret;
			this.SpectateSecret = p.SpectateSecret;
			this.Instance = p.Instance;
			this.PID = pid;
		}
	}

}
