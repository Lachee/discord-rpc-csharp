using DiscordRPC.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Payload
{
	/// <summary>
	/// Used for Discord IPC Events
	/// </summary>
	internal class SubscriptionPayload : ArgumentPayload
	{
		/// <summary>
		/// The type of event the server sent
		/// </summary>
		[JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseConverter))]
		public ServerEvent? Event { get; set; }

        /// <summary>
        /// Creates a payload with empty data
        /// </summary>
		public SubscriptionPayload() : base() {  }

        /// <summary>
        /// Creates a payload with empty data and a set nonce
        /// </summary>
        /// <param name="nonce"></param>
		public SubscriptionPayload(long nonce) : base(nonce) {  }

		public SubscriptionPayload(object args, long nonce) : base(args, nonce) { }

		/// <summary>
		/// Converts the object into a human readable string
		/// </summary>
		/// <returns></returns>
		public override string ToString()
		{
			return "Subscription " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
		}
	}
	

}
