using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Web
{
	/// <summary>
	/// Details of a HTTP Post request that will set the rich presence.
	/// </summary>
	[System.Obsolete("Web Requests is no longer supported as Discord removed HTTP Rich Presence support. See offical Rich Presence github for more information.")]
	public struct WebRequest
	{
		private string _url;
		private string _json;
		private Dictionary<string, string> _headers;

		/// <summary>
		/// The URL to send the POST request too
		/// </summary>
		public string URL { get { return _url; } }

		/// <summary>
		/// The JSON formatted body to send with the POST request
		/// </summary>
		public string Data { get { return _json; } }

		/// <summary>
		/// The headers to send with the body
		/// </summary>
		public Dictionary<string, string> Headers { get { return _headers; } }

		internal WebRequest(string url, string json)
		{
			_url = url;
			_json = json;
			_headers = new Dictionary<string, string>();
			_headers.Add("content-type", "application/json");
		}
	}
}
