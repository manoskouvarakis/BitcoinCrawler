using BitcoinCrawler.Model;
using BitcoinCrawler.Options;
using BitcoinCrawler.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BitcoinCrawler.Harvesting
{
	public class BitstampService : IHarvestingService
	{
		private readonly BitstampSerializer _serializer;
		private readonly BitstampServiceOptions _bitstampServiceOptions;
		private readonly ILogger _logger;

		private static HttpClient bitstampClient = new HttpClient();

		public BitstampService(BitstampSerializer serializer, IOptions<BitstampServiceOptions> bitstampServiceOptions, ILogger<BitstampService> logger)
		{
			this._serializer = serializer;
			this._bitstampServiceOptions = bitstampServiceOptions.Value;
			this._logger = logger;

			bitstampClient.BaseAddress = new Uri(this._bitstampServiceOptions.BaseUrl);

			bitstampClient.DefaultRequestHeaders.Accept.Clear();
			bitstampClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
		}

		public async Task<IBitcoinPrice> GetPriceAsync(HarvestTask task)
		{
			if (task.Origin != Origin.Bitstamp)
			{
				this._logger.LogError("Task exchange service differs");
				return null;
			}

			String method = this.APICurrencyId[task.Pair] + '/';

			this._logger.LogInformation("Fetch new price");
			HttpResponseMessage response = await bitstampClient.GetAsync(method);
			if (!response.IsSuccessStatusCode)
			{
				this._logger.LogError("Error response from exchange API");
				return null;
			}

			String json = await response.Content.ReadAsStringAsync();
			BitstampBitcoinPrice price = JsonConvert.DeserializeObject<BitstampBitcoinPrice>(json, this._serializer);
			price.Currency = task.Pair;

			return price;
		}

		private Dictionary<CurrencyPair, String> mAPICurrencyId = null;
		private Dictionary<CurrencyPair, String> APICurrencyId
		{
			get
			{
				if (this.mAPICurrencyId == null)
				{
					this.mAPICurrencyId = new Dictionary<CurrencyPair, String>();
					this.mAPICurrencyId.Add(CurrencyPair.USDBTC, "btcusd");
					this.mAPICurrencyId.Add(CurrencyPair.EURBTC, "btceur");
				}
				return this.mAPICurrencyId;
			}
		}
	}
}
