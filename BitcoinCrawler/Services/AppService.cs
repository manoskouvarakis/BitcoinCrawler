﻿using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Exchange;
using BitcoinCrawler.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

		public async Task RunAsync(CancellationToken token = default(CancellationToken))
		{
			while (!token.IsCancellationRequested)
			{
				List<Task<BitcoinPrice>> fetchPriceTasks = new List<Task<BitcoinPrice>>
				{
					this._bitstampService.GetBitcoinPriceAsync(),
					this._gdaxService.GetBitcoinPriceAsync()
				};

				while (fetchPriceTasks.Count > 0)
				{
					//Identify the first task that completes.  
					Task<BitcoinPrice> firstFinishedTask = await Task.WhenAny(fetchPriceTasks);

					fetchPriceTasks.Remove(firstFinishedTask);

					// Await the completed task.
					BitcoinPrice newPrice = await firstFinishedTask;
					this._repositoryService.Persist(newPrice);
				}

				this._visualizationService.Visualize();

				try
				{
					await Task.Delay(TimeSpan.FromSeconds(10), token);
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}
		}
	}
}
