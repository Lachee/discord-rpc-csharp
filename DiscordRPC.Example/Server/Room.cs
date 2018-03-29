using DiscordRPC.Example.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
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
	class Room
	{
		/// <summary>
		/// The name of the room
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// The secret join password
		/// </summary>
		public string Secret => _secret;
		private string _secret;

		private List<User> _users;
		private ChatServer _server;

		public Room(ChatServer server, string secret)
		{
			this._server = server;
			this._secret = secret;

			_users = new List<User>();
		}
		
		#region Users
		public void AddUser(User user)
		{
			user.CurrentRoom = this;
			_users.Add(user);
		}
		public bool RemoveUser(User user)
		{
			_users.Remove(user);
			user.CurrentRoom = null;
			return _users.Count > 0;
		}
		#endregion

		#region Messages

		public void SendMessage(User whisper, string message)
		{
			PayloadMessage msg = new PayloadMessage() { Message = message, UID = "room" };
			Whisper(whisper, msg);
		}
		public void SendMessage(string message)
		{
			PayloadMessage msg = new PayloadMessage() { Message = message, UID = "room" };
			Broadcast(msg);
		}

		#endregion

		#region Broadcasting
		private void Whisper(User user, IPayload payload) => user.Send(payload);
		private void Broadcast(IPayload payload)
		{
			for (int i = 0; i < _users.Count; i++)
				Whisper(_users[i], payload);
		}
		#endregion

		#region User Events
		public void OnDisconnect(User user)
		{
			if (!RemoveUser(user))
				_server.RemoveRoom(this);

			user.Dispose();
		}
		public void OnPayload(User user, Frame frame)
		{
			switch (frame.OP)
			{
				default:
					Console.WriteLine("Unkown Opcode");
					OnDisconnect(user);
					break;
					

				case Opcode.Message:
					//We received a message, so just send it back to everyone
					PayloadMessage msg = new PayloadMessage();
					msg.Deserialize(frame.Json);
					Broadcast(msg);
					break;
					

			}
			
		}
		#endregion
	}
}
