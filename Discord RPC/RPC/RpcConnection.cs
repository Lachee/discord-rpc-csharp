using DiscordRPC.IO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.RPC
{
	class RpcConnection : IDisposable
	{
		private PipeConnection pipe;

		private void Connect()
		{
			if (pipe == null) pipe = new PipeConnection();
			pipe.Connect();
		}

		public void Dispose()
		{
			throw new NotImplementedException();
		}
	}
}
