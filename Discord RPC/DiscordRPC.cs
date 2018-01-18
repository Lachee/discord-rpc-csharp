using DiscordRPC.IO;
using DiscordRPC.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
		private RichPresence _queuedPresence;

		private long _nonce = 0;
		private int pid;

		public DiscordRPC()
		{
			connection = new ConnectionWindows();
			connection.Open();

			_updateTimer = new Timer(15 * 1000);
			_updateTimer.AutoReset = true;
			_updateTimer.Elapsed += OnTimerElapsed;
			_updateTimer.Start();

			pid = Process.GetCurrentProcess().Id;
		}
	

		private void OnTimerElapsed(object sender, ElapsedEventArgs e) { }
		public void UpdatePresence(RichPresence presence)
		{
			_queuedPresence = presence;
		
			if (_queuedPresence != null)
			{

				//Update the queued presence
				_currentPresence = _queuedPresence;
				_queuedPresence = null;

				//Create the command
				Command command = new Command()
				{
					Nonce = (_nonce++).ToString(),
					Action = "SET_ACTIVITY",
					Args = new RichPresenceUpdate(_currentPresence, pid)
				};
				
				//Get the JSON presence payload and push it
				string json = Newtonsoft.Json.JsonConvert.SerializeObject(command);

				byte[] bytes = Encoding.Unicode.GetBytes(json);
				if (!connection.Write(bytes))
				{
					Console.WriteLine("FAILURE!");

					//Failed to write, so chuck it back onto the queue
					_queuedPresence = _currentPresence;
				}
				else
				{
					Console.WriteLine("Success!");
				}
			}

		}

		public void Dispose()
		{
			connection.Dispose();
			_updateTimer.Dispose();
		}
	}
}
