# Standard
The standard guide for all .NET projects.

## Download
First the library must be downloaded. For standard projects within the .NET enviorment, a nuget package is available and is updated to the latest release.

 [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/)

```
PM> Install-Package DiscordRichPresence
```
A build of the library itself can be located in the [AppVeyor Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts)

## Usage
The library has 3 phases that must be followed,
1. Initialization
2. Rich Presence Setting & Event Invoking
3. Deinitialization and Disposal

You can set the Rich Presence of your app at any time while the client object has been constructed. The client will store the state of the presence and automatically resend it once initialized again.

### Initialization
The creation of the client should happen once in the lifetime of the app. Where you put the constructor is upto your application design principles, but in general its always a good idea to put it in your initializers.

The client should be _ideally_ treated like a [singleton](https://stackoverflow.com/a/2155713/5010271) and only ever created once. Multiple instances of the client can conflict with each other and cause unpredictable results within Discord and the end users Rich Presence.

```cs	
	public DiscordRpcClient Client { get; private set;}
	
	void Start() { 
		Client = new DiscordRpcClient("my_client_id");
		Client.Initialize();
	}
```

Note that the `Initialize()` can be called later and the current presence state will be re-sent to the Discord Client.

### Event Invoking
The `Invoke` method of the client must be called regulary to dequeue messages from Discord. These messages can be for things such as Join / Spectate to even things such as Rich Presence State Sync.
```cs
	void Update() {
		client.Invoke();
	}
```

If you are unsure about how to invoke events regularly in your app, or it has no actual event loop, you might want to use a `System.Timers.Timer` like so. Just remember that this will execute on a different thread and requires disposal too.
```cs
	var timer = new System.Timers.Timer(150);
	timer.Elapsed += (sender, args) => { client.Invoke(); };
	timer.Start();
```

### Setting Rich Presence
Setting Rich Presence is easy once the client has been initialized:
```cs
	//Set Presence
	client.SetPresence(new RichPresence()
	{
		Details = "Example Project",
		State = "csharp example",
		Assets = new Assets()
		{
			LargeImageKey = "image_large",
			LargeImageText = "Lachee's Discord IPC Library",
			SmallImageKey = "image_small"
		}
	});
```

### Disposal
It is important that the client is properly disposed when finished. This will activate the ghost prevention and clean up the resources properly.
```cs
	//Dispose client
	void End() {
		client.Dispose();
	}
```

## Further Reading
If you wish to implement the Join and Spectate feature within your project (those buttons), please read [Joining & Spectating Introduction](../join_spectate/intro.md) to get started.