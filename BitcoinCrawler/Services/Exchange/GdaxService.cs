using BitcoinCrawler.Model;
using BitcoinCrawler.Serializers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinCrawler.Exchange
{
    public class GdaxService : IGdaxService
	{
		private readonly GdaxSerializer _serializer;

		private static HttpClient gdaxClient = new HttpClient();

		public GdaxService(GdaxSerializer serializer)
		{
			this._serializer = serializer;

			String gdaxCurrencyPair = "BTC-USD";

			string baseUrl = "https://api.gdax.com";
			string method = "/products/" + gdaxCurrencyPair + "/ticker";

			gdaxClient.BaseAddress = new Uri(baseUrl + method);
			gdaxClient.DefaultRequestHeaders.Accept.Clear();
			gdaxClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			gdaxClient.DefaultRequestHeaders.Add("User-Agent", ".NET Framework Test Client");

			//For private api calls
			//String publicKey = "";
			//String passPhrase = ""
			//string ts = GetNonce();
			//string sig = GetSignature(ts, "GET", method, string.Empty);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-KEY", publicKey);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-SIGN", sig);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-TIMESTAMP", ts);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-PASSPHRASE", passPhrase);
		}

		public async Task<BitcoinPrice> GetBitcoinPriceAsync()
		{
			BitcoinPrice product = null;

			String gdaxCurrencyPair = "BTC-USD";
			string method = "/products/" + gdaxCurrencyPair + "/ticker";

			HttpResponseMessage response = await gdaxClient.GetAsync(gdaxClient.BaseAddress);
			if (response.IsSuccessStatusCode)
			{
				String json = await response.Content.ReadAsStringAsync();
				product = JsonConvert.DeserializeObject<BitcoinPrice>(json, this._serializer);
			}

			return product;
		}

		//private static string GetNonce()
		//{
		//	return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
		//}

		//private static string GetSignature(string nonce, string method, string url, string body)
		//{
		//  string secret = "";
		//	string message = string.Concat(nonce, method.ToUpper(), url, body);
		//	var encoding = new ASCIIEncoding();
		//	byte[] keyByte = Convert.FromBase64String(secret);
		//	byte[] messageBytes = encoding.GetBytes(message);
		//	using (var hmacsha256 = new HMACSHA256(keyByte))
		//	{
		//		byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
		//		return Convert.ToBase64String(hashmessage);
		//	}
		//}
	}
}
