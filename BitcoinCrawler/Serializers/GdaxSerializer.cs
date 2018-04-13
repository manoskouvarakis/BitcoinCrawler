using BitcoinCrawler.Model;
using BitcoinCrawler.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Serializers
{
	public class GdaxSerializer : JsonConverter
	{
		private readonly UnixTimeService _unixTimeService;

		public GdaxSerializer(UnixTimeService unixTimeService)
		{
			_unixTimeService = unixTimeService;
		}

		public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
		{
			throw new NotImplementedException();
		}

		public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
		{
			JObject jsonObject = JObject.Load(reader);

			BitcoinPrice configuration = new BitcoinPrice();
			configuration.Origin = BitcoinPrice.OriginType.Gdax;
			configuration.Value = (decimal)jsonObject["price"];

			DateTime temp = DateTime.Parse(jsonObject["time"].ToString());
			configuration.Timestamp = this._unixTimeService.GetUnixTimestampSeconds(temp);
			//DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			//configuration.Timestamp = (long)(temp - unixEpoch).TotalSeconds;

			return configuration;
		}

		public override bool CanConvert(Type objectType)
		{
			return typeof(BitcoinPrice).IsAssignableFrom(objectType);
		}
	}
}
