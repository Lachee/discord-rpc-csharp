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
		public int Maximum { get; private set; }

		/// <summary>
		/// The minimum time the backoff can start at
		/// </summary>
		public int Minimum { get; private set; }

		/// <summary>
		/// The current time of the backoff
		/// </summary>
		public int Current { get { return _current; } }
		private int _current;

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
		public BackoffDelay(int min, int max) : this(min, max, new Random()) { }
		public BackoffDelay(int min, int max, Random random)
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

		public int NextDelay()
		{
			//Increment the failures
			_fails++;

			double diff = (Maximum - Minimum) / 100f;
			_current = (int)Math.Floor(diff * _fails) + Minimum;


			return Math.Min(Math.Max(_current, Minimum), Maximum);
		}
	}
}
