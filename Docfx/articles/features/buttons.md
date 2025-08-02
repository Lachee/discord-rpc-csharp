---
uid: buttons
---

# Buttons

Everyone's favourite, buttons! Buttons allow you to display clickable links inside your presence. They take an external URL and will redirect the user to it on visit.

> [!NOTE]
> You are able to display **2** buttons at once as part of your presence.

> [!WARNING]
> You are unable to view buttons or click on them for your own activity. Only other users can see and click on the buttons.

<img src="https://i.lu.je/2025/Discord_QSPFYPuoHT.png" style="width: 35%; float: left">

```cs
client.SetPresence(new RichPresence()
{
    Details = "A Basic Example",
    State = "In Game",
    Buttons = new Button[]
    {
        new Button() { Label = "Fish", Url = "https://lachee.dev/" },
        new Button() { Label = "Sticks", Url = "https://en.wikipedia.org/wiki/Stick" }
	}
});
```
