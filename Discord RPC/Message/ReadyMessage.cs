

using Newtonsoft.Json;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when the ipc is ready to send arguments.
	/// </summary>
	public class ReadyMessage : IMessage
	{
		public override MessageType Type { get { return MessageType.Ready; } }
		
		[JsonProperty("config")]
		public Configuration Configuration { get; set; }

		[JsonProperty("v")]
		public int Version { get; set; }
	}
}
