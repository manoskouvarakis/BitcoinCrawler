using BitcoinCrawler.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static BitcoinCrawler.DataAccess.RepositoryService;

namespace BitcoinCrawler.DataAccess
{
    public interface IRepositoryService
    {
		void Persist(IBitcoinPrice bitcoinPrice);
		IBitcoinPrice Retrieve(decimal value);

		decimal GetLastValue();

		decimal GetAggregatedValue(AggregateType type);
		decimal GetAggregatedValue(AggregateType type, int takeLastElementsCounter);
		decimal GetAggregatedValue(AggregateType type, Func<IBitcoinPrice, bool> filter);
		decimal GetAggregatedValue(AggregateType type, Func<IBitcoinPrice, bool> filter, int takeLastElementsCounter);
	}
}
