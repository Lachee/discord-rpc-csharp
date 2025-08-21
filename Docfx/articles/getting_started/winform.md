# WinForm
Here are some additional notes about WinForm and getting Rich Presence into these type of applications.

## Timers & Event Loops
To get thread safe events, manually use a timer.
```csharp
var timer = new System.Timers.Timer(150);
timer.Elapsed += (sender, args) => { client.Invoke(); };
timer.Start();
```