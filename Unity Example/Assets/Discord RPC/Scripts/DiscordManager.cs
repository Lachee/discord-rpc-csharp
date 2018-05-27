using DiscordRPC;
using DiscordRPC.IO;
using DiscordRPC.Message;
using System.Collections;
using UnityEngine;

/// <summary>
/// A wrapper for the Discord Sharp Client, providing useful utilities in a Unity-Friendly form.
/// </summary>
[ExecuteInEditMode]
public class DiscordManager : MonoBehaviour {

	public const string EXAMPLE_APPLICATION = "424087019149328395";

	public static DiscordManager instance { get { return _instance; } }
	private static DiscordManager _instance;

	#region Properties and Configurations
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
	[SerializeField] private DiscordUser _currentUser;

	/// <summary>
	/// The current event subscription flag.
	/// </summary>
	public DiscordEvent CurrentSubscription { get { return _currentSubscription; } }
	[Tooltip("The current subscription flag")]
	[SerializeField] private DiscordEvent _currentSubscription = DiscordEvent.Join | DiscordEvent.Spectate;

	/// <summary>
	/// The current presence displayed on the Discord Client.
	/// </summary>
	public DiscordPresence CurrentPresence { get { return _currentPresence; } }
	[Tooltip("The current Rich Presence displayed on the Discord Client.")]
	[SerializeField] private DiscordPresence _currentPresence;

	#endregion

	public DiscordEvents events;

	/// <summary>
	/// The current Discord Client.
	/// </summary>
	public DiscordRpcClient client { get { return _client; } }
	private DiscordRpcClient _client;
	
	#region Unity Events
#if (UNITY_WSA || UNITY_WSA_10_0 || UNITY_STANDALONE_WIN)
	private void OnEnable()
	{
		//Make sure we are allowed to be active.
		if (!active || !gameObject.activeSelf) return;
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
		Debug.Log("[DRP] Starting Discord Rich Presence");
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
		#region Event Registration
		client.OnReady += (s, args) =>
		{
			//We have connected to the Discord IPC. We should send our rich presence just incase it lost it.
			Debug.Log("[DRP] Connection established and received READY from Discord IPC. Sending our previous Rich Presence and Subscription.");

			//Set the user and cache their avatars
			_currentUser = args.User;
			_currentUser.CacheAvatar(this, DiscordAvatarSize.x128);
		}; 
		client.OnPresenceUpdate += (s, args) =>
		{
			Debug.Log("[DRP] Our Rich Presence has been updated. Applied changes to local store.");
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
		client.OnError += (s, args) =>
		{
			//Something bad happened while we tried to send a event. We will just log this for clarity.
			Debug.LogError("[DRP] Error Occured within the Discord IPC: (" + args.Code + ") " + args.Message);
		};

		client.OnJoinRequested += (s, args) =>
		{
			Debug.Log("[DRP] Join Requested");
	
		};

		events.RegisterEvents(client);
		#endregion
		
		//Start the client
		_client.Initialize();
		Debug.Log("[DRP] Discord Rich Presence intialized and connecting...");

		//Set initial presence and sub. (This will enqueue it)
		SetSubscription(_currentSubscription);
		SetPresence(_currentPresence);

	}
	
	private void OnDisable()
	{
		if (_client != null)
		{
			Debug.Log("[DRP] Disposing Discord IPC Client...");
			_client.Dispose();
			_client = null;
			Debug.Log("[DRP] Finished Disconnecting");
		}

	}

	private void FixedUpdate()
	{
		if (client == null) return;

		//Invoke the client events
		client.Invoke();
	}
#endif
#endregion

	/// <summary>
	/// Sets the Rich Presence of the Discord Client through the pipe connection. This differs from <see cref="SendPresence(DiscordPresence)"/> as the client will validate the presence update and the updated state will be tracked. 
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
			Debug.LogWarning("[DRP] Attempted to send a presence update to a client that is not initialized! The messages will be enqueued instead!");
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

		if (!client.IsInitialized)
		{
			Debug.LogError("[DRP] Attempted to send a presence update to a client that is not initialized!");
			return;
		}

		this._currentSubscription = evt;
		client.SetSubscription(evt.ToDiscordRPC());
	}

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

	/// <summary>
	/// Sets the Rich Presence of the Discord Client through a HTTP connection. This differs from <see cref="SetPresence(DiscordPresence)"/> as it does not attempt to connect to the websocket and is a write-only operation with no state validation.
	/// </summary>
	/// <param name="applicationID">The ID of this application</param>
	/// <param name="presence">The presence to set the client</param>
	/// <returns></returns>
	public static IEnumerator SendPresence(string applicationID, DiscordPresence presence)
	{
		var requestPayload = DiscordRPC.Web.WebRPC.PrepareRequest(presence.ToRichPresence(), applicationID);
		byte[] encodedRequest = System.Text.Encoding.UTF8.GetBytes(requestPayload.Data);

		WWW www = new WWW(requestPayload.URL, encodedRequest, requestPayload.Headers);
		yield return www;

		/*
		RichPresence p;
		string response = System.Text.Encoding.UTF8.GetString(www.bytes);
		DiscordRPC.Web.WebRPC.TryParseResponse(response, out p);
		*/
	}
}
