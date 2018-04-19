using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Model;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinCrawler.Harvesting
{
    public class HarvestingWorkerService
    {
		private readonly HarvestingServiceFactory _harvestingServiceFactory;
		private readonly IRepositoryService _repositoryService;
		private readonly TaskQueue _taskQueue;

		public HarvestingWorkerService(
			HarvestingServiceFactory harvestingServiceFactory,
			IRepositoryService repositoryService,
			TaskQueue taskQueue)
		{
			this._harvestingServiceFactory = harvestingServiceFactory;
			this._repositoryService = repositoryService;
			this._taskQueue = taskQueue;
		}

		public async Task Execute(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				HarvestTask task = this._taskQueue.Take(token);

				IHarvestingService harvester = this._harvestingServiceFactory.GetHarvestingService(task.Origin);

				IBitcoinPrice price = await harvester.GetPriceAsync(task);
				this._repositoryService.Persist(price);
			}
		}
    }
}
