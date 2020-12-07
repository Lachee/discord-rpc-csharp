using DiscordRPC.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when some other person has requested access to this game. C -> D -> C.
	/// </summary>
	public class MessageCreateMessage : IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public override MessageType Type { get { return MessageType.MessageCreate; } }

		/// <summary>
		/// The discord user that is requesting access.
		/// </summary>
		[JsonProperty("channel_id")]
		public ulong ChannelId { get; internal set; }

		[JsonProperty("message")]
		public ChatMessage Message { get; internal set; }
	}
}
