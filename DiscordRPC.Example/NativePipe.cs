using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace DiscordRPC.Example
{
	class NativePipe
	{
		[DllImport("DiscordRPC.Native.dll", EntryPoint = "fnGetValue")]
		internal static extern int GetValue();

		[DllImport("DiscordRPC.Native.dll", EntryPoint = "fnSetValue")]
		internal static extern void SetValue(int value);
	}
}
