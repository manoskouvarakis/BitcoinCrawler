namespace BitcoinCrawler.Model
{
	public abstract class BitcoinPriceBase : IBitcoinPrice
	{
		public abstract Origin Origin { get; }

		public long Timestamp { get; set; }

		public decimal Value { get; set; }

		public CurrencyPair Currency { get; set; }
	}
}