using DiscordRPC.Example.Shared;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

/*
 * 
 *             OUT OF SCOPE
 * ==================================
 * 
 * Dont worry about how this server works. It has nothing to do with discord
 *  and only servers as an example master server.
 * 
*/

namespace DiscordRPC.Example.Server
{
	class User : IDisposable
	{
		public string ID { get; }
		public string Name { get; set; }
		public string Discord { get; set; }
		public string DisplayName {  get { return Discord == null ? Name : "@" + Discord; } }

		public Room CurrentRoom { get; set; }
		private TcpClient client;

		private byte[] buffer = new byte[4096];

		public User(TcpClient client)
		{
			this.client = client;
			this.ID = DiscordRPC.Helper.Secret.CreateFriendlySecret().Substring(0, 5);
			this.Name = "New User";

			client.GetStream();
		}

		public void Send(IPayload payload)
		{
			using (MemoryStream stream = new MemoryStream())
			{
				//Convert the serialized payload
				string json = payload.Serialize();
				byte[] msg = Encoding.UTF8.GetBytes(json);

				//Convert the length of the message and the op code to bytes
				byte[] op = BitConverter.GetBytes((uint)payload.OP);
				byte[] len = BitConverter.GetBytes((uint)(msg.Length + op.Length));
				if (!BitConverter.IsLittleEndian)
				{
					//Reverse all our generated arrays
					Array.Reverse(op);
					Array.Reverse(len);
				}

				//Write the length, opcode and then message
				stream.Write(len, 0, len.Length);
				stream.Write(op, 0, op.Length);
				stream.Write(msg, 0, msg.Length);

				//WRite the stream array
				byte[] buff = stream.ToArray();
				this.client.GetStream().Write(buff, 0, buff.Length);
			}
		}

		private void BeginReceive()
		{
			this.client.GetStream().BeginRead(buffer, 0, buffer.Length, new AsyncCallback(OnMessageRead), this.client);
		}

		private void OnMessageRead(IAsyncResult result)
		{
			try
			{
				//Read in the bytes to a memory stream
				int bytes = this.client.GetStream().EndRead(result);
				using (MemoryStream stream = new MemoryStream(buffer, 0, bytes))
				{
					//Read the length of thigns
					byte[] blen = new byte[4];
					byte[] bop = new byte[4];

					stream.Read(blen, 0, 4);
					stream.Read(bop, 0, 4);

					//flip them if required
					if (!BitConverter.IsLittleEndian)
					{
						Array.Reverse(blen);
						Array.Reverse(bop);
					}

					//Conver them
					uint len = BitConverter.ToUInt32(blen, 0);
					uint op = BitConverter.ToUInt32(bop, 0);

					byte[] data = new byte[len - 4];
					stream.Read(data, 0, data.Length);

					string json = Encoding.UTF8.GetString(data);

					//Convert them to appropriate payloads
					if (CurrentRoom != null)
					{
						CurrentRoom.OnPayload(this, new Frame() { OP = (Opcode)op, Json = json });
					}
				}

					BeginReceive();
			}
			catch (ObjectDisposedException)
			{
				Console.WriteLine("Client Disposed");
				CurrentRoom.OnDisconnect(this);
			}
			catch (InvalidOperationException)
			{
				Console.WriteLine("Client Invalid");
				CurrentRoom.OnDisconnect(this);
			}
		}

		public void Dispose()
		{
			if (client != null)
			{
				client.Close();
				client = null;
			}
		}
	}
}
