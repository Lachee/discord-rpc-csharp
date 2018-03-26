using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordRPC.Example
{
	class NativePipe
	{
		[DllImport("DiscordRPC.Native.dll", EntryPoint = "fnDiscordRPCNative")]
		internal static extern int Example();
	}
}
