using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Helper
{

	internal class BackoffDelay
	{
		/// <summary>
		/// The maximum time the backoff can reach
		/// </summary>
		public long Maximum { get; }

		/// <summary>
		/// The minimum time the backoff can start at
		/// </summary>
		public long Minimum { get; }

		/// <summary>
		/// The current time of the backoff
		/// </summary>
		public long Current { get { return _current; } }
		private long _current;

		/// <summary>
		/// The current number of failures
		/// </summary>
		public int Fails { get { return _fails; } }
		private int _fails;

		/// <summary>
		/// The random generator
		/// </summary>
		public Random Random { get; set; }

		private BackoffDelay() { }
		public BackoffDelay(long min, long max) : this(min, max, new Random()) { }
		public BackoffDelay(long min, long max, Random random)
		{
			this.Minimum = min;
			this.Maximum = max;

			this._current = min;
			this._fails = 0;
			this.Random = random;
		}

		/// <summary>
		/// Resets the backoff
		/// </summary>
		public void Reset()
		{
			_fails = 0;
			_current = Minimum;
		}

		public long NextDelay()
		{
			//Increment the failures
			_fails++;

			//Calculate the new delay
			long delay = (long)((double)_current * 2.0 * NextValue());

			//Update the current delay, maxing it out
			_current = Math.Min(_current + delay, Maximum);
			return _current;
		}

		private double NextValue()
		{
			double mantissa = (Random.NextDouble() * 2.0) - 1.0;
			double exponent = Math.Pow(2.0, Random.Next(-126, 128));
			return (mantissa * exponent);
		}
	}
}
