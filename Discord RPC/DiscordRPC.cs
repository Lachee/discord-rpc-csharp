using DiscordRPC.RPC;
using DiscordRPC.Events;
using System;
using System.Diagnostics;
using DiscordRPC.RPC.Payloads;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace DiscordRPC
{
	public class DiscordClient : IDisposable
	{
		public delegate void Log(string formatting, params object[] objects);

		#region Properties
		public string ApplicationID { get { return _appid; } }
		private string _appid;

		public bool HasSteamID { get { return !string.IsNullOrEmpty(SteamID); } }
		public string SteamID { get { return _steamid; } }
		private string _steamid;
		#endregion

		#region Privates
		private RpcConnection rpc;
		private Stopwatch runtime;

		private BackoffDelay reconnectDelay;
		private long nextReconnectAttempt = 0;
		private int PID;

		private RichPresence _currentPresence;
		private ConcurrentQueue<RichPresence> presenceQueue;
		#endregion

		#region Events
		public event DiscordErrorEvent OnError;
		public static event Log OnLog;
		#endregion

		public DiscordClient(string applicationID, bool autoRegister = false) : this(applicationID, "", autoRegister) { }
		public DiscordClient(string applicationID, string steamID, bool autoRegsiter = false)
		{
			_appid = applicationID;
			_steamid = steamID;
			PID = Process.GetCurrentProcess().Id;

			presenceQueue = new ConcurrentQueue<RichPresence>();

			if (autoRegsiter)
			{
				if (HasSteamID)
					IO.Register.RegisterSteam(ApplicationID, SteamID);
				else
					IO.Register.RegisterApp(ApplicationID, null);
			}

			//Create the reconnext time
			reconnectDelay = new BackoffDelay(500, 60 * 1000);
			
			//Create the runtime stopwatch
			runtime = new Stopwatch();
			runtime.Start();
		}

		/// <summary>
		/// Checks the connection to the server and makes sure its valid. 
		/// </summary>
		/// <returns>Returns false if a connection could not be established</returns>
		private async Task<bool> CheckConnection()
		{
			if (rpc == null)
			{
				//create the connection. In the future this will have to autosubscribe to events too
				rpc = new RpcConnection(_appid, PID);
				rpc.OnDisconnect += (s, a) => IncrementReconnectDelay();
				rpc.OnError += (s, a) => this.OnError?.Invoke(this, a);
				rpc.OnConnect += async (s, a) => 
				{
					reconnectDelay.Reset();
					await ProcessPresenceQueue();
				};
			}

			//Check if the RPC is open
			if (rpc.IsOpen) return true;

			//The RPC is not open, are we allowed to open it?
			if (runtime.ElapsedMilliseconds < nextReconnectAttempt) return false;

			//We are allowed to open it, we better open it then!
			DiscordClient.WriteLog("Attempting to connect to the RPC");
			IncrementReconnectDelay();
			return await rpc.AttemptConnection();
		}

		/// <summary>
		/// Goes through all the presence updates that have been cached and sends them off.
		/// </summary>
		private async Task ProcessPresenceQueue()
		{
			//Make sure the RPC is connected
			if (!await CheckConnection())
				return;

			//Loop until the queue is empty
			while (!presenceQueue.IsEmpty)
			{
				//Try to get the element
				RichPresence next;
				if (!presenceQueue.TryDequeue(out next))
					continue;

				//Send it off
				RichPresence response = await rpc.WritePresenceAsync(next);
				if (response != null) _currentPresence = response;
			}
		}

		#region Helpers
		
		public RichPresence GetPresence()
		{
			return _currentPresence;
		}

		public async Task UpdatePresence() { await ProcessPresenceQueue(); }
		public async Task ClearPresence() { await SetPresence(null); }
		public async Task SetPresence(RichPresence presence)
		{
			//Check the status
			if (!await CheckConnection())
			{
				//We failed to connect, just queue the presence for now
				Console.WriteLine("Enquing Presence Update Instead");
				return;
			}

			//Enqueue the status
			presenceQueue.Enqueue(presence);
			
			//We are connected, so process the queue
			await ProcessPresenceQueue();
		}

		private void IncrementReconnectDelay()
		{
			nextReconnectAttempt = runtime.ElapsedMilliseconds + reconnectDelay.NextDelay();
		}

		public async void Dispose()
		{
			//Clear the presence
			await ClearPresence();

			//Stop the RPC socket
			if (rpc != null)
			{
				rpc.Dispose();
				rpc = null;
			}

			//Stop the running clock
			if (runtime != null)
			{
				runtime.Stop();
				runtime = null;
			}
		}

		internal static void WriteLog(string format, params object[] objs)
		{
			if (OnLog != null)
				OnLog(format, objs);
		}
		#endregion
	}
}
