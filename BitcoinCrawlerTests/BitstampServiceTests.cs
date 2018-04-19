using AutoFixture;
using BitcoinCrawler.Harvesting;
using BitcoinCrawler.Model;
using BitcoinCrawler.Options;
using BitcoinCrawler.Serializers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using Newtonsoft.Json;
using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace BitcoinCrawlerTests
{
	public class BitstampServiceTests
	{
		private readonly BitstampService _bitstampService;

		public BitstampServiceTests()
		{
			var mockSerializer = new Mock<BitstampSerializer>();

			var mockOptions = Mock.Of<IOptions<BitstampServiceOptions>>(m =>
					m.Value.BaseUrl == "https://www.bitstamp.net/api/v2/ticker/");

			var mockILogger = new Mock<ILogger<BitstampService>>();

			this._bitstampService = new BitstampService(mockSerializer.Object, mockOptions, mockILogger.Object);
		}

		private readonly HarvestTask _correctTask = new HarvestTask() { Origin = Origin.Bitstamp, Pair = It.IsAny<CurrencyPair>() };
		private readonly HarvestTask _errorTask = new HarvestTask() { Origin = Origin.GDAX, Pair = It.IsAny<CurrencyPair>() };

		[Fact]
		public async Task Correct_Task_Returns_Bitstamp_Bitcoin_Price()
		{
			var result = await this._bitstampService.GetPriceAsync(this._correctTask);

			Assert.IsType<BitstampBitcoinPrice>(result);
			Assert.Equal(Origin.Bitstamp, result.Origin);
			Assert.Equal(CurrencyPair.USDBTC, result.Currency);
		}

		[Fact]
		public async Task Error_Task_Returns_Null()
		{
			var result = await this._bitstampService.GetPriceAsync(this._errorTask);

			Assert.Null(result);
		}
	}
}
