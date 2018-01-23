using DiscordRPC;
using UnityEngine;

public class DiscordManager : MonoBehaviour {

	public static DiscordManager instance { get { return _instance; } }
	private static DiscordManager _instance;

	private DiscordClient client;

	/// <summary>
	/// The ID of the bot we are using
	/// </summary>
	[Tooltip("The ID of the bot we are using. Can be either the key itself or a path to the key")]
	public string clientID;
	private string _clientid;

	/// <summary>
	/// The last presence that was sent to the server
	/// </summary>
	[Tooltip("The last presence that was sent to the server")]
	public UnityPresence presence;


	private void Awake()
	{
		if (_instance)
		{
			Destroy(this);
			return;
		}

		_instance = this;
		DontDestroyOnLoad(gameObject);
	}

	private void OnEnable()
	{
		if (client != null) OnDisable();
		if (string.IsNullOrEmpty(clientID))
		{
			Debug.LogError("No client ID supplied for discord!");
			return;
		}

		//Check if its a file path
		if (clientID.Contains(".key"))
		{
			//Make sure the file exist
			if (!System.IO.File.Exists(clientID))
			{
				Debug.LogError("Path was supplied to client ID, but the file does not exist!");
				return;
			}

			try
			{
				//Try to open the file and read its contents
				_clientid = System.IO.File.ReadAllText(clientID);
			}
			catch (System.Exception e)
			{
				//Oh shit, something happened!
				Debug.LogError("An exception occured while trying to read the key: " + e.Message);
				return;
			}
		}
		else
		{
			//Its just a normal key, read it like a normal person.
			_clientid = clientID;
		}

		//Create the client and register to the events
		client = new DiscordClient(_clientid);
		DiscordClient.OnLog += OnDiscordLog;
		client.OnError += OnDiscordError;
	}

	private void OnDiscordError(object sender, DiscordRPC.Events.DiscordErrorEventArgs args)
	{
		Debug.LogError("Error occured within discord: (" + args.ErrorCode + ") " + args.Message);
	}

	private void OnDiscordLog(string formatting, params object[] objects)
	{
		Debug.Log(string.Format(formatting, objects));
	}


	private async void OnDisable()
	{
		//Clear the presence (Makes discord acknowledge the game is done) then dispose.
		await client.ClearPresence();
		client.Dispose();
		client = null;		
	}

	public static async void SetPresence(UnityPresence presence)
	{
		if (instance.client == null)
		{
			Debug.LogWarning("Cannot set presence as the discord manager is disabled!");
			return;
		}
		
		instance.presence = presence;
		await instance.client.SetPresence(presence.ToRichPresence());
	}
	public static async void ClearPresence()
	{
		if (instance.client == null)
		{
			Debug.LogWarning("Cannot clear presence as the discord manager is disabled!");
			return;
		}

		await instance.client.ClearPresence();
	}
}
