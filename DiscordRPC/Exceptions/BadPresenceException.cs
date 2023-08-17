﻿using System;

namespace DiscordRPC.Exceptions
{
    /// <summary>
    /// A BadPresenceException is thrown when invalid, incompatible or conflicting properties and is unable to be sent.
    /// </summary>
	public class BadPresenceException : Exception
    {
        internal BadPresenceException(string message) : base(message) { }
    }
}
