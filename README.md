# Discord Rich Presence

[![Build status](https://ci.appveyor.com/api/projects/status/dpu2l7ta05uvm397?svg=true)](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/a3fc8999eb734774bff83179fee2409e)](https://app.codacy.com/app/Lachee/discord-rpc-csharp?utm_source=github.com&utm_medium=referral&utm_content=Lachee/discord-rpc-csharp&utm_campaign=badger) [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/)

This is a C# _implementation_ of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. This avoids having to use the official C++ and instead provides a managed way of using the Rich Presence within the .NET environment*.

While the offical C++ library has been depreciated, this library has continued support and development for all your Rich Presence need, without requiring the Game SDK.

Here are some key features of this library:
 - **Message Queuing**
 - **Threaded Reads**
 - **Managed Pipes***
 - **Error Handling** & **Error Checking** with automatic reconnects
 - **Events from Discord** (such as presence update and join requests)
 - **Full Rich Presence Implementation** (including Join / Spectate)
 - **Inline Documented** (for all your intelli-sense needs)
 - **Helper Functionality** (eg: AvatarURL generator from Join Requests)
 - **Ghost Prevention** (Tells discord to clear the RP on disposal)
 - **Full Unity3D Editor** (Contains all the tools, inspectors and helpers for a Unity3D game all in one package).

# Installation

**Dependencies:**
 - Newtonsoft.Json 
 - .NET 3.5+
 
**Unity3D Dependencies:**
 - Newtonsoft.Json  (included in Unity Package).
 - .NET 2.0+ (not subset)
 - [Unity Named Pipes](https://github.com/Lachee/unity-named-pipes) Library (included in Unity Package).
  
**Source: .NET Project**

For projects that target either .NET Core or .NETFX, you can get the package on [nuget](https://www.nuget.org/packages/DiscordRichPresence/):
```
PM> Install-Package DiscordRichPresence
```
You can also [Download or Build](#building) your own version of the library if you have more specific requirements.

**Source: Unity3D Game Engine**

There is a Unity Package available for quick setup, which includes the editor scripts, managers and tools to make your life 100x easier. Simply download the package from the [Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts) AppVoyer generates. This includes the native library and the managed library prebuilt, so you dont need to worry about a thing.  

For building your own package, read the [building](#building) guide.

## Usage

The Discord.Example project within the solution contains example code, showing how to use all available features. For Unity Specific examples, check out the example project included. There are 3 important stages of usage, Initialization, Invoking and Deinitialization. Its important you follow all 3 stages to ensure proper behaviour of the library.

**Initialization**

This stage will setup the connection to Discord and establish the events. Once you have done the intialization you can call `SetPresence` and other variants as many times as you wish throughout your code. Please note that ideally this should only run once, otherwise conflicts may occur with them trying to access the same discord client at the same time.
```csharp
public DiscordRpcClient client;

//Called when your application first starts.
//For example, just before your main loop, on OnEnable for unity.
void Initialize() 
{
	/*
	Create a discord client
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

**Invoking is optional. Use this when thread saftey is paramount.**

The client will store messages from the pipe and won't invoke them until you call `Invoke()` or `DequeueMessages()`. It does this because the pipe is working on another thread, and manually invoking ensures proper thread saftey and order of operations (especially important in Unity3D applications).

In order to enable this method of event calling, you need to set it in teh constructor of the DiscordRpcClient under `autoEvents`.
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

Its important that you dispose your client before you application terminates. This will stop the threads, abort the pipe reads and tell discord to clear the presence. Failure to do so may result in a memory leak!
```csharp
//Called when your application terminates.
//For example, just after your main loop, on OnDisable for unity.
void Deinitialize() 
{
	client.Dispose();
}
```

## Building

**DiscordRPC Library**

You can build the solution easily in Visual Studio, its a simple matter of right clicking the project and hitting build. However if you wish to build via command line, you can do so with the PowerShell build script:
```
.\build.ps1 -target Default -ScriptArgs '-buildType="Release"'
```

**Unity3D**

The project does have a `Unity Pacakge` available on the [Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts) and it is always recommended to use that. Its automatically built and guaranteed to be the latest. 

You can build the Unity3D package using the command below. Make sure you update the `DiscordRPC.dll` within the Unity Project first as it is not automatically updated:
```
.\build.ps1 -target Default -MakeUnityPackage -ScriptArgs '-buildType="Release"'
```

If you wish to have barebones Unity3D implementation, you need to build the `DiscordRPC.dll`, the [Unity Named Pipes](https://github.com/Lachee/unity-named-pipes) Library and the [UnityNamedPipe.cs](https://github.com/Lachee/discord-rpc-csharp/blob/master/Unity%20Example/Assets/Discord%20RPC/Scripts/Control/UnityNamedPipe.cs). Put these in your own Unity Project and the `.dll`s in a folder called `Plugins`. 


