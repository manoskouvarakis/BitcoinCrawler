namespace BitcoinCrawler.Model
{
    public class GdaxBitcoinPrice : BitcoinPriceBase
    {
		public override Origin Origin { get { return Origin.GDAX; } }
	}
}
