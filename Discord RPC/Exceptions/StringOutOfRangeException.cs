using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Exceptions
{
	class StringOutOfRangeException : Exception
	{
		public StringOutOfRangeException(string message) : base(message) { }
		public StringOutOfRangeException(int max) : this("String", max) { }
		public StringOutOfRangeException(string argument, int max) : this(string.Format("{0} is too long. Expected a maximum length of {1}", argument, max)) { }

		public StringOutOfRangeException(int size, int max) : this("String", size, max) { }
		public StringOutOfRangeException(string argument, int size, int max) : this(string.Format("{0} is too long. Expected a maximum length of {1} but got {2}.", argument, size, max)) { }
	}
}
