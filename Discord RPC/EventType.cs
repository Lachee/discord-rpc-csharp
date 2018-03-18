using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// The type of event receieved by the RPC
	/// </summary>
	public enum EventType
	{
		/// <summary>
		/// Called when the Discord Client wishes to enter a game to spectate
		/// </summary>
		Spectate, 

		/// <summary>
		/// Called when the Discord Client wishes to enter a game to play.
		/// </summary>
		Join,

		/// <summary>
		/// Called when another Discord Client has requested permission to join this game.
		/// </summary>
		JoinRequest,

	}
}
