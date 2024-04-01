using DiscordRPC.RPC.Commands;
using System;
using System.Collections.Generic;
using System.Text;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Representation of the message received by discord when an authorization response has been received.
	/// </summary>
	public class AuthorizeMessage : IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public override MessageType Type { get { return MessageType.Authorize; } }

		internal AuthorizeMessage(AuthorizeResponse auth)
		{
			if (auth == null)
			{
				Code = "";
			}
			else
			{
				Code = auth.Code;
			}
		}

		/// <summary>
		/// The OAuth2 authorization code
		/// </summary>
		public string Code { get; internal set; }
	}
}
