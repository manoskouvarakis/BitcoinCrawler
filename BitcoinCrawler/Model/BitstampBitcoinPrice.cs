namespace BitcoinCrawler.Model
{
	public class BitstampBitcoinPrice : BitcoinPriceBase
	{
		public override Origin Origin { get { return Origin.Bitstamp; } }
	}
}
