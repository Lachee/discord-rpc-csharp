# Discord Rich Presence

This is a C# implementation of the [Discord RPC](https://github.com/discordapp/discord-rpc) library which was originally written in C++. You will be able to set the Rich Presence of your game directly in C# without an DllImport nonsence. 

Please note that Join/Spectate features are not yet supported by this library, but they are planned in the future.


# Installation

Installation is super easy. You just need to build the project with .NET 4.6
**Dependencies:**
 - Newtonsoft.Json 
- (For Unity) Unity 2017 in .NET 4.6 Runtime
	- Potential branch for just .NET 3.5 to work with Unity 5 later.
## Usage

The Discord.Test project within the solution contains example code, showing how to use all available features. For Unity Specific examples, check out the example project included. 

**Always remember to Dispose of the client on shutdown!**

For a quick start:
```csharp
//Create a discord client
DiscordClient rpc = new DiscordClient(clientid);

//Sets the current presence
await rpc.SetPresence(new RichPresence() {
	State = "In Editor",
	Details = "Rich Presence Example",
	Timestamps = new Timestamps()
	{
		Start = DateTime.UtcNow
	},
	Assets = new Assets()
	{
		LargeImageKey = "default_large",
		LargeImageText = "Default Large Image",
		SmallImageKey = "default_small",
		SmallImageText = "Default Small Image"
	},
	Party= new Party()
	{
		ID = "someuniqueid",
		Size = 1,
		Max = 10
	}
});

//Do some other things
DoThings();

//Clear the presence. This helps remove ghosting :)
await rpc.ClearPresence();

//Dispose of the connection
rpc.Dispose();
```
