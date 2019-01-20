using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiscordRPC.Converters
{
	internal class EnumValueAttribute : Attribute
	{
		public string Value { get; set; }
		public EnumValueAttribute(string value)
		{
			this.Value = value;
		}
	}
}
