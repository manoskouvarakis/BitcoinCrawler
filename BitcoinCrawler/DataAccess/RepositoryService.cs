using BitcoinCrawler.Helpers;
using BitcoinCrawler.Model;
using BitcoinCrawler.Options;
using Microsoft.Extensions.Options;
using MoreLinq;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace BitcoinCrawler.DataAccess
{
    public class RepositoryService : IRepositoryService
    {
		public enum AggregateType
		{
			Min,
			Max,
			Average
		}

		public enum TrendType
		{
			Up,
			Down
		}

		private readonly RepositoryServiceOptions _repositoryServiceOptions;

		public RepositoryService(IOptions<RepositoryServiceOptions> repositoryServiceOptions)
		{
			this._repositoryServiceOptions = repositoryServiceOptions.Value;
		}

		private ConcurrentDictionary<CurrencyPair, FixedSizedQueue<IBitcoinPrice>> inMemoryStorage = new ConcurrentDictionary<CurrencyPair, FixedSizedQueue<IBitcoinPrice>>();
		private ConcurrentDictionary<CurrencyPair, decimal> lastPricePerCurrency = new ConcurrentDictionary<CurrencyPair, decimal>();

		public void Persist(IBitcoinPrice bitcoinPrice)
		{
			FixedSizedQueue<IBitcoinPrice> newQueue = new FixedSizedQueue<IBitcoinPrice>(this._repositoryServiceOptions.MemorySize);
			newQueue.Enqueue(bitcoinPrice);
			this.inMemoryStorage.AddOrUpdate(bitcoinPrice.Currency, newQueue,
				(key, existingVal) =>
				{
					existingVal.Enqueue(bitcoinPrice);
					return existingVal;
				});
		}

		public IEnumerable<CurrencyPair> GetCurrencyPairs()
		{
			return this.inMemoryStorage.Keys;
		}

		public Boolean IsEmpty()
		{
			return this.inMemoryStorage.IsEmpty;
		}

		public IBitcoinPrice Max(CurrencyPair currencyPair)
		{
			return this.inMemoryStorage[currencyPair].MaxBy(x => x.Value);
		}

		public IBitcoinPrice Min(CurrencyPair currencyPair)
		{
			return this.inMemoryStorage[currencyPair].MinBy(x => x.Value);
		}

		public decimal GetAggregatedValue(AggregateType type, CurrencyPair currency)
		{
			switch (type)
			{
				case AggregateType.Average: return this.inMemoryStorage[currency].Average(x => x.Value);
				case AggregateType.Max: return this.inMemoryStorage[currency].Max(x => x.Value);
				case AggregateType.Min: return this.inMemoryStorage[currency].Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.inMemoryStorage[currency].TakeLast(takeLastElementsCounter).Average(x => x.Value);
				case AggregateType.Max: return this.inMemoryStorage[currency].TakeLast(takeLastElementsCounter).Max(x => x.Value);
				case AggregateType.Min: return this.inMemoryStorage[currency].TakeLast(takeLastElementsCounter).Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, Func<IBitcoinPrice, bool> filter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).Average();
				case AggregateType.Max: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).Max();
				case AggregateType.Min: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).Min();
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, CurrencyPair currency, Func<IBitcoinPrice, bool> filter, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).AsQueryable().TakeLast(takeLastElementsCounter).Average();
				case AggregateType.Max: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).AsQueryable().TakeLast(takeLastElementsCounter).Max();
				case AggregateType.Min: return this.inMemoryStorage[currency].Where(filter).Select(x => x.Value).DefaultIfEmpty(0).AsQueryable().TakeLast(takeLastElementsCounter).Min();
				default: return 0;
			}
		}

		public decimal GetLastValue(CurrencyPair currency, out TrendType trend)
		{
			decimal newPrice = this.inMemoryStorage[currency].Last().Value;

			bool tryGetValue = this.lastPricePerCurrency.TryGetValue(currency, out decimal value);

			trend = tryGetValue ? newPrice > this.lastPricePerCurrency[currency] ? TrendType.Up : TrendType.Down : TrendType.Up;

			this.lastPricePerCurrency.AddOrUpdate(currency, newPrice,
				(key, existingVal) =>
				{
					existingVal = newPrice;
					return existingVal;
				});

			return newPrice;
		}
	}
}
