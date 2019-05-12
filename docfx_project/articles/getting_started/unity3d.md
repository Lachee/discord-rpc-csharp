# Unity3D
This library has full Unity3D intergration and custom editor scripts to help enhance your usage with the library. Please note there are some technical limitations with Unity3D however:

* .NET 2.0+ is required (no subset).
* Newtonsoft.Json is required.
* Native Named Pipe Wrapper is required.

Luckily the provided Unity Package handles all this for you.

## Download

Use the automatically built `.UnityPackage` that can be found in the artifacts of the AppVoyer build. This contains the extra dependencies for the platform and full editor support to make it easier to use.

[Download Package](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts) and import into your project.

[![Unity Package](/images/unity_package.png)](https://ci.appveyor.com/project/Lachee/discord-rpc-csharp/build/artifacts)

## Importing
Import the unity package normally and make sure all content is selected. Once imported you may get the following warning. This library does not support the .NET 2.0 **Subset** and requires the full .NET 2.0 or greater. Proceeding with `Yes` will convert the project automatically to .NET 2.0.

![import Warning](/images/unity_netconvert.png)

## Creating a Manager
The Discord Manager is a wrapper class around the DiscordRpcClient. It will handle the initialization, deinitialization and event invoking for you automatically. 

Create a new Discord Manager in your very first loaded scene by following `GameObject -> Discord Manager`. 

![Import Settings](/images/unity_add_marked.png)

### Discord Manager Inspector

Once created, a new object will appear in your scene. You can _only_ have 1 Discord Manager at a time and any extras will automatically be deleted. The manager has some default values, but will need to be configured to your application. 

| Property | Description |
|----------|-------------|
| Application ID | The Client ID of your Discord App created in the [Developer Portal](https://discordapp.com/developers/applications/). |
| Steam ID | A optional Steam App ID of your game. This will let Discord launch your game through the steam client instead of directly when using the [Join & Spectate](/join_spectate/intro.md) |
| Target Pipe | The pipe your Discord Client is running on. If you have 2 clients running for testing purposes, you can switch which client the game connects too. |
| Log Level | The level of logging to receive from the DiscordRpcClient. |
| Register Uri Scheme | Registers a custom URI scheme so Discord can launch your game. Only required if using the [Join & Spectate](/join_spectate/intro.md) feature. |
| Active | If enabled, the Discord Manager will create a connection and maintain it. |
| **State** | The current state of the connected client. These values are generally `Read Only` |

![Configuration](/images/unity_inspector_hierarchy.png)

## Usage
Setting Rich Presence is done via your game code. It is upto you on how you implement it, but as an example from the Survival Shooter example by Unity3D:
```cs
	public void UpdatePresence()
	{
		presence.state = "Score: " + CompleteProject.ScoreManager.score;
		presence.largeAsset = new DiscordAsset()
		{
			image = health.isDead ? "dead" : "alive",
			tooltip = health.currentHealth + "HP"
		};

		DiscordManager.current.SetPresence(presence);
	}
```

## Further Reading
If you wish to implement the Join and Spectate feature within your project (those buttons), please read [Joining & Spectating Introduction](../join_spectate/intro.md) to get started.