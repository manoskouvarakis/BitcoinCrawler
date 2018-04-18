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
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinCrawler.Harvesting
{
    public class GdaxService : IHarvestingService
	{
		private readonly GdaxSerializer _serializer;
		private readonly GdaxServiceOptions _gdaxServiceOptions;
		private readonly ILogger _logger;

		private static HttpClient gdaxClient = new HttpClient();

		public GdaxService(GdaxSerializer serializer, IOptions<GdaxServiceOptions> gdaxServiceOptions, ILogger<GdaxService> logger)
		{
			this._serializer = serializer;
			this._gdaxServiceOptions = gdaxServiceOptions.Value;
			this._logger = logger;

			gdaxClient.BaseAddress = new Uri(this._gdaxServiceOptions.BaseUrl);

			gdaxClient.DefaultRequestHeaders.Accept.Clear();
			gdaxClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			gdaxClient.DefaultRequestHeaders.Add("User-Agent", ".NET Framework Test Client");
		}

		public async Task<IBitcoinPrice> GetPriceAsync(HarvestTask task)
		{
			if (task.Origin != Origin.GDAX)
			{
				this._logger.LogError("Task exchange service differs");
				return null;
			}

			string method = "products/" + this.APICurrencyId[task.Pair] + "/ticker";

			this._logger.LogInformation("Fetch new price");
			HttpResponseMessage response = await gdaxClient.GetAsync(method);
			if (!response.IsSuccessStatusCode)
			{
				this._logger.LogError("Error response from exchange API");
				return null;
			}

			String json = await response.Content.ReadAsStringAsync();
			GdaxBitcoinPrice price = JsonConvert.DeserializeObject<GdaxBitcoinPrice>(json, this._serializer);
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
					this.mAPICurrencyId.Add(CurrencyPair.USDBTC, "BTC-USD");
					this.mAPICurrencyId.Add(CurrencyPair.EURBTC, "BTC-EUR");
				}
				return this.mAPICurrencyId;
			}
		}

		//The code below is for private api calls.
		//You should add the user public key and the passphrase of gdax api
		private void InitializeClientForPrivateAPICall(String method)
		{
			String publicKey = "";
			String passPhrase = "";
			string ts = this.GetNonce();
			string sig = GetSignature(ts, "GET", method, string.Empty);
			gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-KEY", publicKey);
			gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-SIGN", sig);
			gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-TIMESTAMP", ts);
			gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-PASSPHRASE", passPhrase);
		}

		private string GetNonce()
		{
			return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
		}

		private string GetSignature(string nonce, string method, string url, string body)
		{
			string secret = "";
			string message = string.Concat(nonce, method.ToUpper(), url, body);
			var encoding = new ASCIIEncoding();
			byte[] keyByte = Convert.FromBase64String(secret);
			byte[] messageBytes = encoding.GetBytes(message);
			using (var hmacsha256 = new HMACSHA256(keyByte))
			{
				byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
				return Convert.ToBase64String(hashmessage);
			}
		}
	}
}
