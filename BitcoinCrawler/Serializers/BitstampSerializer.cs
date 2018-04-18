using BitcoinCrawler.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace BitcoinCrawler.Serializers
{
	public class BitstampSerializer : JsonConverter
	{
		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);

			BitcoinPriceBase configuration = new BitstampBitcoinPrice();
			configuration.Timestamp = (long)jsonObject["timestamp"];
			configuration.Value = (decimal)jsonObject["last"];

			return configuration;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BitcoinPriceBase).IsAssignableFrom(objectType);
		}
	}
}
