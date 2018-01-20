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
				rpc = new RpcConnection(_appid);
				rpc.OnConnect += async (s, a) => { reconnectDelay.Reset(); await ProcessPresenceQueue(); };
				rpc.OnDisconnect += (s, a) => IncrementReconnectDelay(); 
			}

			//Check if the RPC is open
			if (rpc.IsOpen) return true;

			//The RPC is not open, are we allowed to open it?
			if (runtime.ElapsedMilliseconds < nextReconnectAttempt) return false;

			//We are allowed to open it, we better open it then!
			DiscordClient.WriteLog("Attempting to connect to the APC");
			IncrementReconnectDelay();
			return await rpc.AttemptConnection();
		}

		/// <summary>
		/// Goes through all the presence updates that have been cached and sends them off.
		/// </summary>
		private async Task ProcessPresenceQueue()
		{
			//Make sure the RPC is connected
			if (!await CheckConnection()) return;

			//Loop until the queue is empty
			while (!presenceQueue.IsEmpty)
			{
				//Try to get the element
				RichPresence presence;
				if (!presenceQueue.TryDequeue(out presence)) continue;

				//Send it off
				await rpc.WriteCommandAsync(Command.SetActivity, new PresenceUpdate() { PID = this.PID, Presence = presence });
			}
		}

		#region Helpers
		
		public async void SetPresence(RichPresence presence)
		{
			if (!await CheckConnection())
			{
				//We failed to connect, just queue the presence for now
				presenceQueue.Enqueue(presence);
				return;
			}

			//We connected, now we just need to update
			await ProcessPresenceQueue();
		}

		private void IncrementReconnectDelay()
		{
			nextReconnectAttempt = runtime.ElapsedMilliseconds + reconnectDelay.NextDelay();
		}

		public void Dispose()
		{
			if (runtime != null)
			{
				runtime.Stop();
				runtime = null;
			}

			if (rpc != null)
			{
				rpc.Dispose();
				rpc = null;
			}
		}
		
		internal static void WriteLog(string format, params object[] objs)
		{
			OnLog?.Invoke(format, objs);
		}
		#endregion
	}
}
