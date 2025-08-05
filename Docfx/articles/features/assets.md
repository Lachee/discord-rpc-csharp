---
uid: assets
---

# Assets
![List of assets](https://i.lu.je/2025/firefox_nta5I3VkPp.png)

Your Rich Presence can have a small and a large image displayed instead of its default icon called Assets.

These assets are ideal for representing maps, characters, loadouts, or other situational information about your game.

> [!TIP]
> The demo application used throughout the library has a set of testing static assets you can try out. They can be found in the [Resources](https://github.com/Lachee/discord-rpc-csharp/tree/master/Resources/Discord%20App%20Images/) folder

## Static Assets
You can upload new assets and given them a `key` in the [Discord Application](https://discord.com/developers/applications) → App → Rich Presence → Art Assets.
This is the prefered way as they are easily referenced by a key and doesn't require an external web server.

Once uploaded, you can assign them to your Activity like so:
```cs
client.SetPresence(new RichPresence()
{
    Type = ActivityType.Playing,
    Details = "A Basic Example",
    State = "In Game",
    Assets = new Assets()
    {
        LargeImageKey = "image_large",
        LargeImageText = "Lachee's Discord IPC Library",
        SmallImageKey = "koala"
    },
});
```
Which results in the image:

![Example of the images](https://i.lu.je/2025/Discord_MJrdavsJvd.png "Example of the images")
![](https://i.lu.je/2025/Discord_osR1l6A6V6.png)

> [!TIP]
> By providing `LargeImageText` or `SmallImageText`, you create a tooltip over the icon. Use this to provide additional information.


## Dynamic Assets
Discord has relaxed on its limitation with these assets and you are now able to use Dynamic Assets. These allow you to provide a link that match the requirements and Discord will use the image at that url.

For best and consistent results, it is easier to use the upload/key solution previously discussed. However, there are situations where you might want to use this such as for Album Art or user-generated content (although you should restrict this to avoid NSFW items).

> [!NOTE]
> Discord will make a request on behalf of the user. Some services will have hot-link protection and block Discord from using their images.
> You should use your own service to serve these images.

As an example, suppose I wish to display the album cover for Duran Druan's "Future Past", found on the [Music Brainz API](https://musicbrainz.org/doc/Cover_Art_Archive/API):

```cs
// omitted: 
//   HTTP GET https://coverartarchive.org/release/6bf261f9-f18b-4ba8-92be-7afc36575f2d
client.SetPresence(new RichPresence()
{
    Type = ActivityType.Listening,
    Details = "Invisible",
    State = "Duran Duran",
    Timestamps = Timestamps.FromTimeSpan(191),
	Assets = new Assets()
    {
        LargeImageKey = "https://coverartarchive.org/release/6bf261f9-f18b-4ba8-92be-7afc36575f2d/33192928727-500.jpg", // Links to the image
        LargeImageUrl = "https://music.youtube.com/watch?v=SMCd5zrsFpE" // Links to the song when clicked
        LargeImageText = "Future Past",
    },
});
```

This will load the image dynamically and display:

![Result](https://i.lu.je/2025/Discord_GOnaDQnm3J.png)

> [!WARNING]
> The maximum length of the Key field is 256 characters __regardless__ if you are using a URL of a Key.
> Make sure the URL you provide is within this limit. You may need to provide a URL shortener/proxy for your application.

