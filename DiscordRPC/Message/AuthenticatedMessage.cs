using DiscordRPC.Entities;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called as validation of a subscribe
	/// </summary>
	public class AuthenticatedMessage : IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public override MessageType Type { get { return MessageType.Authenticated; } }

		/// <summary>
		/// The current authorization
		/// </summary>
		[JsonIgnore]
		public Authorization Authorization { get; internal set; }

		/// <summary>
		/// The user that was authorized
		/// </summary>
		[JsonProperty("user")]
		public Entities.User User { get; private set; }

		/// <summary>
		/// The scopes that were authorized
		/// </summary>
		[JsonProperty("scopes")]
		public string[] Scopes { get; private set; }

		/// <summary>
		/// The time the access token will expire
		/// </summary>

		[JsonProperty("expires")]
		public DateTime Expires { get; private set; }

		/// <summary>
		/// The oAuth2 Application
		/// </summary>

		[JsonProperty("application")]
		public Application Application { get; private set; }
	}
}
