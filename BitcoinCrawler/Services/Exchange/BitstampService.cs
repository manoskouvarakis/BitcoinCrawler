using BitcoinCrawler.Model;
using BitcoinCrawler.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinCrawler.Exchange
{
    public class BitstampService : IBitstampService
    {
		private readonly BitstampSerializer _serializer;

		private static HttpClient bitstampClient = new HttpClient();

		public BitstampService(BitstampSerializer serializer)
		{
			this._serializer = serializer;

			String bitstampCurrencyPair = "btcusd";

			bitstampClient.BaseAddress = new Uri("https://www.bitstamp.net/api/v2/ticker/" + bitstampCurrencyPair + '/');
			bitstampClient.DefaultRequestHeaders.Accept.Clear();
			bitstampClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<IBitcoinPrice> GetBitcoinPriceAsync()
		{
			IBitcoinPrice product = null;

			HttpResponseMessage response = await bitstampClient.GetAsync(bitstampClient.BaseAddress);
			if (response.IsSuccessStatusCode)
			{
				String json = await response.Content.ReadAsStringAsync();
				product = JsonConvert.DeserializeObject<BitstampBitcoinPrice>(json, this._serializer);
			}

			return product;
		}
	}
}
