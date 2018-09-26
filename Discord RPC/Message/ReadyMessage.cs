

using Newtonsoft.Json;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when the ipc is ready to send arguments.
	/// </summary>
	public class ReadyMessage : IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public override MessageType Type { get { return MessageType.Ready; } }
		
		/// <summary>
		/// The configuration of the connection
		/// </summary>
		[JsonProperty("config")]
		public Configuration Configuration { get; set; }

		/// <summary>
		/// User the connection belongs too
		/// </summary>
		[JsonProperty("user")]
		public User User { get; set; }

		/// <summary>
		/// The version of the RPC
		/// </summary>
		[JsonProperty("v")]
		public int Version { get; set; }
	}
}
