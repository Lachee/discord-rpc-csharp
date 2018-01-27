using System;

namespace DiscordRPC.IO.Events
{
	public delegate void PipeErrorEvent(object sender, PipeErrorEventArgs args);
	public class PipeErrorEventArgs : EventArgs
	{
		public bool PipeClosed { get; set; }
		public Exception Exception { get; set; }
	}
}
