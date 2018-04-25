using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO
{
	/// <summary>
	/// The exception that is thrown when a error occurs while communicating with a pipe or when a connection attempt fails.
	/// </summary>
	public class InvalidPipeException : Exception
	{
		internal InvalidPipeException(string message) : base(message) { }
	}
}
