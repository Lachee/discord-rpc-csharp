using DiscordRPC.Registry;
using DiscordRPC.RPC;
using System;

namespace DiscordRPC
{
	/// <summary>
	/// A Discord RPC Client which is used to send Rich Presence updates and receive Join / Spectate events.
	/// </summary>
	public class DiscordRpcClient : IDisposable
	{
		#region Properties
		/// <summary>
		/// Gets a value indicating if the RPC Client has registered a URI scheme. If this is false, Join / Spectate events will fail.
		/// </summary>
		public bool HasRegisteredUriScheme { get; private set; }

		/// <summary>
		/// Gets the Application ID of the RPC Client.
		/// </summary>
		public string ApplicationID { get; private set; }

		/// <summary>
		/// Gets the Steam ID of the RPC Client. This value can be null if none was supplied.
		/// </summary>
		public string SteamID { get; private set; }

		/// <summary>
		/// Gets the ID of the process used to run the RPC Client. Discord tracks this process ID and waits for its termination.
		/// </summary>
		public int ProcessID { get; private set; }
		#endregion

		private RpcConnectionPipe connectionPipe;
		private RpcConnection connection;

		#region Initialization

		/// <summary>
		/// Creates a new Discord RPC Client without using any uri scheme. This will disable the Join / Spectate functionality.
		/// </summary>
		/// <param name="applicationID"></param>
		public DiscordRpcClient(string applicationID) : this(applicationID, false) { }

		/// <summary>
		/// Creates a new Discord RPC Client using the default uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="registerUriScheme">Should a URI scheme be registered for Join / Spectate functionality? If false, the Join / Spectate functionality will be disabled.</param>
		public DiscordRpcClient(string applicationID, bool registerUriScheme) : this(applicationID, null, registerUriScheme) { }

		/// <summary>
		/// Creates a new Discord RPC Client using the steam uri scheme.
		/// </summary>
		/// <param name="applicationID">The ID of the application created at discord's developers portal.</param>
		/// <param name="steamID">The steam ID of the app. This is used to launch Join / Spectate through steam URI scheme instead of manual launching</param>
		/// <param name="registerUriScheme">Should a URI scheme be registered for Join / Spectate functionality? If false, the Join / Spectate functionality will be disabled.</param>
		public DiscordRpcClient(string applicationID, string steamID, bool registerUriScheme)
		{

			//Store our values
			ApplicationID = applicationID;
			SteamID = steamID;
			HasRegisteredUriScheme = registerUriScheme;
			ProcessID = System.Diagnostics.Process.GetCurrentProcess().Id;

			//If we are to register the URI scheme, do so.
			//The UriScheme.RegisterUriScheme function takes steamID as a optional parameter, its null by default.
			//   this means it will handle a null steamID for us :)
			if (registerUriScheme)
				UriScheme.RegisterUriScheme(applicationID, steamID);

			//Create the RPC client
			//connectionPipe = new RpcConnectionPipe(ApplicationID, ProcessID);
			connection = new RpcConnection(ApplicationID, ProcessID);
			connection.AttemptConnection();
		}

		#endregion


		/// <summary>
		/// Sets the Rich Presences
		/// </summary>
		/// <param name="presence">The rich presence to send to discord</param>
		public void SetPresence(RichPresence presence)
		{
			//connectionPipe.SetPresence(presence);
			connection.SetPresence(presence);
		}

		/// <summary>
		/// Clears the Rich Presence. Use this just before disposal to prevent ghosting.
		/// </summary>
		public void ClearPresence()
		{
			//connectionPipe.SetPresence(null, false);
			connection.SetPresence(null);
		}

		/// <summary>
		/// Causes the Rich Presence Connection to force a reconnection.
		/// </summary>
		public void Reconnect()
		{
			connection.Reconnect();
		}

		/// <summary>
		/// Disconnects the server, forcing a new connection to be established on next <see cref="SetPresence(RichPresence)"/>
		/// </summary>
		public void Close()
		{
			//connectionPipe.Close();
			connection.Close();
		}

		/// <summary>
		/// Terminates the connection to Discord and disposes of the object.
		/// </summary>
		public void Dispose()
		{
			//connectionPipe.Close();
			connection.Close();
		}

	}
}
