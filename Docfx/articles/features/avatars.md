---
uid: avatars
---

# Avatars
When the RPC Client connects to Discord it will be given a User Object representing the active user on that client. It is possible to grab the avatar of the user and Display it within your application. 

This is useful for scoreboards, invite links, and other personalisation features.

![diagram of images](https://i.lu.je/2025/avatars.png)

## OnReady Event
The user's information is not immediately available. You must wait for the `OnReady` event emitted by the client.
This event will contain the current user information, and the [DiscordRpcClient.CurrentUser](xref:DiscordRPC.DiscordRpcClient.CurrentUser) is not valid until it has invoked.

> [!WARNING]
> As described in the [Getting Started](../getting_started.md#events), events are executed in another thread.
>
> If you are using a Game Engine, you need to manually use [DiscordRpcClient.Invoke](xref:DiscordRPC.DiscordRpcClient.Invoke) on your main thread or otherwise handle the cross-thread data transfer.

## Getting the Avatar
The [User.Avatar](xref:DiscordRPC.User.Avatar) is the current avatar hash, and isn't the picture. Use the helper functions to get an avatar with the correct format and sizing:
```cs
client.OnReady += (sender, msg) =>
{
    //Create some events so we know things are happening
    Console.WriteLine("Connected to discord with user {0}", msg.User.Username);
    Console.WriteLine("Avatar URL: {0}", msg.User.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x128));
};
```

Once you have the Avatar URL, you can download it using your favourite HTTP Client:
```cs
string url = client.CurrentUser.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x128);
using (var client = new System.Net.WebClient())
{
    client.DownloadFile(url, "avatar.png");
}

// Or using HttpClient (recommended for modern applications)
using (var client = new HttpClient())
{
    byte[] imageBytes = await client.GetByteArrayAsync(url);
    await File.WriteAllBytesAsync("avatar.png", imageBytes);
}

// Or if you are using Unity3D
IEnumerator LoadAvatar()
{
    string url = client.CurrentUser.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x128);
    UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
    yield return request.SendWebRequest();

    if (request.result == UnityWebRequest.Result.Success)
    {
        Texture2D texture = DownloadHandlerTexture.GetContent(request);
        // Use the texture (e.g., assign to a material or UI image)
    }
}
```

> [!TIP]
> You can use Async/Await in Unity3D! 
> 
> You will have much higher code readability and less frustrations when using asnchronous functions rather than coroutine hack Unity came up with.
> Use a library like [Cysharp/UniTask](https://github.com/Cysharp/UniTask) for extended functionality, support, and performance.


### Animations
If the avatar has a animation ( its hash begins with `a_` ), then you can get a link to the animated variant by using either a `GIF` or `WebP`:
```cs
string url;
if (client.CurrentUser.IsAvatarAnimated) 
{
    url = client.CurrentUser.GetAvatarURL(User.AvatarFormat.GIF, User.AvatarSize.x64);
} 
else 
{
    url = client.CurrentUser.GetAvatarURL(User.AvatarFormat.PNG, User.AvatarSize.x64);
}
```

Depending on your platform, you will need to decode and animate these files yourself.

> [!WARNING]
> If the user has a Default avatar, using anything other than PNG will throw an exception.
> 
> Check `Avatar != null` before getting urls.

## Getting the Avatar Decoration
Avatar Decorations are provided by a struct on [User.AvatarDecoration](xref:DiscordRPC.User.AvatarDecoration). These images are a set size and allows you to layer decoration on top of your users.

These decorations include both a SKU and Hash, however like with avatars there are helper functions to get the correct URL to download them from.
```cs
string decorationUrl = client.CurrentUser.GetAvatarDecorationURL();
if (decorationUrl != null) {
    DownloadDecoration(decorationUrl);
}
```

This will return `null` if the user does not have a decoration available.

### Animations
Avatar Decorations are almost always animated. However, unlike Avatars, they are animated using [Animated PNG](https://en.wikipedia.org/wiki/APNG) (APNG). 

The `GIF` variant does not exist, `JPEG` will lose the needed transparency, and `WebP` are not animated. This is a Discord limitation.

Because of this, your program must be able to parse and use `APNG` if you wish to add decorations on top of the user's avatar.

## Default Avatars
If the user does not have an avatar yet (or it was otherwise deleted), they will be assigned a Default Discord avatar. These avatars are automatically calculated by the user's ID (_or Discriminator if they have not yet moved to a global name_).

However, Discord will only support PNG for these. To prevent frustration and unexpected 404's, this library will throw a `BadImageFormatException` when the user is using a default avatar and you request a non-png image.

This guard pattern is not ideal and might be changed in the future. It is best to check if `Avatar != null` manually before trying to get animated or compressed avatars.

