using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Model;

namespace BitcoinCrawler.Services
{
	public class PrintToConsoleService : IVisualizationService
	{
		private readonly UnixTimeService _unixTimeService;

		private readonly IRepositoryService _repositoryService;

		public PrintToConsoleService(IRepositoryService repositoryService, UnixTimeService unixTimeService)
		{
			_repositoryService = repositoryService;
			_unixTimeService = unixTimeService;
		}

		public void Visualize()
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);

			decimal allHistoryMax = this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Max);
			decimal allHistoryMin = this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Min);

			Console.WriteLine("BTC/USD");
			Console.WriteLine("-------");
			Console.WriteLine("LAST: {0:N2}", this._repositoryService.GetLastValue());
			Console.WriteLine();

			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average));
			Console.WriteLine("MAX: {0:N2} at {1}", allHistoryMax, this._unixTimeService.DateTimeFromUnixTimestampSeconds(this._repositoryService.Retrieve(allHistoryMax)?.Timestamp ?? 0));
			Console.WriteLine("MIX: {0:N2} at {1}", allHistoryMin, this._unixTimeService.DateTimeFromUnixTimestampSeconds(this._repositoryService.Retrieve(allHistoryMin)?.Timestamp ?? 0));
			Console.WriteLine();

			Console.WriteLine("Last 5 prices");
			Console.WriteLine("-------------");
			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average, 5));
			Console.WriteLine("MAX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Max, 5));
			Console.WriteLine("MIX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Min, 5));
			Console.WriteLine();

			Console.WriteLine("Last 2 minutes");
			Console.WriteLine("--------------");
			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average, x => this._unixTimeService.BelongsToLastTwoMinutes(x.Timestamp)));
			Console.WriteLine("MAX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Max, x => this._unixTimeService.BelongsToLastTwoMinutes(x.Timestamp)));
			Console.WriteLine("MIX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Min, x => this._unixTimeService.BelongsToLastTwoMinutes(x.Timestamp)));
		}
	}
}
