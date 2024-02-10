# Discord Rich Presence

[![Release 📦](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml/badge.svg)](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml)
[![Codacy Badge](https://app.codacy.com/project/badge/Grade/30c4e9f58b7f4a058f79ad0acd743edf)](https://www.codacy.com/gh/Lachee/discord-rpc-csharp/dashboard?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=Lachee/discord-rpc-csharp&amp;utm_campaign=Badge_Grade) [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/) 
[![GitHub package.json version](https://img.shields.io/github/package-json/v/lachee/discord-rpc-csharp?label=master)](https://github.com/Lachee/discord-rpc-csharp/tags)

This is a C# _implementation_ of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. This avoids having to use the official C++ and instead provides a managed way of using the Rich Presence within the .NET environment*.

While the official C++ library has been deprecated, this library has continued support and development for all your Rich Presence need, without requiring the Game SDK.

Here are some key features of this library:
 - **Message Queuing**
 - **Threaded Reads**
 - **Managed Pipes***
 - **Error Handling** & **Error Checking** with automatic reconnects
 - **Events from Discord** (such as presence update and join requests)
 - **Full Rich Presence Implementation** (including Join / Spectate)
 - **Inline Documented** (for all your IntelliSense needs)
 - **Helper Functionality** (eg: AvatarURL generator from Join Requests)
 - **Ghost Prevention** (Tells Discord to clear the RP on disposal)
 - **Full Unity3D Editor** (Contains all the tools, inspectors and helpers for a Unity3D game all in one package).

# Documentation
All the documentation can be found [lachee.github.io/discord-rpc-csharp/docs/](https://lachee.github.io/discord-rpc-csharp/)

# Installation

**Dependencies:**
 - Newtonsoft.Json 
 - .NET Standard 2.0
 
### **.NET Project**

For projects that target either .NET Core or .NETFX, you can get the package on [nuget](https://www.nuget.org/packages/DiscordRichPresence/):
```
PM> Install-Package DiscordRichPresence
```
You can also [Download or Build](#building) your own version of the library if you have more specific requirements.

### **Unity3D Game Engine**

Unity Package is being moved to [Lachee/Discord-RPC-Unity](https://github.com/Lachee/discord-rpc-unity). 
Please check the releases / documentation there.

## Usage

The Discord.Example project within the solution contains example code, showing how to use all available features. For Unity Specific examples, check out the example project included. There are 3 important stages of usage, Initialization, Invoking and Deinitialization. It's important you follow all 3 stages to ensure proper behaviour of the library.

**Initialization**

This stage will setup the connection to Discord and establish the events. Once you have done the initialization you can call `SetPresence` and other variants as many times as you wish throughout your code. Please note that ideally this should only run once, otherwise conflicts may occur with them trying to access the same Discord client at the same time.
```csharp
public DiscordRpcClient client;

//Called when your application first starts.
//For example, just before your main loop, on OnEnable for unity.
void Initialize() 
{
	/*
	Create a Discord client
	NOTE: 	If you are using Unity3D, you must use the full constructor and define
			 the pipe connection.
	*/
	client = new DiscordRpcClient("my_client_id");			
	
	//Set the logger
	client.Logger = new ConsoleLogger() { Level = LogLevel.Warning };

	//Subscribe to events
	client.OnReady += (sender, e) =>
	{
		Console.WriteLine("Received Ready from user {0}", e.User.Username);
	};
		
	client.OnPresenceUpdate += (sender, e) =>
	{
		Console.WriteLine("Received Update! {0}", e.Presence);
	};
	
	//Connect to the RPC
	client.Initialize();

	//Set the rich presence
	//Call this as many times as you want and anywhere in your code.
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
}
```



**Invoking**

**Invoking is optional. Use this when thread safety is paramount.**

The client will store messages from the pipe and won't invoke them until you call `Invoke()` or `DequeueMessages()`. It does this because the pipe is working on another thread, and manually invoking ensures proper thread safety and order of operations (especially important in Unity3D applications).

In order to enable this method of event calling, you need to set it in the constructor of the DiscordRpcClient under `autoEvents`.
```csharp
//The main loop of your application, or some sort of timer. Literally the Update function in Unity3D
void Update() 
{
	//Invoke all the events, such as OnPresenceUpdate
	client.Invoke();
};
```

Here is an example on how a Timer could be used to invoke the events for a WinForm
```csharp
var timer = new System.Timers.Timer(150);
timer.Elapsed += (sender, args) => { client.Invoke(); };
timer.Start();
```

**Deinitialization**

It's important that you dispose your client before your application terminates. This will stop the threads, abort the pipe reads, and tell Discord to clear the presence. Failure to do so may result in a memory leak!
```csharp
//Called when your application terminates.
//For example, just after your main loop, on OnDisable for unity.
void Deinitialize() 
{
	client.Dispose();
}
```

## Building

[![Release 📦](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml/badge.svg)](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml) [![Documentation 📚](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/documentation.yml/badge.svg)](https://github.com/Lachee/discord-rpc-csharp/actions/workflows/documentation.yml)


**DiscordRPC Library**
```
dotnet build -c Release
```

**Unity3D**

If you wish to have barebones Unity3D implementation, you need to build the `DiscordRPC.dll`, the [Unity Named Pipes](https://github.com/Lachee/unity-named-pipes) Library and the [UnityNamedPipe.cs](https://github.com/Lachee/discord-rpc-csharp/blob/master/Unity%20Example/Assets/Discord%20RPC/Scripts/Control/UnityNamedPipe.cs). Put these in your own Unity Project and the `.dll`s in a folder called `Plugins`. 

**UWP / .NET MAUI / WIN UI 3**

For now, the library doesn't work on UWP applications until we find the issue and fix it.

In order to make this library work with the WIN UI 3 related applications such as .NET MAUI, you need to define `runFullTrust` Capability inside `Package.appxmanifest`.

Here is an example of how to add `runFullTrust` to your WIN UI 3 application:

`Package.appxmanifest`:

```xml
<?xml version="1.0" encoding="utf-8"?>

<Package
  xmlns="http://schemas.microsoft.com/appx/manifest/foundation/windows10"
  xmlns:uap="http://schemas.microsoft.com/appx/manifest/uap/windows10"
  xmlns:rescap="http://schemas.microsoft.com/appx/manifest/foundation/windows10/restrictedcapabilities"
  IgnorableNamespaces="uap rescap">
  ...
    <Capabilities>
	    <rescap:Capability Name="runFullTrust" />
    </Capabilities>
</Package>
```

If you use .NET MAUI or WIN UI 3 template for C#, it automatically puts `runFullTrust` capability.