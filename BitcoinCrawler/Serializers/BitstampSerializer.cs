using BitcoinCrawler.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

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

			BitcoinPrice configuration = new BitcoinPrice();
			configuration.Origin = BitcoinPrice.OriginType.Bitstamp;
			configuration.Timestamp = (long)jsonObject["timestamp"];
			configuration.Value = (decimal)jsonObject["last"];

			return configuration;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BitcoinPrice).IsAssignableFrom(objectType);
		}
	}
}
