using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Model
{
	public class BitstampBitcoinPrice : BitcoinPriceBase
	{
		public override OriginType Origin { get { return OriginType.Gdax; } }
	}
}
