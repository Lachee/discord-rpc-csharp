using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Exceptions
{
    /// <summary>
    /// A StringOutOfRangeException is thrown when the length of a string exceeds the allowed limit.
    /// </summary>
    public class StringOutOfRangeException : Exception
	{
		internal StringOutOfRangeException(string message) : base(message) { }
        internal StringOutOfRangeException(int max) : this("String", max) { }
        internal StringOutOfRangeException(string argument, int max) : this(string.Format("{0} is too long. Expected a maximum length of {1}", argument, max)) { }

        internal StringOutOfRangeException(int size, int max) : this("String", size, max) { }
        internal StringOutOfRangeException(string argument, int size, int max) : this(string.Format("{0} is too long. Expected a maximum length of {1} but got {2}.", argument, size, max)) { }
	}
}
