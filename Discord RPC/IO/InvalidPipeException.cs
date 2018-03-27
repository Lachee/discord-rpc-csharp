using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	public class InvalidPipeException : Exception
	{
		public InvalidPipeException(string message) : base(message) { }
	}
}
