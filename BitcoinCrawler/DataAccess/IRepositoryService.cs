using BitcoinCrawler.Model;
using System;
using System.Collections.Generic;
using System.Text;
using static BitcoinCrawler.DataAccess.RepositoryService;

namespace BitcoinCrawler.DataAccess
{
    public interface IRepositoryService
    {
		void Persist(BitcoinPrice bitcoinPrice);
		BitcoinPrice Retrieve(decimal value);

		decimal GetLastValue();

		decimal GetAggregatedValue(AggregateType type);
		decimal GetAggregatedValue(AggregateType type, int takeLastElementsCounter);
		decimal GetAggregatedValue(AggregateType type, Func<BitcoinPrice, bool> filter);
		decimal GetAggregatedValue(AggregateType type, Func<BitcoinPrice, bool> filter, int takeLastElementsCounter);
	}
}
