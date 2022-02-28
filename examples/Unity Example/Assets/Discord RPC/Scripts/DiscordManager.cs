﻿using DiscordRPC.Message;
using System.Collections;
using UnityEngine;

/// <summary>
/// A wrapper for the Discord Sharp Client, providing useful utilities in a Unity-Friendly form.
/// </summary>
[ExecuteInEditMode]
public class DiscordManager : MonoBehaviour
{

	public const string EXAMPLE_APPLICATION = "424087019149328395";

	[System.Obsolete("Use current instead")]
	public static DiscordManager instance { get { return _instance; } }

	/// <summary>
	/// The current instance of the Discord Manager
	/// </summary>
	public static DiscordManager current { get { return _instance; } }
	private static DiscordManager _instance;

	#region Properties and Configurations
	[Header("Properties")]
	[Tooltip("The ID of the Discord Application. Visit the Discord API to create a new application if nessary.")]
	public string applicationID = EXAMPLE_APPLICATION;

	[Tooltip("The Steam App ID. This is a optional field used to launch your game through steam instead of the executable.")]
	public string steamID = "";

	[Tooltip("The pipe discord is located on. Useful for testing multiple clients.")]
	public DiscordPipe targetPipe = DiscordPipe.FirstAvailable;


	/// <summary>
	/// All possible pipes discord can be found on.
	/// </summary>
	public enum DiscordPipe
	{
		FirstAvailable = -1,
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

	[Tooltip("Registers a custom URI scheme for your game. This is required for the Join / Specate features to work.")]
	public bool registerUriScheme = false;

	[SerializeField]
	[Tooltip("The enabled state of the IPC connection")]
	private bool active = true;

	/// <summary>
	/// The current Discord user. This does not get set until the first Ready event.
	/// </summary>
	public DiscordUser CurrentUser { get { return _currentUser; } }
	[Tooltip("The current Discord user. This does not get set until the first Ready event.")]

	[Header("State")]
	[SerializeField] private DiscordUser _currentUser;

	/// <summary>
	/// The current event subscription flag.
	/// </summary>
	public DiscordEvent CurrentSubscription { get { return _currentSubscription; } }
	[Tooltip("The current subscription flag")]
	[SerializeField] private DiscordEvent _currentSubscription = DiscordEvent.None;

	/// <summary>
	/// The current presence displayed on the Discord Client.
	/// </summary>
	public DiscordPresence CurrentPresence { get { return _currentPresence; } }
	[Tooltip("The current Rich Presence displayed on the Discord Client.")]
	[SerializeField] private DiscordPresence _currentPresence;

	#endregion

	[Header("Handlers and Events")]
	public DiscordRPC.Unity.DiscordEvents events = new DiscordRPC.Unity.DiscordEvents();

	/// <summary>
	/// The current Discord Client.
	/// </summary>
	public DiscordRPC.DiscordRpcClient client { get { return _client; } }
	private DiscordRPC.DiscordRpcClient _client = null;

	public bool isInitialized { get { return _client != null && _client.IsInitialized; } }

	#region Unity Events

	private void OnDisable() { Deinitialize(); }    //Try to dispose the client when we are disabled
	private void OnDestroy() { Deinitialize(); }

#if (UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE) && !DISABLE_DISCORD

    private void OnEnable() { if (gameObject.activeSelf && !isInitialized) Initialize(); }   //Try to initialize the client when we are enabled.


    //Try to initialize the client when we start. This is useful for moments where we are spawned in
    private void Start()
    {
        if (!isInitialized)
        {
            if (_client != null)
            {
                Debug.LogWarning("Client already exists! Disposing Early.");
                Deinitialize();
            }

            Initialize();
        }  
    }

    private void FixedUpdate()
	{
		if (client == null) return;

        //Update the client log level
        client.Logger.Level = logLevel;

		//Invoke the client events
		client.Invoke();
	}

#endif

#if UNITY_EDITOR
    [UnityEditor.MenuItem("GameObject/Discord Manager", priority = 10)]
    private static void CreateNewManager()
    {
        var prev = FindObjectOfType<DiscordManager>();
        if (prev == null)
        {
            var go = new GameObject("Discord Manager");
            prev = go.AddComponent<DiscordManager>();
        }
        else
        {
            Debug.LogWarning("Cannot create new Discord Manager because one already exists.");
        }

        UnityEditor.Selection.activeObject = prev;
    }
#endif

	#endregion

	/// <summary>
	/// Initializes the discord client if able to. Wont initialize if <see cref="active"/> is false, we are not in playmode, we already have a instance or we already have a client.
	/// <para>This function is empty unless UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE) && !DISABLE_DISCORD is meet.</para>
	/// </summary>
	public void Initialize()
	{
#if (UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE) && !DISABLE_DISCORD

        if (!active) return;                //Are we allowed to be active?
        if (!Application.isPlaying) return; //We are not allowed to initialize while in the editor.

        //This has a instance already that isn't us
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("[DAPI] Multiple DiscordManagers exist already. Destroying self.", _instance);
            Destroy(this);
            return;
        }

        //Make sure the client doesnt already exit
        if (_client != null)
        {
            Debug.LogError("[DAPI] Cannot initialize a new client when one is already initialized.");
            return;
        }

        //Assign the instance
        _instance = this;
        DontDestroyOnLoad(this);

        //Prepare the logger
        DiscordRPC.Logging.ILogger logger = null;

        //Update the logger to the unity logger
        if (Debug.isDebugBuild)         logger = new DiscordRPC.Logging.FileLogger("discordrpc.log") { Level = logLevel };
        if (Application.isEditor)       logger = new DiscordRPC.Unity.UnityLogger() { Level = logLevel };

        //We are starting the client. Below is a break down of the parameters.
        Debug.Log("[DRP] Starting Discord Rich Presence");
        _client = new DiscordRPC.DiscordRpcClient(
            applicationID,                                  //The Discord Application ID            
            pipe: (int)targetPipe,                          //The target pipe to connect too
            logger: logger,                                 //The logger,
            autoEvents: false,                              //WE will manually invoke events
            client: new DiscordRPC.Unity.UnityNamedPipe()   //The client for the pipe to use. Unity MUST use a NativeNamedPipeClient since its managed client is broken.
        );

        if (registerUriScheme)
            client.RegisterUriScheme(steamID);

        //Subscribe to some initial events
		#region Event Registration
        client.OnError += (s, args) => Debug.LogError("[DRP] Error Occured within the Discord IPC: (" + args.Code + ") " + args.Message);
        client.OnJoinRequested += (s, args) => Debug.Log("[DRP] Join Requested");

        client.OnReady += (s, args) =>
        {
            //We have connected to the Discord IPC. We should send our rich presence just incase it lost it.
            Debug.Log("[DRP] Connection established and received READY from Discord IPC. Sending our previous Rich Presence and Subscription.");


            //Set the user and cache their avatars
            _currentUser = args.User;
            _currentUser.GetAvatar(this, DiscordAvatarSize.x128);
        };
        client.OnPresenceUpdate += (s, args) =>
        {
            Debug.Log("[DRP] Our Rich Presence has been updated. Applied changes to local store.");
            Debug.Log(args.Presence.State);
            _currentPresence = (DiscordPresence)args.Presence;
        };
        client.OnSubscribe += (s, a) =>
        {
            Debug.Log("[DRP] New Subscription. Updating local store.");
            _currentSubscription = client.Subscription.ToUnity();
        };
        client.OnUnsubscribe += (s, a) =>
        {
            Debug.Log("[DRP] Removed Subscription. Updating local store.");
            _currentSubscription = client.Subscription.ToUnity();
        };
  
        //Register the unity events
        events.RegisterEvents(client);
		#endregion

        //Set initial presence and sub. (This will enqueue it)
        SetSubscription(_currentSubscription);
        SetPresence(_currentPresence);

        //Start the client
        _client.Initialize();
        Debug.Log("[DRP] Discord Rich Presence intialized and connecting...");

#endif
	}

	/// <summary>
	/// If not already disposed, it will dispose and deinitialize the discord client.
	/// </summary>
	public void Deinitialize()
	{
		//We dispose outside the scripting symbols as we always want to be able to dispose (just in case).
		if (_client != null)
		{
			Debug.Log("[DRP] Disposing Discord IPC Client...");
			_client.Dispose();
			_client = null;
			Debug.Log("[DRP] Finished Disconnecting");
		}
	}

	/// <summary>
	/// Sets the Rich Presence of the Discord Client through the pipe connection. 
	/// <para>This will log a error if the client is null or not yet initiated.</para>
	/// </summary>
	/// <param name="presence">The Rich Presence to be shown to the client</param>
	public void SetPresence(DiscordPresence presence)
	{
		if (client == null)
		{
			Debug.LogError("[DRP] Attempted to send a presence update but no client exists!");
			return;
		}

		if (!client.IsInitialized)
		{
			//Debug.LogWarning("[DRP] Attempted to send a presence update to a client that is not initialized! The messages will be enqueued instead!");
		}

		//Just do some validation
		if (!presence.secrets.IsEmpty() && _currentSubscription == DiscordEvent.None)
		{
			Debug.LogWarning("[DRP] Sending a secret, however we are not actually subscribed to any events. This will cause the messages to be ignored!");
		}

		//Set the presence
		_currentPresence = presence;
		client.SetPresence(presence != null ? presence.ToRichPresence() : null);
	}

	/// <summary>
	/// Resends the current Rich Presence to the Discord Client via the pipe connectoin.
	/// </summary>
	[ContextMenu("Resend Presence")]
	public void ResetPresence()
	{
		SetPresence(_currentPresence);
	}

	/// <summary>
	/// Sets the subscription flag, unsubscribing and then subscribing to the nessary events. Used for Join / Spectate feature. If you have not registered your application, this feature is unavailable.
	/// <para>This will log a error if the client is null or not yet initiated.</para>
	/// </summary>
	/// <param name="evt">The events to subscribe too</param>
	public void SetSubscription(DiscordEvent evt)
	{
		if (client == null)
		{
			Debug.LogError("[DRP] Attempted to send a presence update but no client exists!");
			return;
		}

		//if (!client.IsInitialized)
		//{
		//	Debug.LogError("[DRP] Attempted to send a presence update to a client that is not initialized!");
		//	return;
		//}

		this._currentSubscription = evt;
		client.SetSubscription(evt.ToDiscordRPC());
	}

	#region Single Components Sets
	public DiscordPresence UpdateDetails(string details)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateDetails(details);
	}

	public DiscordPresence UpdateState(string state)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateState(state);
	}

	public DiscordPresence UpdateParty(DiscordParty party)
	{
		if (_client == null) return null;
		if (party == null) return (DiscordPresence)_client.UpdateParty(null);
		return (DiscordPresence)_client.UpdateParty(party.ToRichParty());
	}
	public DiscordPresence UpdatePartySize(int size, int max)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdatePartySize(size, max);
	}

	public DiscordPresence UpdateLargeAsset(DiscordAsset asset)
	{
		if (_client == null) return null;
		if (asset == null) return (DiscordPresence)_client.UpdateLargeAsset("", "");
		return (DiscordPresence)_client.UpdateLargeAsset(asset.image, asset.tooltip);
	}
	public DiscordPresence UpdateSmallAsset(DiscordAsset asset)
	{
		if (_client == null) return null;
		if (asset == null) return (DiscordPresence)_client.UpdateSmallAsset("", "");
		return (DiscordPresence)_client.UpdateSmallAsset(asset.image, asset.tooltip);
	}

	public DiscordPresence UpdateSecrets(DiscordSecrets secrets)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateSecrets(secrets.ToRichSecrets());
	}

	public DiscordPresence UpdateStartTime()
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateStartTime();
	}
	public DiscordPresence UpdateStartTime(DiscordTimestamp timestamp)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateStartTime(timestamp.GetDateTime());
	}
	public DiscordPresence UpdateEndTime()
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateEndTime();
	}
	public DiscordPresence UpdateEndTime(DiscordTimestamp timestamp)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateEndTime(timestamp.GetDateTime());
	}
	public DiscordPresence UpdateClearTime(DiscordTimestamp timestamp)
	{
		if (_client == null) return null;
		return (DiscordPresence)_client.UpdateClearTime();
	}
	#endregion


	/// <summary>
	/// Resonds to a Join Request.
	/// </summary>
	/// <param name="request">The request being responded too</param>
	/// <param name="acceptRequest">The result of the request. True to accept the request.</param>
	public void Respond(JoinRequestMessage request, bool acceptRequest)
	{
		if (client == null)
		{
			Debug.LogError("[DRP] Attempted to send a presence update but no client exists!");
			return;
		}

		if (!client.IsInitialized)
		{
			Debug.LogError("[DRP] Attempted to send a presence update to a client that is not initialized!");
			return;
		}


		client.Respond(request, acceptRequest);
	}
}
