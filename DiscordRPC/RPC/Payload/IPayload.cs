using DiscordRPC.Converters;
using Newtonsoft.Json;

namespace DiscordRPC.RPC.Payload
{
    /// <summary>
    /// Base Payload that is received by both client and server
    /// </summary>
    internal abstract class IPayload
	{
		/// <summary>
		/// The type of payload
		/// </summary>
		[JsonProperty("cmd"), JsonConverter(typeof(EnumSnakeCaseConverter))]
		public Command Command { get; set; }

		/// <summary>
		/// A incremental value to help identify payloads
		/// </summary>
		[JsonProperty("nonce")]
		public string Nonce { get; set; }

		protected IPayload() { }
        protected IPayload(long nonce)
		{
			Nonce = nonce.ToString();
		}

		public override string ToString()
		{
			return $"Payload || Command: {Command}, Nonce: {Nonce}";
		}
	}
}

