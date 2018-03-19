using DiscordRPC;
using DiscordRPC.Message;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class DiscordManager : MonoBehaviour {

	public string applicationID = "424087019149328395";
	public string steamID = "";
	public bool registerUriScheme = true;

	public int targetPipe = -1;

	public DiscordRPC.Logging.LogLevel logLevel = DiscordRPC.Logging.LogLevel.Info;

	public UnityPresence presence;

	public DiscordRpcClient client { get { return _client; } }
	private DiscordRpcClient _client;

	[SerializeField] private bool enabled = true;

	private void OnEnable()
	{
		Debug.Log("OnEnable");
		


		if (!enabled) return;
		if (!Application.isPlaying) return;

		//We only want to enable in the play mode
		Debug.Log("Starting Client...");
		_client = new DiscordRpcClient(applicationID, steamID, registerUriScheme, targetPipe);
		_client.Logger = new UnityLogger() { Level = logLevel };

		//Add listeners to all the events
		client.OnReady += OnReady;
		client.OnClose += OnClose;
		client.OnError += OnError;
		client.OnPresenceUpdate += OnPresenceUpdate;
		client.OnSubscribe += OnSubscribe;
		client.OnUnsubscribe += OnUnsubscribe;
		client.OnJoin += OnJoin;
		client.OnSpectate += OnSpectate;
		client.OnJoinRequested += OnJoinRequested;
				
		Debug.Log("Initializing Client...");
		_client.Initialize();
		_client.SetPresence(presence.ToRichPresence());		

	}


	private void OnDisable()
	{
		Debug.Log("OnDisable");
		Dispose();
	}

	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.A))
			Dispose();

		if (Input.GetKeyDown(KeyCode.Space) && _client && !_client.Disposed)
		{
			presence.details = Random.value.ToString();
			presence.assets.largeKey = "image_large_2";
			presence.assets.smallKey = "image_large_1";
			_client.SetPresence(presence.ToRichPresence());
		}

		if (_client != null)
			_client.Invoke();
	}

	private void Dispose()
	{
		Debug.Log("Disposing Client...");
		if (_client != null)
		{
			Debug.Log("Actually Disposing Client...");
			_client.Dispose();
		}

		Debug.Log("Setting client to null...");
		_client = null;
	}
	private static void OnReady(object sender, ReadyMessage args)
	{
		Debug.Log("On Ready: " + args.Version);
	}

	private static void OnClose(object sender, CloseMessage args)
	{
		Debug.Log("Lost Connection with client: " + args.Reason);
	}

	private static void OnError(object sender, ErrorMessage args)
	{
		Debug.Log("Error occured: " + args.Message + ", " + args.Code);
	}

	private static void OnPresenceUpdate(object sender, PresenceMessage args)
	{
		Debug.Log("Rich Presence Updated: " + args.Presence == null ? "NULL" : args.Presence.State);
	}

	private static void OnSubscribe(object sender, SubscribeMessage args)
	{
		Debug.Log("Subscribed: " + args.Event);
	}

	private static void OnUnsubscribe(object sender, UnsubscribeMessage args)
	{
		Debug.Log("Unsubscribed: " + args.Event);
	}

	private static void OnJoin(object sender, JoinMessage args)
	{
		Debug.Log("Joining Game '" + args.Secret + "'...");
		Debug.Log(" - Failed: Actual Game Not Implemented.");
	}

	private static void OnSpectate(object sender, SpectateMessage args)
	{
		Debug.Log("Spectating Game '" + args.Secret + "'...");
		Debug.Log(" - Failed: Actual Game Not Implemented.");
	}

	private static void OnJoinRequested(object sender, JoinRequestMessage args)
	{
		Debug.Log(args.User.Username + " has requested to join our game.");
		Debug.Log(" - User's Avatar: " + args.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x2048));
		Debug.Log(" - User's Descrim: " + args.User.Descriminator);
		Debug.Log(" - User's Snowflake: " + args.User.ID);

		/*
		Console.Write("Do you give this user permission to join? [Y / n]: ");
		bool accept = Console.ReadKey().Key == ConsoleKey.Y; Debug.Log();

		DiscordRpcClient client = (DiscordRpcClient)sender;
		client.Respond(args, accept);
		Debug.Log(" - Sent a {0} invite to the client {1}", accept ? "ACCEPT" : "REJECT", args.User.Username);
		*/
	}

}
