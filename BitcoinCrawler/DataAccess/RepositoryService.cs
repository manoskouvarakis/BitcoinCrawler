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


		private FixedSizedQueue<IBitcoinPrice> queue = new FixedSizedQueue<IBitcoinPrice>(10);

		public void Persist(IBitcoinPrice bitcoinPrice)
		{
			this.queue.Enqueue(bitcoinPrice);
		}

		public IBitcoinPrice Retrieve(decimal value)
		{
			return this.queue.FirstOrDefault(x => x.GetValue() == value);
		}

		public decimal GetAggregatedValue(AggregateType type)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Average(x => x.GetValue());
				case AggregateType.Max: return this.queue.Max(x => x.GetValue());
				case AggregateType.Min: return this.queue.Min(x => x.GetValue());
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.TakeLast(takeLastElementsCounter).Average(x => x.GetValue());
				case AggregateType.Max: return this.queue.TakeLast(takeLastElementsCounter).Max(x => x.GetValue());
				case AggregateType.Min: return this.queue.TakeLast(takeLastElementsCounter).Min(x => x.GetValue());
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, Func<IBitcoinPrice, bool> filter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Where(filter).Average(x => x.GetValue());
				case AggregateType.Max: return this.queue.Where(filter).Max(x => x.GetValue());
				case AggregateType.Min: return this.queue.Where(filter).Min(x => x.GetValue());
				default: return 0;
			}
		}

		public decimal GetAggregatedValue(AggregateType type, Func<IBitcoinPrice, bool> filter, int takeLastElementsCounter)
		{
			switch (type)
			{
				case AggregateType.Average: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Average(x => x.GetValue());
				case AggregateType.Max: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Max(x => x.GetValue());
				case AggregateType.Min: return this.queue.Where(filter).TakeLast(takeLastElementsCounter).Min(x => x.GetValue());
				default: return 0;
			}
		}

		public decimal GetLastValue()
		{
			return this.queue.Last().GetValue();
		}
	}
}
