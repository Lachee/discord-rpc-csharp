using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	internal interface IConnection : IDisposable
	{
		bool IsOpen { get; }

		bool Open();
		bool Close();
		bool Write(byte[] data);
		int Read(byte[] data, int length);
	}
}
