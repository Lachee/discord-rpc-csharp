using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DiscordRPC.Serialization
{
	public class DateTimeSerializer : JsonConverter
	{
		public override bool CanConvert(Type objectType) { return objectType == typeof(DateTime); }

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			if (reader.Value == null) return null;
			if (reader.Value is DateTime) return (DateTime)reader.Value;


			ulong seconds = 0;
			if (reader.Value is Int64)
				seconds = (ulong)(Int64)reader.Value;
			else
				seconds = ulong.Parse((string)reader.Value, NumberStyles.None, CultureInfo.InvariantCulture);
			return new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(seconds);
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			if (value == null)
			{
				writer.WriteNull();
				return;
			}
			
			double time = (TimeZoneInfo.ConvertTimeToUtc((DateTime)value) - new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc)).TotalSeconds;		
			writer.WriteValue(time.ToString(CultureInfo.InvariantCulture));
		}
	}

}
