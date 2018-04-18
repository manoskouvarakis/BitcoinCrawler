namespace BitcoinCrawler.Model
{
    public interface IBitcoinPrice
    {
		decimal Value { get; set; }

		long Timestamp { get; set; }

		CurrencyPair Currency { get; set; }

		Origin Origin { get; }
	}
}
