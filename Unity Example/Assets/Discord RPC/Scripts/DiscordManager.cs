using DiscordRPC;
using DiscordRPC.IO;
using DiscordRPC.Message;
using UnityEngine;

[ExecuteInEditMode]
public class DiscordManager : MonoBehaviour {

	public static DiscordManager instance { get { return _instance; } }
	private static DiscordManager _instance;

	#region Properties and Configurations
	[Tooltip("The ID of the Discord Application. Visit the Discord API to create a new application if nessary.")]
	public string applicationID = "424087019149328395";

	[Tooltip("The Steam App ID. This is a optional field used to launch your game through steam instead of the executable.")]
	public string steamID = "";

	[Tooltip("Registers a custom URI scheme for your game. This is required for the Join / Specate features to work.")]
	public bool registerUriScheme = false;

	[Tooltip("The pipe discord is located on. Useful for testing multiple clients.")]
	public DiscordPipe targetPipe = DiscordPipe.First;

	/// <summary>
	/// All possible pipes discord can be found on.
	/// </summary>
	public enum DiscordPipe
	{
		First = -1,
		Pipe0 = 0,
		Pipe1 = 1,
		Pipe2 = 2,
		Pipe3 = 3,
		Pipe4 = 4,
		Pipe5 = 5,
		Pipe6 = 6,
		Pipe7 = 7,
		Pipe8 = 8,
		Pipe9 = 9
	}

	[Tooltip("Logging level of the Discord IPC connection.")]
	public DiscordRPC.Logging.LogLevel logLevel = DiscordRPC.Logging.LogLevel.Warning;

	/// <summary>
	/// The current presence displayed on the Discord Client.
	/// </summary>
	public DiscordPresence CurrentPresence { get { return _currentPresence; } }
	[Tooltip("The current Rich Presence displayed on the Discord Client.")]
	[SerializeField] private DiscordPresence _currentPresence;
	
	[SerializeField]
	[Tooltip("The enabled state of the IPC connection")]
	private bool active = true;
	#endregion


	public DiscordEvents events;

	/// <summary>
	/// The current Discord Client.
	/// </summary>
	public DiscordRpcClient client { get { return _client; } }
	private DiscordRpcClient _client;
	
	#region Unity Events
	private void OnEnable()
	{
		//Make sure we are allowed to be active.
		if (!active) return;
		if (!Application.isPlaying) return;

		//This has a instance already that isn't us
		if (_instance != null && _instance != this)
		{
			Destroy(this);
			return;
		}

		//Assign the instance
		_instance = this;
		DontDestroyOnLoad(this);

		//We are starting the client. Below is a break down of the parameters.
		Debug.Log("Starting Discord Rich Presence");
		_client = new DiscordRpcClient(
			applicationID,									//The Discord Application ID
			steamID,										//The Steam App. This can be null or empty string to disable steam intergration.
			registerUriScheme,								//Should the client register a custom URI Scheme? This must be true for endpoints
			(int )targetPipe,								//The target pipe to connect too
			new NativeNamedPipeClient()                     //The client for the pipe to use. Unity MUST use a NativeNamedPipeClient since its managed client is broken.
		);

		//Update the logger to the unity logger
		_client.Logger = new UnityLogger() { Level = logLevel };

		//Subscribe to some initial events
		client.OnReady += ClientOnReady;
		client.OnError += ClientOnError;
		client.OnPresenceUpdate += ClientOnPresenceUpdate;

		events.RegisterEvents(client);

		//Start the client
		_client.Initialize();
		Debug.Log("Discord Rich Presence intialized and connecting...");

	}

	private void OnDisable()
	{
		Debug.Log("Disconnecting from Discord IPC...");
		if (_client != null)
		{
			Debug.Log("Disposing Discord IPC Client...");
			_client.Dispose();
			_client = null;
		}

		Debug.Log("Finished Disconnecting");
	}

	private void Update()
	{
		if (client == null || client.Disposed) return;

		//Invoke the client events
		client.Invoke();
	}
	#endregion




	private void ClientOnReady(object sender, ReadyMessage args)
	{
		//We have connected to the Discord IPC. We should send our rich presence just incase it lost it.
		Debug.Log("Connection established and received READY from Discord IPC. Sending our previous Rich Presence.");
		client.SetPresence((RichPresence) _currentPresence);
	}
	
	private void ClientOnPresenceUpdate(object sender, PresenceMessage args)
	{
		//Our Rich Presence has updated, better update our reference
		Debug.Log("Our Rich Presence has been updated. Applied changes to local store.");
		_currentPresence = (DiscordPresence) args.Presence;
	}
	
	private void ClientOnError(object sender, ErrorMessage args)
	{
		//Something bad happened while we tried to send a event. We will just log this for clarity.
		Debug.LogError("Error Occured within the Discord IPC: (" + args.Code + ") " + args.Message);
	}
}
