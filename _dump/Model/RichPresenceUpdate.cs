using Newtonsoft.Json;
using DiscordRPC.RPC;

namespace DiscordRPC.Model
{
	[JsonObject(MemberSerialization = MemberSerialization.OptIn)]
	public class PresenceUpdate : RichPresence, CommandPayload
	{
		public int PID { get; set; }

		public PresenceUpdate(RichPresence p, int pid)
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
