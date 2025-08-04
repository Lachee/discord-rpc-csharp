using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// Rich Presence activity type
	/// </summary>
	public enum ActivityType
	{
		// Streaming (1) and Custom (4) are not supported via RPC
		/// <summary>
		/// Playing status type. Displays as "Playing ..."
		/// </summary>
		Playing = 0,
		/// <summary>
		/// Listening status type. Displays as "Listening to ..."
		/// </summary>
		Listening = 2,
		/// <summary>
		/// Watching status type. Displays as "Watching ..."
		/// </summary>
		Watching = 3,
		/// <summary>
		/// Competing status type. Displays as "Competing in ..."
		/// </summary>
		Competing = 5
	}
}
