using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Logging
{
	/// <summary>
	/// Level of logging to use.
	/// </summary>
	public enum LogLevel
	{
        /// <summary>
        /// Trace, Info, Warning and Errors are logged
        /// </summary>
        Trace = 1,

        /// <summary>
        /// Info, Warning and Errors are logged
        /// </summary>
        Info = 2,

        /// <summary>
        /// Warning and Errors are logged
        /// </summary>
        Warning = 3,

        /// <summary>
        /// Only Errors are logged
        /// </summary>
        Error = 4,

		/// <summary>
		/// Nothing is logged
		/// </summary>
		None = 256
	}
}
