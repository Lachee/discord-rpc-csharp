# Getting Started
This is a quck start guide on getting very basic presence working.

## Platforms
For non-dotnet platforms, there are a variety of guides to get you started

| Platform | Guide |
|----------|---------------|
| .NET app | [Getting Started](./introduction.md#download) |
| [Godot](https://godotengine.org/) | 🚧 📦 Package [lachee/discord-rpc-godot](https://github.com/Lachee/discord-rpc-godot/) |
| [Mono](https://www.mono-project.com/) | [Mono Guide](./mono.md) |
| [MonoGame](https://monogame.net/) | [Mono Guide](./mono.md) |
| [Unity3D](https://unity.com/) | 📦 Package [lachee/discord-rpc-unity](https://github.com/lachee/discord-rpc-unity/) |
| [UWP](https://learn.microsoft.com/en-us/windows/uwp/get-started/universal-application-platform-guide) | [UWP Guide](./uwp.md) |
| [WinForm](https://learn.microsoft.com/en-us/visualstudio/ide/create-csharp-winform-visual-studio?view=vs-2022) | [WinForm Guide](./winform.md) |

The following guide will assume a standard .NET application.

## Download

[![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/)
[![GitHub package.json version](https://img.shields.io/github/package-json/v/lachee/discord-rpc-csharp?label=Release)](https://github.com/Lachee/discord-rpc-csharp/tags)

```sh
dotnet add package DiscordRichPresence
```

Download the package with [NuGet](https://www.nuget.org/packages/DiscordRichPresence/) to start using in your project.

Unpackaged binaries can be found in the project's [Releases](https://github.com/Lachee/discord-rpc-csharp/releases).

## Usage

The library has 3 phases that must be followed,

1. Initialization
2. Rich Presence Setting
3. Deinitialization and Disposal

You can set the Rich Presence of your app at any time while the client object has been constructed. The client will store the state of the presence and automatically resend it once initialized again.

### Initialization

The creation of the client should happen once in the lifetime of the app. Where you put the constructor is upto your application design principles, but in general its always a good idea to put it in your initializers.

The client should be treated like a [singleton](https://stackoverflow.com/a/2155713/5010271) and only ever created once. Multiple instances of the client can conflict with each other and cause unpredictable results within Discord and the end users Rich Presence.

```cs
public DiscordRpcClient Client { get; private set; }

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

> [!NOTE]
> This method is _only_ required where cross-thread talk is a big no-no. 
> Implementing this as a Timer would just defeat the purpose as they are [threaded anyways](https://stackoverflow.com/questions/1435876/do-c-sharp-timers-elapse-on-a-separate-thread).


## Code Example

The [DiscordRPC.Example](https://github.com/Lachee/discord-rpc-csharp/blob/master/DiscordRPC.Example/Basic.cs) project contains a very basic example of setting up a client and creating your first presence.

```sh
dotnet run --framework net9 --project DiscordRPC.Example --example=Basic
```

## Building

Check out the building guide in the [CONTRIBUTING.md](https://github.com/Lachee/discord-rpc-csharp/blob/master/CONTRIBUTING.md)

# Need More Help?

[![GitHub issues](https://img.shields.io/github/issues-raw/lachee/discord-rpc-csharp.svg?color=green&label=issues%20opened&logo=github)](https://github.com/Lachee/discord-rpc-csharp/issues)

Still stuck? Make a [new GitHub issue](https://github.com/Lachee/discord-rpc-csharp/issues/new)! 
