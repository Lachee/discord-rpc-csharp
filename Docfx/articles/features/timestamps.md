---
uid: timestamps
---

# Timestamps
You can display timers and progress in your activity. However, Discord likes to change their mind on how these are displayed. Below is a matrix of all the variety of timers you can set and what they will do for each @activity_types:

| Activity Type / Timestamps | `null` | `Timestamps.Now` | `Timestamps.FromTimespan` |
|---------------|------------------|------------|--------------|
| Playing | ![](https://i.lu.je/2025/Discord_IAvcJCJ0N6.png) | ![](https://i.lu.je/2025/Discord_tdvadyQGNf.png) | ![](https://i.lu.je/2025/Discord_obmYyhKhfN.png) |
| Listening | ![](https://i.lu.je/2025/Discord_jTfyj05Npz.png) | ![](https://i.lu.je/2025/Discord_0kqFsY7DUo.png) | ![](https://i.lu.je/2025/Discord_nFVsnVgPm5.png) |
| Watching | ![](https://i.lu.je/2025/Discord_P81U0PWtc9.png) | ![](https://i.lu.je/2025/Discord_P81U0PWtc9.png) | ![](https://i.lu.je/2025/Discord_5SPwBlHK2Q.png) |
| Competing | ![](https://i.lu.je/2025/Discord_ZGju3yetFZ.png) | ![](https://i.lu.je/2025/Discord_7AcYhUz3hp.png) | ![](https://i.lu.je/2025/Discord_XTNQLFWbsE.png) |

Your control over what is displayed is limited. You use to be able to set a count down timer in a Playing activity, but Discord has since removed that. Now the only purpose of setting a timestamp is for Listening and Watching activities.

As an example, the following snippet of code will make it as if you are playing a Video.

<table>
    <tr>
        <td><img src="https://i.lu.je/2025/Discord_GzHlOd9WUE.png" alt="Users activity saying 'Watching Skibidi Toilet'"></td>
        <td><img src="https://i.lu.je/2025/Discord_oiR7lMxial.png" alt="Users rich presence saying 'Watching Discord IPC.NET' and a timeline"></td>
    </tr>
</table>


```cs
client.SetPresence(new RichPresence()
{
    Type = ActivityType.Watching,
    StatusDisplay = StatusDisplayType.Details,
    Details = "Skibidi Toilet - Season 1",
    State = "DaFuq!?Boom!",
    Timestamps = Timestamps.FromTimeSpan(66),
});
```

> [!TIP]
> This library provides a variety of ways to set the time. 
> The @DiscordRPC.Timestamps will allow you to use seconds, or a `System.DateTime.TimeSpan`.