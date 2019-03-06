using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Exceptions
{
    /// <summary>
    /// Thrown when an action is performed on a client that has not yet been initialized
    /// </summary>
    public class UninitializedException : Exception
    {
        /// <summary>
        /// Creates a new unintialized exception
        /// </summary>
        /// <param name="message"></param>
        internal UninitializedException(string message) : base(message) { }

        /// <summary>
        /// Creates a new uninitialized exception with default message.
        /// </summary>
        internal UninitializedException() : this("Cannot perform action because the client has not been initialized yet or has been deinitialized.") { }
    }
}
