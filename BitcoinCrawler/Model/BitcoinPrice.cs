using BitcoinCrawler.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Model
{
	public class BitcoinPrice
	{
		public enum OriginType
		{
			Gdax,
			Bitstamp,
		}

		public long Timestamp { get; set; }

		public decimal Value { get; set; }

		public OriginType Origin { get; set; }
	}
}