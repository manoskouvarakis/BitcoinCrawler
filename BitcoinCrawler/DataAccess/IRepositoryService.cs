using BitcoinCrawler.Model;
using System;
using System.Collections.Generic;
using static BitcoinCrawler.DataAccess.RepositoryService;

namespace BitcoinCrawler.DataAccess
{
    public interface IRepositoryService
    {
		void Persist(IBitcoinPrice bitcoinPrice);
		IBitcoinPrice Retrieve(decimal value);

		Boolean IsEmpty();

		IEnumerable<CurrencyPair> GetCurrencyPairs();

		IBitcoinPrice Max(CurrencyPair currencyPair);
		IBitcoinPrice Min(CurrencyPair currencyPair);

		decimal GetLastValue(CurrencyPair currency, out TrendType trend);

		decimal GetAggregatedValue(AggregateType type, CurrencyPair currency);
		decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, int takeLastElementsCounter);
		decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, Func<IBitcoinPrice, bool> filter);
		decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, Func<IBitcoinPrice, bool> filter, int takeLastElementsCounter);
	}
}
