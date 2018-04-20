using BitcoinCrawler.Options;
using BitcoinCrawler.Reporting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinCrawler
{
	public class AppService
	{
		private readonly IServiceProvider _serviceProvider;
		private readonly Harvesting.HarvestingOrchestratorService _harvestingOrchestratorService;
		private readonly PrintoutService _printoutService;
		private readonly AppServiceOptions _appServiceOptions;

		public AppService(IServiceProvider serviceProvider, 
			Harvesting.HarvestingOrchestratorService harvestingOrchestratorService, 
			PrintoutService printoutService,
			IOptions<AppServiceOptions> appServiceOptions)
		{
			this._serviceProvider = serviceProvider;
			this._harvestingOrchestratorService = harvestingOrchestratorService;
			this._printoutService = printoutService;
			this._appServiceOptions = appServiceOptions.Value;
		}

		public Task[] Run(CancellationToken token)
		{
			List<Task> workerTasks = new List<Task>();
			Task orchestratorTask = this._harvestingOrchestratorService.Orchestrate(token);
			Task printoutTask = this._printoutService.Execute(token);

			for (int i = 0; i < this._appServiceOptions.Threads; i += 1)
			{
				Harvesting.HarvestingWorkerService worker = this._serviceProvider.GetRequiredService<Harvesting.HarvestingWorkerService>();
				Task workerTask = worker.Execute(token);
				workerTasks.Add(workerTask);
			}

			return workerTasks.Union(new Task[] { orchestratorTask, printoutTask }).ToArray();
		}
	}
}
