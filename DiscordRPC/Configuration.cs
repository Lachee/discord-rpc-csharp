using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// Configuration of the current RPC connection
	/// </summary>
	public class Configuration
	{
		/// <summary>
		/// The Discord API endpoint that should be used.
		/// </summary>
		[JsonProperty("api_endpoint")]
		public string ApiEndpoint { get; set; }

		/// <summary>
		/// The CDN endpoint
		/// </summary>
		[JsonProperty("cdn_host")]
		public string CdnHost { get; set; }

		/// <summary>
		/// The type of environment the connection on. Usually Production. 
		/// </summary>
		[JsonProperty("environment")]
		public string Environment { get; set; }
	}
}
