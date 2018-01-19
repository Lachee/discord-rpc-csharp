using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Serialization
{
	public class EnumSnakeCaseSerializer : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsEnum;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			var enumtype = existingValue.GetType();
			return Enum.Parse(enumtype, ToCamelCase((string)reader.Value));
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var enumtype = value.GetType();
			var name =	enumtype.GetEnumName(value);
			writer.WriteValue(ToSnakeCase(name));
		}

		public string ToCamelCase(string str)
		{
			return str.ToLowerInvariant().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1)).Aggregate(string.Empty, (s1, s2) => s1 + s2);
		}

		public string ToSnakeCase(string str)
		{
			return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
		}
	}
}
