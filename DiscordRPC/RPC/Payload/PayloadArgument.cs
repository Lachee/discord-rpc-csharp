using DiscordRPC.Converters;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace DiscordRPC.RPC.Payload
{
	/// <summary>
	/// The payload that is sent by the client to discord for events such as setting the rich presence.
	/// <para>
	/// SetPrecense
	/// </para>
	/// </summary>
	internal class ArgumentPayload : IPayload
	{
		/// <summary>
		/// The data the server sent too us
		/// </summary>
		[JsonProperty("args", NullValueHandling = NullValueHandling.Ignore)]
		public JObject Arguments { get; set; }
		
		public ArgumentPayload() { Arguments = null; }
		public ArgumentPayload(long nonce) : base(nonce) { Arguments = null; }
		public ArgumentPayload(object args, long nonce) : base(nonce)
		{
			SetObject(args);
		}

		/// <summary>
		/// Sets the obejct stored within the data.
		/// </summary>
		/// <param name="obj"></param>
		public void SetObject(object obj)
		{
			Arguments = JObject.FromObject(obj);
		}

		/// <summary>
		/// Gets the object stored within the Data
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <returns></returns>
		public T GetObject<T>()
		{
			return Arguments.ToObject<T>();
		}

		public override string ToString()
		{
			return "Argument " + base.ToString();
		}
	}
}

