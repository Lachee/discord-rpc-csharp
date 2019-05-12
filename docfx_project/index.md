#Discord Rich Presence

[![Build status](https://ci.appveyor.com/api/projects/status/dpu2l7ta05uvm397?svg=true)](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp)  [![Nuget](https://img.shields.io/nuget/v/DiscordRichPresence.svg)](https://www.nuget.org/packages/DiscordRichPresence/)
 [![GitHub stars](https://img.shields.io/github/stars/lachee/discord-rpc-csharp.svg?color=yellow&label=GitHub%20Stars&logo=github)](https://github.com/Lachee/discord-rpc-csharp)
 
This is a C# _implementation_ of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. This avoids having to use the official C++ and instead provides a managed way of using the Rich Presence within the .NET environment*.

This library supports all features of the Rich Presence that the official C++ library supports, plus a few extra:
 - **Message Queuing**
 - **Threaded Reads**
 - **Managed Pipes**
 - **Error Handling** & **Error Checking** with automatic reconnects
 - **Events from Discord** (such as presence update and join requests)
 - **Full Rich Presence Implementation** (including Join / Spectate)
 - **Inline Documented** (for all your intelli-sense needs)
 - **Helper Functionality** (eg: AvatarURL generator from Join Requests)
 - **Ghost Prevention** (Tells discord to clear the RP on disposal)
 - **Full Unity3D Editor** (Contains all the tools, inspectors and helpers for a Unity3D game all in one package).

 
#Quick Start

Check out the [Introduction](/articles/intro.html) article on how to get your Rich Presence working. Here is a summary of what is given:

1. Download on [Nuget](https://nuget.org/packages/DiscordRichPresence/) or [Artifacts](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts)
2. Include [Newtonsoft.JSON](https://www.newtonsoft.com/json) Dependency
3. Setup the client:
```
client = new DiscordRpcClient("myappid");
client.Initialize();
```
4. Send Presence: `client.SetPresence(presence);`
5. Invoke Events Regularly: `client.Invoke();`
6. Dispose when done: `client.Dispose();`

#Need More Help?

[![GitHub issues](https://img.shields.io/github/issues-raw/lachee/discord-rpc-csharp.svg?color=green&label=issues%20opened&logo=github)](https://github.com/Lachee/discord-rpc-csharp/issues)

Need more help or looking for some of the more advance features? Check out the [articles](/articles/intro.html) for more details on advance topics. Still stuck? Make a [new GitHub issue](https://github.com/Lachee/discord-rpc-csharp/issues/new)! 