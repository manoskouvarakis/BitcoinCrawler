using BitcoinCrawler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using static BitcoinCrawler.Extensions;

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


		private FixedSizedQueue<BitcoinPrice> queue = new FixedSizedQueue<BitcoinPrice>(10);

		public void Persist(BitcoinPrice bitcoinPrice)
		{
			this.queue.Enqueue(bitcoinPrice);
		}

		public BitcoinPrice Retrieve(decimal value)
		{
			return this.queue.FirstOrDefault(x => x.Value == value);
		}

		public decimal GetAggregatedValue(AggregateType type)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Average(x => x.Value);
				case AggregateType.Max: return this.queue.Max(x => x.Value);
				case AggregateType.Min: return this.queue.Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.TakeLast(takeLastElementsCounter).Average(x => x.Value);
				case AggregateType.Max: return this.queue.TakeLast(takeLastElementsCounter).Max(x => x.Value);
				case AggregateType.Min: return this.queue.TakeLast(takeLastElementsCounter).Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, Func<BitcoinPrice, bool> filter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Where(filter).Average(x => x.Value);
				case AggregateType.Max: return this.queue.Where(filter).Max(x => x.Value);
				case AggregateType.Min: return this.queue.Where(filter).Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, Func<BitcoinPrice, bool> filter, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Average(x => x.Value);
				case AggregateType.Max: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Max(x => x.Value);
				case AggregateType.Min: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Min(x => x.Value);
				default: return 0;
			}
		}

		public decimal GetLastValue()
		{
			return this.queue.Last().Value;
		}
	}
}
