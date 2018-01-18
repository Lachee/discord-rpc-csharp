using DiscordRPC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace DiscordRPC
{
	public class DiscordRPC : IDisposable
	{
		public RichPresence CurrentPresence { get { return _currentPresence; } }

		private IConnection connection;
		private Timer _updateTimer;


		private RichPresence _currentPresence;
		private RichPresence? _queuedPresence;

		public DiscordRPC()
		{
			connection = new ConnectionWindows();
			connection.Open();

			_updateTimer = new Timer(15 * 1000);
			_updateTimer.AutoReset = true;
			_updateTimer.Elapsed += OnTimerElapsed;
			_updateTimer.Start();
		}

		public void UpdatePresence(RichPresence presence)
		{
			_queuedPresence = presence;
		}

		private void OnTimerElapsed(object sender, ElapsedEventArgs e)
		{
			if (!_queuedPresence.HasValue) return;

			//Update the queued presence
			_currentPresence = _queuedPresence.Value;
			_queuedPresence = null;

			//Push the changes
			string presence = Newtonsoft.Json.JsonConvert.SerializeObject(_queuedPresence);
		}

		public void Dispose()
		{
			connection.Dispose();
			_updateTimer.Dispose();
		}
	}
}
