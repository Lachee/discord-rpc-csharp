# Discord Rich Presence

[![Build status](https://ci.appveyor.com/api/projects/status/dpu2l7ta05uvm397?svg=true)](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp) [![Codacy Badge](https://api.codacy.com/project/badge/Grade/a3fc8999eb734774bff83179fee2409e)](https://app.codacy.com/app/Lachee/discord-rpc-csharp?utm_source=github.com&utm_medium=referral&utm_content=Lachee/discord-rpc-csharp&utm_campaign=badger) [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/) [![https://img.shields.io/badge/dynamic/json?label=ko-fi&query=%24.0&suffix=%20cups&url=https%3A%2F%2Fd.lu.je%2Fkofi.php%3FP5P2YOWG&color=FF5E5B&logo=data:image/png;base64,iVBORw0KGgoAAAANSUhEUgAAAlkAAAJYCAYAAABRpPPiAAAACXBIWXMAAAsSAAALEgHS3X78AAAeDUlEQVR4nO3dP5BcxZ3A8cZFghxoXSVSaS+zIsngEFkiAWcILjtMIaqMHSJC/rgs6gwOEZcZXMWqbMKDVWaceAWExkiJIWNXKQTa4ESoq9a+Ka1W+2dm5/3m9ev+fKq21nXlw7P9lp3vdPfr99CdO3cSAAD9+pHxBADon8gCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACIL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ACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAAAogsAIAAIgsAIIDIAgAIILIAAAKILACAACILACCAyAIACCCyAAACiCwAgAAiCwAggMgCAAggsgAA+pZS+n8++k5QM42SigAAAABJRU5ErkJggg==](Ko-Fi)](https://ko-fi.com/lachee)

This is a C# _implementation_ of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. This avoids having to use the official C++ and instead provides a managed way of using the Rich Presence within the .NET environment*.

While the offical C++ library has been deprecated, this library has continued support and development for all your Rich Presence need, without requiring the Game SDK.

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

# Documentation
All the documentation can be found https://lachee.github.io/discord-rpc-csharp/docs/

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

The project does have a `Unity Package` available on the [Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts) and it is always recommended to use that. Its automatically built and guaranteed to be the latest. 

You can build the Unity3D package using the command below. Make sure you update the `DiscordRPC.dll` within the Unity Project first as it is not automatically updated:
```
.\build.ps1 -target Default -MakeUnityPackage -ScriptArgs '-buildType="Release"'
```

If you wish to have barebones Unity3D implementation, you need to build the `DiscordRPC.dll`, the [Unity Named Pipes](https://github.com/Lachee/unity-named-pipes) Library and the [UnityNamedPipe.cs](https://github.com/Lachee/discord-rpc-csharp/blob/master/Unity%20Example/Assets/Discord%20RPC/Scripts/Control/UnityNamedPipe.cs). Put these in your own Unity Project and the `.dll`s in a folder called `Plugins`. 


