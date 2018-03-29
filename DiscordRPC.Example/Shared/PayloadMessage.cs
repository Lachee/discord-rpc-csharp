using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Example.Shared
{
	class PayloadMessage : IPayload
	{
		public Opcode OP => Opcode.Message;

		[JsonProperty]
		public string Message { get; set; }

		[JsonProperty]
		public string UID { get; set; }

		public void Deserialize(string json)
		{
			var p = JsonConvert.DeserializeObject<PayloadMessage>(json);
			this.Message = p.Message;
			this.UID = p.UID;
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(this);
		}	
	}
}
