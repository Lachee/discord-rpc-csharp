using DiscordRPC.Exceptions;
using Newtonsoft.Json;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// Rich Presence Display type
	/// </summary>
	public enum StatusDisplayType
	{
		/// <summary>
		/// Displays the rich presence name "Listening to Spotify"
		/// </summary>
		Name = 0,
		/// <summary>
		/// Displays the rich presence state "Listening to Rick Astley"
		/// </summary>
		State = 1,
		/// <summary>
		/// Displays the rich presence details "Listening to Never Gonna Give You Up"
		/// </summary>
		Details = 2,
	}
}
