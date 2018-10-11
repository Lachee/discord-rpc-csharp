using DiscordRPC.Helper;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace DiscordRPC.Converters
{
	/// <summary>
	/// Converts enums with the <see cref="EnumValueAttribute"/> into Json friendly terms. 
	/// </summary>
	internal class EnumSnakeCaseConverter : JsonConverter
	{
		public override bool CanConvert(Type objectType)
		{
			return objectType.IsEnum;
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) return null;

			object val = null;
			if (TryParseEnum(objectType, (string)reader.Value, out val))
				return val;

			return existingValue;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
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

			writer.WriteValue(name);
		}


		public bool TryParseEnum(Type enumType, string str, out object obj)
		{
			//Make sure the string isn;t null
			if (str == null)
			{
				obj = null;
				return false;
			}	

			//Get the real type
			Type type = enumType;
			if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
				type = type.GetGenericArguments().First();

			//Make sure its actually a enum
			if (!type.IsEnum)
			{
				obj = null;
				return false;
			}


			//Get each member and look for hte correct one
			var members = type.GetMembers(BindingFlags.Public | BindingFlags.Static);
			foreach (var m in members)
			{
				var attributes = m.GetCustomAttributes(typeof(EnumValueAttribute), true);
				foreach(var a in attributes)
				{
					var enumval = (EnumValueAttribute)a;
					if (str.Equals(enumval.Value))
					{
						obj = Enum.Parse(type, m.Name, ignoreCase: true);

						return true;
					}
				}
			}

			//We failed
			obj = null;
			return false;
		}

	}
}
