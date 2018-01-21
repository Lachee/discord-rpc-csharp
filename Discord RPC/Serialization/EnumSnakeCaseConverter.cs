using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Serialization
{
	public class EnumSnakeCaseConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsEnum;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) return null;
			return TryParseEnum(objectType, (string)reader.Value);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			var enumtype = value.GetType();
			var name =	enumtype.GetEnumName(value);
			writer.WriteValue(ToSnakeCase(name));
		}


		public object TryParseEnum(Type enumType, string str)
		{
			if (str == null)
				return null;

			Type type = enumType;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = type.GetGenericArguments().First();

			string line = ToCamelCase(str);
			return Enum.Parse(type, line, ignoreCase: true);
		}
		
		public string ToCamelCase(string str)
		{
			if (str == null) return null;
			return str.ToLowerInvariant().Split(new[] { "_" }, StringSplitOptions.RemoveEmptyEntries).Select(s => char.ToUpperInvariant(s[0]) + s.Substring(1, s.Length - 1)).Aggregate(string.Empty, (s1, s2) => s1 + s2);
		}

		public string ToSnakeCase(string str)
		{
			if (str == null) return null;
			return string.Concat(str.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString())).ToUpper();
		}
	}
}
