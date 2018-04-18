using BitcoinCrawler.Options;
using Microsoft.Extensions.Options;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinCrawler.Harvesting
{
    public class HarvestingOrchestratorService
    {
		private readonly TaskQueue _taskQueue;
		private readonly HarvestingOrchestratorServiceOptions _harvestingOrchestratorServiceOptions;

		public HarvestingOrchestratorService(TaskQueue taskQueue, IOptions<HarvestingOrchestratorServiceOptions> harvestingOrchestratorServiceOptions)
		{
			this._taskQueue = taskQueue;
			this._harvestingOrchestratorServiceOptions = harvestingOrchestratorServiceOptions.Value;
		}

		public async Task Orchestrate(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				//If an exchange client is slow, orchestrator may add the same task more than once.
				//This is a known bug named feature for the purpose of this assignment.
				//It can be solved using a concurrent queue or dictionary, checking for existence before adding new tasks.
				foreach (HarvestTask harvestTask in this._harvestingOrchestratorServiceOptions.Tasks)
					this._taskQueue.Add(harvestTask);

				try
				{
					await Task.Delay(TimeSpan.FromSeconds(this._harvestingOrchestratorServiceOptions.Interval));
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}

		}
	}
}
