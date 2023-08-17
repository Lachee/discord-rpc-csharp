using System;
using System.Linq;
using System.Reflection;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace DiscordRPC.Converters
{
    /// <summary>
    /// Converts enums with the <see cref="EnumValueAttribute"/> into Json friendly terms.
    /// </summary>
    internal class EnumSnakeCaseConverter : JsonConverter<object>
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeToConvert.IsEnum;
        }

        public override object Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            object result = null;

            if (reader.TokenType == JsonTokenType.StartObject)
            {
                while (reader.Read() && reader.TokenType != JsonTokenType.EndObject)
                {
                    if (reader.TokenType == JsonTokenType.PropertyName &&
                        reader.GetString() == "Name" &&
                        reader.Read() &&
                        reader.TokenType == JsonTokenType.String)
                    {
                        var name = reader.GetString();
                        if (!string.IsNullOrWhiteSpace(name))
                            result = TryParseEnum(typeToConvert, name);
                    }
                }
            }

            return result ?? throw new InvalidOperationException($"Invalid JSON for type '{typeToConvert.FullName}'.");
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            var enumtype = value.GetType();
            var name = Enum.GetName(enumtype, value);

            //Get each member and look for hte correct one
            var members = enumtype.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                if (m.Name.Equals(name))
                {
                    var attributes = m.GetCustomAttributes(typeof(EnumValueAttribute), true);
                    if (attributes.Length > 0)
                    {
                        name = ((EnumValueAttribute)attributes[0]).Value;
                    }
                }
            }

            writer.WriteStartObject();
            writer.WriteString("Name", name);
            writer.WriteEndObject();
        }

        public object TryParseEnum(Type enumType, string str)
        {
            //Make sure the string isn;t null
            if (str == null)
            {
                return null;
            }

            //Get the real type
            Type type = enumType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
                type = type.GetGenericArguments().First();

            //Make sure its actually a enum
            if (!type.IsEnum)
            {
                return null;
            }

            //Get each member and look for hte correct one
            var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
            foreach (var m in members)
            {
                var attributes = m.GetCustomAttributes(typeof(EnumValueAttribute), true);
                foreach (var a in attributes)
                {
                    var enumval = (EnumValueAttribute)a;
                    if (str.Equals(enumval.Value))
                    {
                        return Enum.Parse(type, m.Name, ignoreCase: true);
                    }
                }
            }

            //We failed
            return null;
        }
    }
}
