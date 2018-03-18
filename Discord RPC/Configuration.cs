using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC
{
	public class Configuration
	{
		[JsonProperty("api_endpoint")]
		public string ApiEndpoint { get; set; }

		[JsonProperty("cdn_host")]
		public string CdnHost { get; set; }

		[JsonProperty("enviroment")]
		public string Enviroment { get; set; }
	}
}
