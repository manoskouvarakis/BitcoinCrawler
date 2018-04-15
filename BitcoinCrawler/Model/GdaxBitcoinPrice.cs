using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Model
{
    public class GdaxBitcoinPrice : BitcoinPriceBase
    {
		public override OriginType Origin { get { return OriginType.Gdax; } }
	}
}
