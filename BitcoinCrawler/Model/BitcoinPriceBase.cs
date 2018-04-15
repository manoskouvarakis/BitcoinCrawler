using BitcoinCrawler.Services;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Model
{
	public abstract class BitcoinPriceBase : IBitcoinPrice
	{
		public enum OriginType
		{
			Gdax,
			Bitstamp,
		}

		public long Timestamp { get; set; }

		public decimal Value { get; set; }

		public abstract OriginType Origin { get; }

		public decimal GetValue()
		{
			return this.Value;
		}

		public long GetUnixTime()
		{
			return this.Timestamp;
		}
	}
}