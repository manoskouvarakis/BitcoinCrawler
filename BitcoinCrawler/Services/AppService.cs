using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Exchange;
using BitcoinCrawler.Model;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static BitcoinCrawler.Extensions;

namespace BitcoinCrawler.Services
{
	public class AppService : IAppService
	{
		private readonly IVisualizationService _visualizationService;

		private readonly IRepositoryService _repositoryService;

		private readonly IBitstampService _bitstampService;

		private readonly IGdaxService _gdaxService;

		public AppService(IRepositoryService repositoryService, IBitstampService bitstampService, IGdaxService gdaxService, IVisualizationService visualizationService)
		{
			_repositoryService = repositoryService;
			_bitstampService = bitstampService;
			_gdaxService = gdaxService;
			_visualizationService = visualizationService;
		}

		public async Task RunAsync()
		{
			while (true)
			{
				BitcoinPrice bitstampPrice = await this._bitstampService.GetBitcoinPriceAsync();
				this._repositoryService.Persist(bitstampPrice);

				BitcoinPrice gdaxPrice = await this._gdaxService.GetBitcoinPriceAsync();
				this._repositoryService.Persist(gdaxPrice);

				this._visualizationService.Visualize();

				Thread.Sleep(10000);
			}
		}
	}
}
