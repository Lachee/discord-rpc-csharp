using DiscordRPC.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Payload
{
	/// <summary>
	/// Used for Discord IPC Events
	/// </summary>
	public class EventPayload : IPayload
	{
		/// <summary>
		/// The data the server sent too us
		/// </summary>
		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Data { get; set; }

		/// <summary>
		/// The type of event the server sent
		/// </summary>
		[JsonProperty("evt"), JsonConverter(typeof(EnumSnakeCaseConverter))]
		public ServerEvent? Event { get; set; }

		public EventPayload() : base() { Data = null; }
		public EventPayload(long nonce) : base(nonce) { Data = null; }

		/// <summary>
		/// Sets the obejct stored within the data.
		/// </summary>
		/// <param name="obj"></param>
        [System.Obsolete("Not used and there is no reason to set the data manually.", true)]
		public void SetObject(object obj)
		{
            throw new System.NotImplementedException();
		}

		/// <summary>
		/// Gets the object stored within the Data
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetObject<T>()
		{
			if (Data == null) return default(T);
            return Data.ToObject<T>();
		}

		public override string ToString()
		{
			return "Event " + base.ToString() + ", Event: " + (Event.HasValue ? Event.ToString() : "N/A");
		}
	}
	

}
