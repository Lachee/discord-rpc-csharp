using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
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
	class ChatServer
	{
		public const int PORT = 4200;
		public const string EOF = "<EOF>";

		private List<Room> rooms = new List<Room>();
		private TcpListener server;

		public Room GetRoom(string room)
		{
			return rooms.Where(r => r.Name.Equals(room)).First();
		}
				
		public void RemoveRoom(Room room)
		{
			Console.WriteLine("Removing Room " + room);
			rooms.Remove(room);
		}

		public void Run()
		{
			//IPHostEntry ipHostInfo = Dns.GetHostEntry(Dns.GetHostName());
			//IPAddress ipAddress = ipHostInfo.AddressList[0];

			IPAddress ipAddress = new IPAddress(new byte[] { 127, 0, 0, 1 });
			IPEndPoint localEndPoint = new IPEndPoint(ipAddress, PORT);

			// Create a TCP/IP socket.
			server = new TcpListener(localEndPoint);
			server.Start();

			while (true)
			{
				Console.WriteLine("Waiting for connnection...");
				var client = server.AcceptTcpClient();

				//Create a new user
				User user = new Server.User(client);

				//Generate a new room
				Console.WriteLine("Connection Recieved, creating new room");

				string roomName = "R_" + user.ID;
				string roomSecret = DiscordRPC.Helper.Secret.CreateFriendlySecret().Substring(0, 3);
				Room room = new Room(this, roomSecret);
				rooms.Add(room);

				//Add the user to the room
				Console.WriteLine("Adding to room");
				room.AddUser(user);
			}
		}
		
	}
}
