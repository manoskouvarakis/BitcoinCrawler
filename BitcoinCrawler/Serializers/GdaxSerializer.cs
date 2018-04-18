using BitcoinCrawler.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BitcoinCrawler.Serializers
{
	public class GdaxSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);

			BitcoinPriceBase configuration = new GdaxBitcoinPrice();
			configuration.Value = (decimal)jsonObject["price"];

			DateTime temp = DateTime.Parse(jsonObject["time"].ToString());
			configuration.Timestamp = temp.GetUnixTimestampInSeconds(); 

			return configuration;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BitcoinPriceBase).IsAssignableFrom(objectType);
		}
	}
}
