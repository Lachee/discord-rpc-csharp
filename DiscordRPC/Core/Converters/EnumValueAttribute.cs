using System;

namespace DiscordRPC.Core.Converters
{
    internal class EnumValueAttribute : Attribute
    {
        public string Value { get; set; }
        
        public EnumValueAttribute(string value) => Value = value;
    }
}