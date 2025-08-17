# Joining and Parties
## Parties
Discord can show you and your friends in a party together for a specific game.

todo: (xref:DiscordRPC.Party)
todo: image of 2 people in a party
todo: example code for parties

## Joining / Lobbies
You can provide a way for users to invite others to play your game either directly or by having a "Join Game" / "Request to Join" button on your presence.

When accepted/joined, the other player's game will be launched and a (xref:DiscordRPC.DiscordRpcClient.OnJoin) will be called with the secret you provided in your presence.

todo: image here of a invite

> [!NOTE]
> Rich Presence only sends your secret to the other player. Your game must be able to connect and manage lobbies itself.
>
> Check out [Steams Documentation](https://partner.steamgames.com/doc/features/multiplayer/matchmaking) for their lobbies.

### 1. Launching your game
Discord uses custom [URI Schemes](https://developer.mozilla.org/en-US/docs/Web/URI/Reference/Schemes) to launch your game. It requires one to be set to show any of joining buttons.

This library provides functionality to do this. Before initializing, use the RegisterScheme function:
```cs
client.RegisterUriScheme();
client.Initialize();
```

If you have a Steam game, you can launch using the `steam://` scheme instead by providing an appid:
```cs
client.RegisterUriScheme("657300");
client.Initialize();
```

> [!WARNING]
> MacOS requires schemes to be registered through [App Bundles](https://developer.apple.com/documentation/xcode/defining-a-custom-url-scheme-for-your-app). This library cannot access your App Bundle and cannot automatically register **non-steam** games. 
>
> If you are publishing a MacOS game, you **must** provide either a `Steam App ID` or a custom uri scheme (such as `my-game://launch`) to the `RegisterUriScheme`. 

### 2. Setting a Secret

### 3. Accepting Requests
todo (xref:DiscordRPC.DiscordRpcClient.OnJoinRequest)

### 4. Joining
todo (xref:DiscordRPC.DiscordRpcClient.OnJoin)