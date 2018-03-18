using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Called when the Discord Client wishes for this process to spectate a game. D -> C. 
	/// </summary>
	public class SpectateMessage : JoinMessage
	{
		public override MessageType Type { get { return MessageType.Spectate; } }		
	}
}
