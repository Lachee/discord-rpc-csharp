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
        /// <summary>
        /// Maximum length the string is allowed to be.
        /// </summary>
        public int MaximumLength { get; private set; }

        /// <summary>
        /// Minimum length the string is allowed to be.
        /// </summary>
        public int MinimumLength { get; private set; }

        /// <summary>
        /// Creates a new string out of range exception with a range of min to max and a custom message
        /// </summary>
        /// <param name="message">The custom message</param>
        /// <param name="min">Minimum length the string can be</param>
        /// <param name="max">Maximum length the string can be</param>
        internal StringOutOfRangeException(string message,  int min, int max) : base(message)
        {
            MinimumLength = min;
            MaximumLength = max;
        }

        /// <summary>
        /// Creates a new sting out of range exception with a range of min to max
        /// </summary>
        /// <param name="minumum"></param>
        /// <param name="max"></param>
        internal StringOutOfRangeException(int minumum, int max) 
            : this($"Length of string is out of range. Expected a value between {minumum} and {max}", minumum, max) { }

        /// <summary>
        /// Creates a new sting out of range exception with a range of 0 to max
        /// </summary>
        /// <param name="max"></param>
        internal StringOutOfRangeException(int max)           
            : this($"Length of string is out of range. Expected a value with a maximum length of {max}", 0, max) { }
    }
}
