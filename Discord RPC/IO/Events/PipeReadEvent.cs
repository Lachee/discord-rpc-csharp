using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.IO.Events
{
	public delegate void PipeReadEvent(object sender, PipeReadEventArgs args);
	public class PipeReadEventArgs : EventArgs
	{
		public PipeFrame Frame { get; set; }
	}
}
