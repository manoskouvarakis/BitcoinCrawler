using BitcoinCrawler;
using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Model;
using BitcoinCrawler.Options;
using Microsoft.Extensions.Options;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;
using static BitcoinCrawler.DataAccess.RepositoryService;

namespace BitcoinCrawlerTests
{
    public class RepositoryServiceTests
    {
		private readonly RepositoryService _repositoryService;

		public RepositoryServiceTests()
		{
			var mockOptions = Mock.Of<IOptions<RepositoryServiceOptions>>(m =>
					m.Value.MemorySize == 8);

			this._repositoryService = new RepositoryService(mockOptions);
		}

		[Fact]
		public void Test_Last_Added_Value()
		{
			this.InitializeRepositoryWithTestData();

			IBitcoinPrice price = new GdaxBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 1, Timestamp = DateTime.UtcNow.AddMinutes(-1).GetUnixTimestampInSeconds() };
			this._repositoryService.Persist(price);

			decimal lastAddedValue = this._repositoryService.GetLastValue(price.Currency, out TrendType trend);

			Assert.Equal(price.Value, lastAddedValue);
		}

		[Fact]
		public void Test_Max_Value()
		{
			this.InitializeRepositoryWithTestData();

			IBitcoinPrice maxPrice = this._repositoryService.Max(CurrencyPair.USDBTC);

			Assert.Equal(5, maxPrice.Value);
			Assert.Equal(CurrencyPair.USDBTC, maxPrice.Currency);
		}

		[Fact]
		public void Test_Min_Value()
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			IBitcoinPrice minPrice = this._repositoryService.Min(CurrencyPair.USDBTC);

			Assert.Equal(testData.Min(x => x.Value), minPrice.Value);
			Assert.Equal(CurrencyPair.USDBTC, minPrice.Currency);
		}

		[Fact]
		public void Test_Aggregate_Average_Value()
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Average, CurrencyPair.USDBTC);

			Assert.Equal(testData.Average(x => x.Value), aggregateValue);
		}

		[Fact]
		public void Test_Aggregate_Min_Value()
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Min, CurrencyPair.USDBTC);

			Assert.Equal(testData.Min(x => x.Value), aggregateValue);
		}

		[Fact]
		public void Test_Aggregate_Max_Value()
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Max, CurrencyPair.USDBTC);

			Assert.Equal(testData.Max(x => x.Value), aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		public void Test_Aggregate_Average_Value_Last_X_Items(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Average, CurrencyPair.USDBTC, last);

			Assert.Equal(testData.TakeLast(last).Average(x => x.Value), aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		public void Test_Aggregate_Min_Value_Last_X_Items(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Min, CurrencyPair.USDBTC, last);

			Assert.Equal(testData.TakeLast(last).Min(x => x.Value), aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(3)]
		public void Test_Aggregate_Max_Value_Last_X_Items(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Max, CurrencyPair.USDBTC, last);

			Assert.Equal(testData.TakeLast(last).Max(x => x.Value), aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(10)]
		public void Test_Aggregate_Average_Value_Last_X_Minutes(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Average, CurrencyPair.USDBTC, x => x.Timestamp.BelongsToLastMinutes(last));

			decimal expectedResult = testData.Where(x => x.Timestamp.BelongsToLastMinutes(last)).Select(x => x.Value).DefaultIfEmpty(0).Average();

			Assert.Equal(expectedResult, aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(10)]
		public void Test_Aggregate_Min_Value_Last_X_Minutes(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Min, CurrencyPair.USDBTC, x => x.Timestamp.BelongsToLastMinutes(last));

			decimal expectedResult = testData.Where(x => x.Timestamp.BelongsToLastMinutes(last)).Select(x => x.Value).DefaultIfEmpty(0).Min();

			Assert.Equal(expectedResult, aggregateValue);
		}

		[Theory]
		[InlineData(1)]
		[InlineData(2)]
		[InlineData(10)]
		public void Test_Aggregate_Max_Value_Last_X_Minutes(int last)
		{
			this.InitializeRepositoryWithTestData();
			Queue<IBitcoinPrice> testData = this.GetTestBitcoinPrices();

			decimal aggregateValue = this._repositoryService.GetAggregatedValue(AggregateType.Max, CurrencyPair.USDBTC, x => x.Timestamp.BelongsToLastMinutes(last));

			decimal expectedResult = testData.Where(x => x.Timestamp.BelongsToLastMinutes(last)).Select(x => x.Value).DefaultIfEmpty(0).Max();

			Assert.Equal(expectedResult, aggregateValue);
		}

		#region Helpers

		private void InitializeRepositoryWithTestData()
		{
			Queue<IBitcoinPrice> priceQueue = this.GetTestBitcoinPrices();

			while (priceQueue.Count > 0)
			{
				this._repositoryService.Persist(priceQueue.Dequeue());
			}
		}

		private Queue<IBitcoinPrice> GetTestBitcoinPrices()
		{
			Queue<IBitcoinPrice> priceList = new Queue<IBitcoinPrice>();

			priceList.Enqueue(new GdaxBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 1, Timestamp = DateTime.UtcNow.GetUnixTimestampInSeconds() });
			priceList.Enqueue(new GdaxBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 2, Timestamp = DateTime.UtcNow.GetUnixTimestampInSeconds() });
			priceList.Enqueue(new GdaxBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 3, Timestamp = DateTime.UtcNow.AddMinutes(-3).GetUnixTimestampInSeconds() });
			priceList.Enqueue(new BitstampBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 4, Timestamp = DateTime.UtcNow.AddMinutes(-4).GetUnixTimestampInSeconds() });
			priceList.Enqueue(new BitstampBitcoinPrice() { Currency = CurrencyPair.USDBTC, Value = 5, Timestamp = DateTime.UtcNow.AddMinutes(-5).GetUnixTimestampInSeconds() });

			return priceList;
		}

		#endregion
	}
}
