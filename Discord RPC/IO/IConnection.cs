using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.IO
{
	public interface IConnection : IDisposable
	{
		bool IsOpen { get; }

		bool Open();
		bool Close();
		bool Write(byte[] data);
		bool Read(out byte[] data, int length);
		void Destroy();
	}
}
