using System;

namespace DiscordRPC.IO.Events
{
	internal delegate void PipeReadEvent(object sender, PipeReadEventArgs args);
	internal class PipeReadEventArgs : EventArgs
	{
		internal PipeFrame Frame { get; set; }
	}
}
