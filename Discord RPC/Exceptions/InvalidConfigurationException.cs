using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Exceptions
{
	class InvalidConfigurationException : Exception
	{
		public InvalidConfigurationException(string message) : base(message) { }
	}
}
