using DiscordRPC.RPC;
using DiscordRPC.Events;
using System;
using System.Diagnostics;
using DiscordRPC.RPC.Payloads;

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

			if (autoRegsiter)
			{
				if (HasSteamID)
					IO.Register.RegisterSteam(ApplicationID, SteamID);
				else
					IO.Register.RegisterApp(ApplicationID, null);
			}

			//Create the reconnext time
			reconnectDelay = new BackoffDelay(500, 60 * 1000);

			//create the connection
			rpc = new RpcConnection(_appid);
			rpc.OnConnect += OnRpcConnect;
			rpc.OnDisconnect += OnRpcDisconnect;

			//Create the runtime stopwatch
			runtime = new Stopwatch();
			runtime.Start();
		}

		public void UpdateConnection()
		{
			//No connection has been made, so we cannot update it
			if (rpc == null) return;

			//WriteLog("Updating Connection");

			//We are not open, so we have to try to open it instead
			if (!rpc.IsOpen)
			{
				WriteLog("We cannot send any information as RPC is not available");

				//Have we meet the current delay?
				if (runtime.ElapsedMilliseconds >= nextReconnectAttempt)
				{
					WriteLog("Attempt to connect to the RPC");

					//Reconnect to the RPC.
					IncrementReconnectDelay();
					rpc.AttemptConnection();
				}

				//Don't want to do anything else until its open
				return;
			}

			//Do some reading
			//ReadConnection();

			//Do some writing
			WriteConnection();
		}

		private void ReadConnection()
		{
			WriteLog("Attempting to read...");

			while (true)
			{
				WriteLog("Reading Content...");

				//Attempt to read the payload.
				ResponsePayload response;
				if (!rpc.ReadEvent(out response)) break;

				WriteLog("Payload: {0} {1}", response.Command, response.Event);

				if (!string.IsNullOrEmpty(response.Nonce))
				{
					WriteLog("Nonce found!");

					// Check if we have an error event
					if (response.Event == SubscriptionEvent.Error)
					{
						WriteLog("Error Found!");

						//We are an error! Makes me sad really ;-;
						PipeError close = response.Data as PipeError;
						if (close == null)
						{
							//We have had an error, but we have no idea how it works!?
							OnError?.Invoke(this, new DiscordErrorEventArgs()
							{
								ErrorCode = ErrorCode.UnkownError,
								Message = "An unkown error has occured with the RPC connection. The error code returned was not successfully parsed either!"
							});
						}
						else
						{
							//We need to tell the subscribers that we have come across an error
							OnError?.Invoke(this, new DiscordErrorEventArgs()
							{
								ErrorCode = close.Code,
								Message = close.Message
							});
						}
					}
					
					//Nothing else to do if we have a Nonce apparently
					return;
				}

				//Get the appropriate event
				switch(response.Event)
				{
					//An event we don't care about, ignore this.
					default:
						Console.WriteLine("Ignoring Event " + response.Event.ToString());
						break;

					case SubscriptionEvent.ActivityJoin:
					case SubscriptionEvent.ActivitySpectate:
					case SubscriptionEvent.ActivityJoinRequest:
						OnError?.Invoke(this, new DiscordErrorEventArgs()
						{
							ErrorCode = ErrorCode.NotImplemented,
							Message = response.Event.ToString() + " has not yet been implemented by this library."
						});
						break;
				}
			}
		}

		private void WriteConnection()
		{
			
		}

		#region Events

		private void OnRpcConnect(object sender, Events.RpcConnectEventArgs args)
		{
			//We have connected to the RPC, so reset our delay
			reconnectDelay.Reset();

			//Send the events for ACTIVITY_JOIN, ACTIVITY_SPECTATE and ACTIVITY_JOIN_REQUEST,
			// ONLY if we have handlers for those events.
			//TODO: Implement Join/Spectate systems.
			//src: https://github.com/discordapp/discord-rpc/blob/b85758ec19ab37a317662eb93d7208eaae129e84/src/discord-rpc.cpp#L285
			/*
			if (Handlers.joinGame) {
				RegisterForEvent("ACTIVITY_JOIN");
			}

			if (Handlers.spectateGame) {
				RegisterForEvent("ACTIVITY_SPECTATE");
			}

			if (Handlers.joinRequest) {
				RegisterForEvent("ACTIVITY_JOIN_REQUEST");
			}
			*/

		}

		private void OnRpcDisconnect(object sender, Events.RpcDisconnectEventArgs args)
		{
			//Update the disconnect timer because we failed to connect
			//Potential bug? This will end up incrementing the delay twice (one here, and one when we try to reconnect)
			IncrementReconnectDelay();
		}
		#endregion

		#region Helpers
		internal static void WriteLog(string format, params object[] objs)
		{
			OnLog?.Invoke(format, objs);
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
		#endregion
	}
}
