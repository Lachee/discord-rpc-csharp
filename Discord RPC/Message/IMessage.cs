using System;

namespace DiscordRPC.Message
{
	/// <summary>
	/// Messages received from discord.
	/// </summary>
	public abstract class IMessage
	{
		/// <summary>
		/// The type of message received from discord
		/// </summary>
		public abstract MessageType Type { get; }

		/// <summary>
		/// The time the message was created
		/// </summary>
		public DateTime TimeCreated { get { return _timecreated; } }
		private DateTime _timecreated;

		/// <summary>
		/// Creates a new instance of the message
		/// </summary>
		public IMessage()
		{
			_timecreated = DateTime.Now;
		}
	}
}
