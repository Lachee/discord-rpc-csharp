using DiscordRPC;
using UnityEngine;
using UnityEngine.Events;

public class DiscordManager : MonoBehaviour {
	
	/// <summary>
	/// The current instance of the Discord Manager
	/// </summary>
	public static DiscordManager instance { get { return _instance; } }
	private static DiscordManager _instance;


    /// <summary>
    /// The ID of the bot we are using
    /// </summary>
    [Tooltip("The ID of the bot we are using.")]
	public string clientID;
	
    /// <summary>
    /// The last presence that was sent to the server
    /// </summary>
    [Tooltip("The last presence that was sent to the server")]
    public UnityPresence presence;
    
	/// <summary>
	/// Are server messages being logged?
	/// </summary>
    [Tooltip("Log all messages from the Discord RPC. Cannot be changed runtime.")]
    public bool logMessages = false;

	/// <summary>
	/// Are server errors being logged?
	/// </summary>
    [Tooltip("Log all error messages from the Discord RPC. Cannot be changed runtime.")]
    public bool logErrors = true;

	/// <summary>
	/// Should Discord be disabled while in the editor?
	/// </summary>
    [Tooltip("Dont run the Discord Rich Presence in edit?")]
    public bool disableInEditor = false;

	[Tooltip("Called when the presences gets successfully updated")]
	public UnityEvent OnPresenceUpdate;

	private void Awake()
	{
		//Set this object to not destroy on load
		DontDestroyOnLoad(this);
	}

	//The discord client
	private DiscordClient client;
	private void OnEnable()
	{
		//Update the instance
		_instance = this;

		//We are editor, so stop here
		if (disableInEditor && Application.isEditor) return;

		//We already have a client, stop here
		if (client != null) return;

		//Validate the client ID
		if (string.IsNullOrEmpty(clientID))
		{
			Debug.LogError("No client ID supplied for discord!");
			return;
		}
		
		//Create the client. This will automatically connected when needed.
		client = new DiscordClient(clientID);

		//Register to the events.
		if (logMessages) DiscordClient.OnLog += OnDiscordLog;
		if (logErrors) client.OnError += OnDiscordError;
		
	}

	private void OnDiscordError(object sender, DiscordRPC.Events.DiscordErrorEventArgs args)
	{
		//We got a discord error, better log it
		Debug.LogError("Discord: (" + args.ErrorCode + ") " + args.Message);
	}

	private void OnDiscordLog(string formatting, params object[] objects)
	{
		//We got a discord log.
		Debug.Log("Discord: " + string.Format(formatting, objects));
	}


	private async void OnDisable()
	{
		//If we have the client, we need to dispose of it
        if (client != null)
        {
			//Ww are in the editor, but this should be made?
            if (disableInEditor && Application.isEditor)
                Debug.LogError("Disabling client even though its been disabled!");

			//Check if we have connected yet and if we have, clear the presence.
            if (client.IsConnected)
				await client.ClearPresence();

			//Dispose of the client
			client.Dispose();
            client = null;
        }
	}

	/// <summary>
	/// Sets the presence of the discord client
	/// </summary>
	/// <param name="presence">The presence</param>
	public static async void SetPresence(UnityPresence presence)
	{
		//We are disabled in the editor, so give up
		if (instance.disableInEditor && Application.isEditor)
		{
			Debug.LogWarning("Cannot set presence as the discord manager has been disabled for the editor");
			return;
		}

		//Discord client hasn't been created yet.
		if (instance.client == null)
		{
			Debug.LogWarning("Cannot set presence as the discord manager is disabled!");
			return;
		}
				
		/*
		 * Send the presence to discord. The client will automatically attempt to connect to discord and set the presence.
		 * If the connection fails, then it will enqueue it and wait for either the next message or a manual update.
		 */
        RichPresence response = await instance.client.SetPresence(presence.ToRichPresence());

		// Check that we have a valid response back and update our own presence to match.
		if (response != null)
		{
			instance.presence = new UnityPresence(response);
			instance.OnPresenceUpdate?.Invoke();
		}
	}

	/// <summary>
	/// Attempts to process the enqueued presence messages.
	/// </summary>
	public static async void UpdatePresence()
	{     
		//We are disabled in the editor, so give up
		if (instance.disableInEditor && Application.isEditor)
		{
			Debug.LogWarning("Cannot set presence as the discord manager has been disabled for the editor");
			return;
		}

		//Discord client hasn't been created yet.
		if (instance.client == null)
		{
			Debug.LogWarning("Cannot set presence as the discord manager is disabled!");
			return;
		}

		/*
		 * Tell the client to process any messages it has enqueued. This will try to connect to the server and send the message.
		 * If connection fails, null will be returned and it will need to be done again.
		 */
		RichPresence response = await instance.client.UpdatePresence();

		// Check that we have a valid response back and update our own presence to match.
		if (response != null)
		{
			instance.presence = new UnityPresence(response);
			instance.OnPresenceUpdate?.Invoke();
		}
	}

	/// <summary>
	/// Clears the stored presence
	/// </summary>
	public static async void ClearPresence()
	{
		//We are disabled in the editor, so what ya trying at boi?
		if (instance.disableInEditor && Application.isEditor)
		{
			Debug.LogWarning("Cannot clear presence as the discord manager has been disabled for the editor");
			return;
		}

		//We dont have a client yet?
		if (instance.client == null)
		{
			Debug.LogWarning("Cannot clear presence as the discord manager is disabled!");
			return;
		}

		/*
		 * Tell the client to clear out all the presence. It will attempt to make a connection to discord and send it a null presence.
		 * Use this to help prevent game ghosting. Becareful though, it will create a connection if not already established to do so!
		 */
		await instance.client.ClearPresence();
		
		//Create a new empty presence, as we have reset everything.
		instance.presence = new UnityPresence();
		instance.OnPresenceUpdate?.Invoke();
		
	}    
}
