using DiscordRPC.Exceptions;
using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;
using System.Text;

namespace DiscordRPC
{
	/// <summary>
	/// A Rich Presence button.
	/// </summary>
	public class Button
	{
		/// <summary>
		/// Text shown on the button
		/// <para>Max 31 bytes.</para>
		/// </summary>
		[JsonProperty("label")]
		public string Label
		{
			get { return _label; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _label, true, 31, Encoding.UTF8))
					throw new StringOutOfRangeException(31);
			}
		}
		private string _label;

		/// <summary>
		/// The URL opened when clicking the button.
		/// <para>Max 512 characters.</para>
		/// </summary>
		[JsonProperty("url")]
		public string Url
		{
			get { return _url; }
			set
			{
				if (!BaseRichPresence.ValidateString(value, out _url, false, 512))
					throw new StringOutOfRangeException(512);

				if (!BaseRichPresence.ValidateUrl(_url))
					throw new ArgumentException("Url must be a valid URI");
			}
		}
		private string _url;
	}

}
