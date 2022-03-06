using Newtonsoft.Json;

namespace DiscordRPC.RPC.Payload
{
	internal class ClosePayload : IPayload
	{
		/// <summary>
		/// The close code the discord gave us
		/// </summary>
		[JsonProperty("code")]
		public int Code { get; set; }

		/// <summary>
		/// The close reason discord gave us
		/// </summary>
		[JsonProperty("message")]
		public string Reason { get; set; }
	}
}
