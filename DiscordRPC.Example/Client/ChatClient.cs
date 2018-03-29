using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Example.Client
{
	class ChatClient
	{
		const int PORT = 4200;
		public const string EOF = "<EOF>";

		private byte[] buffer = new byte[4096];
		private string cmd;

		private Socket socket;

		public string Name { get; set; }
		public void Run()
		{
			Console.WriteLine("What is your name? ");
			Name = Console.ReadLine();

			//IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());


			try
			{
				// Create a TCP/IP socket.
				Console.WriteLine("Connecting....");

				IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
				IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);
				socket = new Socket(ipAddress.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
				socket.Connect("127.0.0.1", PORT);

				Console.WriteLine("Setting initial name...");
				SetNickname(Name);
				BeginReceive();

				Console.WriteLine("Done. Type anything to send a message.");
				while (true)
				{
					string line = Console.ReadLine();
					if (line == "exit") break;
					Send(line);
				}
			}
			catch (Exception e)
			{
				Console.WriteLine("Exception Occured: {0}", e.Message);
				Console.ReadKey();
			}
			finally
			{
				if (socket != null) socket.Dispose();
			}
		}

		#region Helpers

		public void SetNickname(string name)
		{
			this.Name = name;
			Send("/nick:" + name);
		}

		public void SetDiscord(string discord)
		{
			Send("/discord:" + discord);
		}

		#endregion

		#region Events
		private void OnMessageReceive(string message)
		{
			Console.WriteLine(message);
		}
		#endregion

		#region IO
		/// <summary>
		/// Sends a message to the server
		/// </summary>
		/// <param name="message"></param>
		public void Send(string message)
		{
			byte[] buff = Encoding.UTF8.GetBytes(message);
			this.socket.Send(buff);
		}

		/// <summary>Begins a async receive of messages from the server</summary>
		private void BeginReceive()
		{
			socket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(EndReceive), socket);
		}
		private void EndReceive(IAsyncResult result)
		{
			int bytes = this.socket.EndReceive(result);			
			cmd += Encoding.UTF8.GetString(buffer, 0, bytes);

			int endIndex = -1;
			do
			{
				endIndex = cmd.IndexOf(EOF);
				if (endIndex >= 0)
				{
					string command = cmd.Substring(0, endIndex);
					cmd = cmd.Substring(endIndex + EOF.Length);

					OnMessageReceive(command);
				}
			} while (endIndex >= 0);

			BeginReceive();
		}
		#endregion
	}
}
