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

		/// <summary>
		/// The ID being used by the client
		/// </summary>
		public string ApplicationID { get { return _appid; } }
		private string _appid;

		/*
		public bool HasSteamID { get { return !string.IsNullOrEmpty(SteamID); } }
		public string SteamID { get { return _steamid; } }
		*/
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
		/// <summary>
		/// Called when an error occurs within the client
		/// </summary>
		public event DiscordErrorEvent OnError;

		/// <summary>
		/// General call used by the client to log messages. Generally used for debugging only and probably won't say anything useful.
		/// </summary>
		public static event Log OnLog;
		#endregion

		/// <summary>
		/// Creates a new Rich Presence client.
		/// </summary>
		/// <param name="applicationID">The Client ID supplied by the bot</param>
		public DiscordClient(string applicationID)
		{
			_appid = applicationID;
			//_steamid = steamID;
			PID = Process.GetCurrentProcess().Id;

			presenceQueue = new ConcurrentQueue<RichPresence>();

			/*
			//TODO: Implement this
			if (autoRegsiter)
			{
				if (HasSteamID)
					IO.Register.RegisterSteam(ApplicationID, SteamID);
				else
					IO.Register.RegisterApp(ApplicationID, null);
			}
			*/

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
            WriteLog("Processing Queue");

			//Make sure the RPC is connected
			if (!await CheckConnection())
            {
                WriteLog("Connection Failed");
                return;
            }

            //Loop until the queue is empty
            while (presenceQueue != null && !presenceQueue.IsEmpty)
            {

                //Try to get the element
                RichPresence next;
				if (!presenceQueue.TryDequeue(out next))
					continue;

                WriteLog("Sending Presence {0}", next != null ? next.State : "null (clear)");

                //Send it off
                RichPresence response = await rpc.WritePresenceAsync(next);
                if (next == null || response != null)
                {
                    WriteLog("Success, updating current presence.");
                    _currentPresence = response;
                }
                else
                {
                    WriteLog("Failure, continuing to next presence");
                }
            }

            WriteLog("Finish processing queue");
        }

        #region Helpers

        /// <summary>
        /// Gets the current presence that is being used by Discord
        /// </summary>
        /// <returns>The latest presence</returns>
        public RichPresence GetPresence()
		{
			return _currentPresence;
		}

		/// <summary>
		/// Attempts to make a connection to Discord and process the queued presence updates. Returns the presence current set.
		/// </summary>
		/// <returns>Returns the presence current set.</returns>
		public async Task<RichPresence> UpdatePresence()
		{
            WriteLog("Updating Presence");

			//Process the queue
			await ProcessPresenceQueue();

			//Send the current presence
			return _currentPresence;
		}

        /// <summary>
        /// Clears the current presence
        /// </summary>
        /// <returns></returns>
		public async Task ClearPresence() { await SetPresence(null);  }

		/// <summary>
		/// Sets the presence of the Discord client. Returns the presence current set and null if no connection was established.
		/// <para>
		/// The presence is queued and the queue is processed. If a connection could not be established, then the message will be sent on the next presence update. 
		/// To initiate a presence update, call either <see cref="SetPresence(RichPresence)"/> or <see cref="UpdatePresence"/>
		/// </para>
		/// </summary>
		/// <param name="presence">The presences to apply</param>
		/// <returns>Returns the presence current set and null if no connection was established.</returns>
		public async Task<RichPresence> SetPresence(RichPresence presence)
		{
            //Enqueue the status
            WriteLog("Setting Presence");
            presenceQueue.Enqueue(presence);

            //Check the status
            if (!await CheckConnection())
			{
				//We failed to connect, just queue the presence for now
				WriteLog("Connection failed, presence only enqueued. Manually call Update Presence.");
				return null;
			}
            
			//We are connected, so process the queue
			return await UpdatePresence();
		}

		private void IncrementReconnectDelay()
		{
			nextReconnectAttempt = runtime.ElapsedMilliseconds + reconnectDelay.NextDelay();
		}

		public void Dispose()
		{
			//Clear the presence
            //TODO: Should this be removed?
			//await ClearPresence();

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
