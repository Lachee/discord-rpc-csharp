using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Exceptions
{
	public class BadPresenceException : Exception
	{
		public BadPresenceException(string message) : base(message) { }
	}
}
