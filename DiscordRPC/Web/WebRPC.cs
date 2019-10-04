using DiscordRPC.Exceptions;
using DiscordRPC.RPC;
using DiscordRPC.RPC.Commands;
using DiscordRPC.RPC.Payload;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;

#if INCLUDE_WEB_RPC
namespace DiscordRPC.Web
{
	/// <summary>
	/// Handles HTTP Rich Presence Requests
	/// </summary>
	[System.Obsolete("Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
	public static class WebRPC
	{
		/// <summary>
		/// Sets the Rich Presence over the HTTP protocol. Does not support Join / Spectate and by default is blocking.
		/// </summary>
		/// <param name="presence">The presence to send to discord</param>
		/// <param name="applicationID">The ID of the application</param>
		/// <param name="port">The port the discord client is currently on. Specify this for testing. Will start scanning from supplied port.</param>
		/// <returns>Returns the rich presence result from the server. This can be null if presence was set to be null, or if there was no valid response from the client.</returns>
		[System.Obsolete("Setting Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
		public static RichPresence SetRichPresence(RichPresence presence, string applicationID, int port = 6463)
		{
			try
			{
				RichPresence response;
				if (TrySetRichPresence(presence, out response, applicationID, port))
					return response;
				
				return null;
			}
			catch (Exception)
			{
				throw;
			}
		}

		/// <summary>
		/// Attempts to set the Rich Presence over the HTTP protocol. Does not support Join / Specate and by default is blocking.
		/// </summary>
		/// <param name="presence">The presence to send to discord</param>
		/// <param name="response">The response object from the client</param>
		/// <param name="applicationID">The ID of the application</param>
		/// <param name="port">The port the discord client is currently on. Specify this for testing. Will start scanning from supplied port.</param>
		/// <returns>True if the response was valid from the server, otherwise false.</returns>
		[System.Obsolete("Setting Rich Presence over HTTP is no longer supported by Discord. See offical Rich Presence github for more information.")]
		public static bool TrySetRichPresence(RichPresence presence, out RichPresence response, string applicationID, int port = 6463)
		{
			//Validate the presence
			if (presence != null)
			{
				//Send valid presence
				//Validate the presence with our settings
				if (presence.HasSecrets())
					throw new BadPresenceException("Cannot send a presence with secrets as HTTP endpoint does not suppport events.");

				if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
					throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
			}

			//Iterate over the ports until the first succesfull one
			for (int p = port; p < 6472; p++)
			{
				//Prepare the url and json
				using (WebClient client = new WebClient())
				{
					try
					{
						WebRequest request = PrepareRequest(presence, applicationID, p);
						client.Headers.Add("content-type", "application/json");

						var result = client.UploadString(request.URL, request.Data);
						if (TryParseResponse(result, out response))
							return true;
					}
					catch (Exception)
					{
						//Something went wrong, but we are just going to ignore it and try the next port.
					}
				}
			}

			//we failed, return null
			response = null;
			return false;
		}

		/// <summary>
		/// Attempts to parse the response of a Web Request to a rich presence
		/// </summary>
		/// <param name="json">The json data received by the client</param>
		/// <param name="response">The parsed rich presence</param>
		/// <returns>True if the parse was succesfull</returns>
		public static bool TryParseResponse(string json, out RichPresence response)
		{
			try
			{
				//Try to parse the JSON into a event
				EventPayload ev = JsonConvert.DeserializeObject<EventPayload>(json);

				//We have a result, so parse the rich presence response and return it.
				if (ev != null)
				{
					//Parse the response into a rich presence response
					response = ev.GetObject<RichPresenceResponse>();
					return true;
				}

			}catch(Exception) { }

			//We failed.
			response = null;
			return false;
		}

		/// <summary>
		/// Prepares a struct containing data requried to make a succesful web client request to set the rich presence.
		/// </summary>
		/// <param name="presence">The rich presence to set.</param>
		/// <param name="applicationID">The ID of the application the presence belongs too.</param>
		/// <param name="port">The port the client is located on. The default port for the discord client is 6463, but it may move iteratively upto 6473 if the ports are unavailable.</param>
		/// <returns>Returns a web request containing nessary data to make a POST request</returns>
		[System.Obsolete("WebRequests are no longer supported because of the removed HTTP functionality by Discord. See offical Rich Presence github for more information.")]
		public static WebRequest PrepareRequest(RichPresence presence, string applicationID, int port = 6463)
		{
			//Validate the presence
			if (presence != null)
			{
				//Send valid presence
				//Validate the presence with our settings
				if (presence.HasSecrets())
					throw new BadPresenceException("Cannot send a presence with secrets as HTTP endpoint does not suppport events.");

				if (presence.HasParty() && presence.Party.Max < presence.Party.Size)
					throw new BadPresenceException("Presence maximum party size cannot be smaller than the current size.");
			}

			//Prepare some params
			int pid = System.Diagnostics.Process.GetCurrentProcess().Id;

			//Prepare the payload
			PresenceCommand command = new PresenceCommand() { PID = pid, Presence = presence };
			var payload = command.PreparePayload(DateTime.UtcNow.ToFileTime());

			string json = JsonConvert.SerializeObject(payload);

			string url = "http://127.0.0.1:" + port + "/rpc?v=" + RpcConnection.VERSION + "&client_id=" + applicationID;
			return new WebRequest(url, json);
		}
	}

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
#endif