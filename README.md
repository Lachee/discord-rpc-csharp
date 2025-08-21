<table frame="void">
    <tr>
      <td width="200px">
        <img src="https://raw.githubusercontent.com/Lachee/discord-rpc-csharp/master/Resources/logo.png" align="center" width="100%" />
      </td>
      <td>
        <h1>Discord RPC CSharp</h1>
        <p>
			<a href="https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml"><img src="https://github.com/Lachee/discord-rpc-csharp/actions/workflows/release.yml/badge.svg" alt="Release 📦" /></a>
			<a href="https://app.codacy.com/gh/Lachee/discord-rpc-csharp/dashboard?utm_source=gh&utm_medium=referral&utm_content=&utm_campaign=Badge_grade"><img src="https://app.codacy.com/project/badge/Grade/30c4e9f58b7f4a058f79ad0acd743edf" alt="Codacy Badge" /></a>
			<a href="https://www.nuget.org/packages/DiscordRichPresence/"><img src="https://img.shields.io/nuget/v/DiscordRichPresence.svg" alt="Nuget" /></a>
			<a href="https://github.com/Lachee/discord-rpc-csharp/tags"><img src="https://img.shields.io/github/package-json/v/lachee/discord-rpc-csharp?label=release" alt="GitHub package.json version" /></a>
        </p>
		<p>
			<strong>Discord RPC Csharp</strong> is a C# <em>implementation</em> of <a href="https://github.com/discordapp/discord-rpc">Discord RPC</a>. 
			It enables integration of Discord Rich Presence into .NET applications without relying on the deprecated official C++ library.
		</p>
		<p>
			This project continues to receive updates and support, offering a managed and user-friendly way to add Rich Presence and related features into your apps without the need of the GameSDK.
		</p>
      </td>
    </tr>
</table>

## Key Features

Here are some key features of this library:
 - **Full Rich Presence Implementation** (including Join)
 - **Events from Discord** (such as presence update and join requests)
 - **Error Handling** & **Error Checking** with automatic reconnects
 - **Well Documented** (for all your IntelliSense needs)
 - **Helper Functionality** (eg: AvatarURL generator from Join Requests)
 - **Managed Pipes**
 - **Ghost Prevention** (Tells Discord to clear the RP on disposal)
 - **Optionally Threaded Events** (For when you just want the events to wait a moment)

## Documentation
Extensive API documentation and usage articles can be found at [lachee.github.io/discord-rpc-csharp/](https://lachee.github.io/discord-rpc-csharp/).

## Supported .NET
This project supports the following .NET:
- `fx 4.5`
- `core 3.1`
- `net 7.0`
- `net 8.0`
- `net 9.0`

Dependencies:
- [JSON.NET 13](https://www.nuget.org/packages/newtonsoft.json/)
 
## Quick Start
Check out the [Getting Started](https://lachee.github.io/discord-rpc-csharp/articles/getting_started/introduction.html) for full guide on initialization, setting the presence, listening to events, and cleaning up.

Below is a very basic guide to get your first presence up.

```sh
dotnet add package DiscordRichPresence
```

```csharp
using DiscordRPC;

public const string DISCORD_APP_ID = "424087019149328395";
public static DiscordRpcClient client;

public static void Main() 
{
	// Create the client and setup some basic events
	client = new DiscordRpcClient(DISCORD_APP_ID)
	{
		Logger = new Logging.ConsoleLogger(Logging.LogLevel.Info, true)
	};

	client.OnReady += (sender, e) =>
	{
		Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
		Console.WriteLine("Avatar: {0}", msg.User.GetAvatarURL(User.AvatarFormat.WebP));
		Console.WriteLine("Decoration: {0}", msg.User.GetAvatarDecorationURL());
	};
	
	//Connect to the RPC
	client.Initialize();

	//Set the rich presence
	client.SetPresence(new RichPresence()
	{
		Details = "A Basic Example",
		State = "In Game",
		Assets = new Assets()
		{
			LargeImageKey = "image_large",
			LargeImageText = "Lachee's Discord IPC Library",
			SmallImageKey = "image_small"
		},
		Buttons = new Button[]
		{
			new Button() { Label = "lachee.dev", Url = "https://lachee.dev/" }
		}
	});	

	// ... Do Stuff ... 
	Console.ReadKey();

	// Cleanup
	client.Dispose();
}
```

> [!TIP]
> Some platforms require specific tricks, gotchyas, and troubleshooting.
>
> Check out the main documentation [Getting Started](https://lachee.github.io/discord-rpc-csharp/articles/getting_started/introduction.html) to address these issues.x


## Code Example
The [DiscordRPC.Example](https://github.com/Lachee/discord-rpc-csharp/blob/master/DiscordRPC.Example/) project contains a variety of examples to test and experiment with this library.

Use this project as a example on how to implement your own.

```sh
dotnet run --framework net9 --project DiscordRPC.Example --example=Basic
```

## Building
Check out the building guide in the [CONTRIBUTING.md](https://github.com/Lachee/discord-rpc-csharp/blob/master/CONTRIBUTING.md)

## Tests
There are currently no tests available to validate the library. This is a active issue and need help with this.
The test suite will likely need a way to mock the RPC client.

## Contribution
All contributions are welcome and I am happy for any contribution. However, there are some things that will not be accepted:
- Spelling only fixes (rude to only contribute to something copilot could do)
- Complete or large rewrites (unwanted work load to review)
- Dependency substitutions / removals / additions (these require a issue and discussion first)
- Support for features only provided by custom Discord clients
- Obviously AI additions

For more information, please read [CONTRIBUTING.md](https://github.com/Lachee/discord-rpc-csharp/blob/master/CONTRIBUTING.md)
