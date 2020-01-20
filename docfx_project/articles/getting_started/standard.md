# Standard

The standard guide for all .NET projects.

## Download

First the library must be downloaded. For standard projects within the .NET enviorment, a nuget package is available and is updated to the latest release.

 [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/)

```powershell
PM> Install-Package DiscordRichPresence
```

A build of the library itself can be located in the [AppVeyor Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts)

## Usage

The library has 3 phases that must be followed,

1. Initialization
2. Rich Presence Setting
3. Deinitialization and Disposal

You can set the Rich Presence of your app at any time while the client object has been constructed. The client will store the state of the presence and automatically resend it once initialized again.

### Initialization

The creation of the client should happen once in the lifetime of the app. Where you put the constructor is upto your application design principles, but in general its always a good idea to put it in your initializers.

The client should be _ideally_ treated like a [singleton](https://stackoverflow.com/a/2155713/5010271) and only ever created once. Multiple instances of the client can conflict with each other and cause unpredictable results within Discord and the end users Rich Presence.

```cs
public DiscordRpcClient Client { get; private set;}

void Setup() {
	Client = new DiscordRpcClient("my_client_id");	//Creates the client
	Client.Initialize();							//Connects the client
}
```

Note that the `Initialize()` can be called later and the current presence state will be re-sent to the Discord Client.


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

You may call this as regularly as you wish, the default behaviour of the application will ignore duplicate presence and Discord itself will handle ratelimiting. 
With that said, its always a good idea to only set the presence when there is actual change, to avoid any overheads. 

### Disposal

It is important that the client is properly disposed when finished. This will safely disconnect from Discord and dispose of the resources correctly. If you have any issues with ghosting (particularly in Unity3D), make sure you dispose the client.

```cs
	//Dispose client
	void Cleanup() {
		client.Dispose();
	}
```


### Events

By defaults, events will be executed as they occur. This means they are executed on the **RPC Thread**, and not on the main. For most applications, this works fine and is treated as a normal event from any other library you may use. However, for applications where thread-safety is paramount (such as Game Engines), you may need to disable this feature and manually invoke events on your calling thread like so:

```cs
void Start() {
	//Creates a new client, telling it not to automatically invoke the events on RPC thread.
	Client = new DiscordRpcClient("my_client_id", autoEvents: false);
	Client.Initialize();
}

void Update() {
	//Invoke the events once per-frame. The events will be executed on calling thread.
	Client.Invoke();
}
```

Please note that this method is _only_ required where cross-thread talk is a big no-no. Implementing this as a Timer would just defeat the purpose as they are [threaded anyways](https://stackoverflow.com/questions/1435876/do-c-sharp-timers-elapse-on-a-separate-thread).

## Further Reading

If you wish to implement the Join and Spectate feature within your project (those buttons), please read [Joining & Spectating Introduction](../join_spectate/intro.md) to get started.

