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

			//String gdaxCurrencyPair = "BTC-USD";

			//string method = "/products/" + gdaxCurrencyPair + "/ticker";
			//string ts = GetNonce();
			//string sig = GetSignature(ts, "GET", method, string.Empty);

			//gdaxClient.BaseAddress = new Uri("https://api.gdax.com/" + method);
			//gdaxClient.DefaultRequestHeaders.Accept.Clear();
			//gdaxClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-KEY", config_API_Key);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-SIGN", sig);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-TIMESTAMP", ts);
			//gdaxClient.DefaultRequestHeaders.Add("CB-ACCESS-PASSPHRASE", config_API_Passphrase);
			//gdaxClient.DefaultRequestHeaders.Add("User-Agent", UserAgent);
		}

		public async Task<BitcoinPrice> GetBitcoinPriceAsync()
		{
			//BitcoinPrice product = null;
			//HttpResponseMessage response = await gdaxClient.GetAsync(gdaxClient.BaseAddress);
			//if (response.IsSuccessStatusCode)
			//{
			//	String json = await response.Content.ReadAsStringAsync();
			//	product = JsonConvert.DeserializeObject<BitcoinPrice>(json, new BitcoinPrice.BitstampSerializer());
			//}
			//return product;

			BitcoinPrice product = null;

			try
			{
				String URL = "http://api.gdax.com/products/BTC-USD/ticker";
				HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);
				request.UserAgent = ".NET Framework Test Client";
				WebResponse response = await request.GetResponseAsync();
				var encoding = ASCIIEncoding.ASCII;
				using (var reader = new System.IO.StreamReader(response.GetResponseStream(), encoding))
				{
					string responseText = reader.ReadToEnd();
					product = JsonConvert.DeserializeObject<BitcoinPrice>(responseText, this._serializer);
					return product;
				}
			}
			catch (WebException ex)
			{
				HttpWebResponse xyz = ex.Response as HttpWebResponse;
				var encoding = ASCIIEncoding.ASCII;
				using (var reader = new System.IO.StreamReader(xyz.GetResponseStream(), encoding))
				{
					string responseText = reader.ReadToEnd();
				}
			}

			return product;
		}

		//private static string GetNonce()
		//{
		//	return (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds.ToString();
		//}

		//private static string GetSignature(string nonce, string method, string url, string body)
		//{
		//	string message = string.Concat(nonce, method.ToUpper(), url, body);
		//	var encoding = new ASCIIEncoding();
		//	byte[] keyByte = Convert.FromBase64String(config_API_Secret);
		//	byte[] messageBytes = encoding.GetBytes(message);
		//	using (var hmacsha256 = new HMACSHA256(keyByte))
		//	{
		//		byte[] hashmessage = hmacsha256.ComputeHash(messageBytes);
		//		return Convert.ToBase64String(hashmessage);
		//	}
		//}
	}
}
