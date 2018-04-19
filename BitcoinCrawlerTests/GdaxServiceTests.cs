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
	public class GdaxServiceTests
	{
		private readonly GdaxService _gdaxService;

		public GdaxServiceTests()
		{
			var mockSerializer = new Mock<GdaxSerializer>();

			var mockOptions = Mock.Of<IOptions<GdaxServiceOptions>>(m =>
					m.Value.BaseUrl == "https://api.gdax.com/");

			var mockILogger = new Mock<ILogger<GdaxService>>();
			this._gdaxService = new GdaxService(mockSerializer.Object, mockOptions, mockILogger.Object);
		}

		private readonly HarvestTask _correctTask = new HarvestTask() { Origin = Origin.GDAX, Pair = It.IsAny<CurrencyPair>() };
		private readonly HarvestTask _errorTask = new HarvestTask() { Origin = Origin.Bitstamp, Pair = It.IsAny<CurrencyPair>() };

		[Fact]
		public async Task Correct_Task_Returns_Gdax_Bitcoin_Price()
		{
			var result = await this._gdaxService.GetPriceAsync(this._correctTask);

			Assert.IsType<GdaxBitcoinPrice>(result);
			Assert.Equal(Origin.GDAX, result.Origin);
			Assert.Equal(CurrencyPair.USDBTC, result.Currency);
		}

		[Fact]
		public async Task Error_Task_Returns_Null()
		{
			var result = await this._gdaxService.GetPriceAsync(this._errorTask);

			Assert.Null(result);
		}
	}
}
