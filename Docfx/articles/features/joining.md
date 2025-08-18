# Joining and Parties
## Parties
Discord can show you and your friends in a party together for a specific game.

![image of parties](https://i.lu.je/2025/firefox_cxOtJ1xrdJ.png)

The [Party](xref:DiscordRPC.Party) can be used to create parties and control their size. With helper functions such as [DiscordRpcClient.UpdatePartySize](xref:DiscordRPC.DiscordRpcClient.UpdatePartySize(System.Int32)) you can dynamically update when players join and leave.
```cs
client.SetPresence(new()
{
    Details = "Party Example",
    State = "In Game",
    Party = new()
    {
        ID = "my-unique-party-id",
        Privacy = Party.PrivacySetting.Private,
        Size = 1,
        Max = 4,
    },
});
```
The [Party.ID](xref:DiscordRPC.Party.ID) is a unique id (to your application) that tells Discord what party the user is currently in. When two users use the same ID, Discord will group them together.

> [!TIP]
> If the size is set larger than actual number of users sharing the ID, then a default "fake" user is displayed instead.
>
> This value is also synchronised across all clients with the same ID.

## Joining / Lobbies
Users can use your Rich Presence as a very basic invite / join system. As long as the Party is set, you are able to provide special secret that discord will share to other users.

When accepted/joined, the other player's game will be launched and a [OnJoin](xref:DiscordRPC.DiscordRpcClient.OnJoin) will be called with the secret you provided in your presence.

![image of join button](https://i.lu.je/2025/Discord_dT7xxZDifj.png)


### Launching your game
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

> [!NOTE]
> You need to register the URI scheme to see other user's Join button and invites.

> [!WARNING]
> MacOS requires schemes to be registered through [App Bundles](https://developer.apple.com/documentation/xcode/defining-a-custom-url-scheme-for-your-app). This library cannot access your App Bundle and cannot automatically register **non-steam** games. 
>
> If you are publishing a MacOS game, you **must** provide either a `Steam App ID` or a custom uri scheme (such as `my-game://launch`) to the `RegisterUriScheme`. 

### Setting a Secret
To join a game, users will use a secret to connect to a lobby. These secrets should contain enough data to know who & how to connect to lobby but shouldn't include sensitive data like IP addresses.

As a basic example, consider this implementation:
```cs
client.SetPresence(new()
{
    Details = "Party Example",
    State = "In Game",
    Party = new() { /* .. Party Details .. */ },

    //  This is how a launching app will figure out how to connect to the game.
    Secrets = new()
    {
        Join = Lobby.CreateJoinToken(Lobby.current.ID)
    },
});

```

The `Lobby.current.ID` and `Lobby.CreateJoinToken()` will be up to your implementation. A naive approach would be to combine the lobby id and the password together. However, a better approach would be to use a signed key such as a [JWT](https://www.jwt.io/introduction#when-to-use-json-web-tokens) which could include the lobby id and the host could verify this token signature before allowing the user to join.

> [!TIP]
> Rich Presence is not a networking stack and does not provide lobbies or match making.
> Consult your engine's manual or check out [Steams Documentation](https://partner.steamgames.com/doc/features/multiplayer/matchmaking) for their lobbies.

### Joining
Users can join directly into a lobby from the Discord client via a "Join" button. The host must have the following set in their presence for this to be available:
* Party
* Secret
* Registered URI Scheme

![join](https://i.lu.je/2025/Discord_dT7xxZDifj.png)

When a joining user clicks the "Join" button, Discord will launch the application via the URI Scheme.

Once loaded, the application will subscribe to the [EventType.Join](xref:DiscordRPC.EventType), which will make Discord send a Join message via the [OnJoin](xref:DiscordRPC.DiscordRpcClient.OnJoin) event. This message will contain the secret set in the host's presence.

The application can use that secret to then join the lobby. As a basic example:
```cs
client.Subscribe(EventType.Join);
client.OnJoin += (object sender, JoinMessage args) =>  {
    Lobby.JoinWithToken(args.Secret);
};  
```

The `Lobby.JoinWithToken` will be dependant on your implementation. However, using a JWT as described earlier, you can extract the target Lobby ID and verify the signing signature when attempting to connect:

> [!NOTE]
> We do not automatically subscribe to events. This library is primarily uesd for simple Rich Presence and the Joining is a feature generally only useful for Games.
>
> Because of this, by default this app will not receive Discord Join events. You must call the [DiscordRpcClient.Subscribe](xref:DiscordRPC.DiscordRpcClient.Subscribe(DiscordRPC.EventType)).

> [!TIP]
> You can directly invite users to play your game. They will see a game invitation like so, and if they have run the URI Scheme registration too, they will be able to launch from here.
> 
> ![invite button](https://i.lu.je/2025/Discord_ivzV8uc0F8.png)

### Ask to Join
To make your lobby a invite-only, set your [Privacy Settings](xref:DiscordRPC.Party.PrivacySetting) to `Private`.

This will change the "Join" button to a "Ask To Join". When clicked, a message will appear in the host's Discord client. The host can directly respond to this in Discord, or your app can listen to this event and provide a way for the host to respond in game using the [OnJoinRequested](xref:DiscordRPC.DiscordRpcClient.OnJoinRequested) event.

![ask to join](https://i.lu.je/2025/Discord_P1f65SgZun.png)

To respond in game, subscribe to the [EventType.JoinRequest](xref:DiscordRPC.EventType) and use the [Respond(User, Boolean)](xref:DiscordRPC.DiscordRpcClient.Respond(DiscordRPC.User,System.Boolean)) to tell discord to accept or ignore:

```cs
client.Subscribe(EventType.JoinRequest);
client.OnJoinRequested += async (object sender, JoinRequestMessage args) => {
    var result = await UI.ShowRequestToJoin(args.user); // Hypothetical UI system
    client.Respond(args.User, result.accepted);
};
```

### Spectating
Spectating has been removed from Discord 😢

## Code Example
The [DiscordRPC.Example](https://github.com/Lachee/discord-rpc-csharp/blob/master/DiscordRPC.Example/Joining.cs) project contains a very basic example of the joining flow. Use 2 seperate clients as an example. 

```sh
# Run on the first pipe
dotnet run --framework net9 --project DiscordRPC.Example --example=Joining --pipe=0

# Run on the second pipe
dotnet run --framework net9 --project DiscordRPC.Example --example=Joining --pipe=1
```

> [!NOTE]
> While it does register a scheme, this example cannot launch properly because it just registers the dotnet runner. 
>
> This wont be a problem in a full application.