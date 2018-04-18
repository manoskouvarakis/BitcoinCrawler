using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Model;
using BitcoinCrawler.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using static BitcoinCrawler.DataAccess.RepositoryService;

namespace BitcoinCrawler.Reporting
{
	public class PrintoutService
	{
		private readonly IRepositoryService _repositoryService;
		private readonly PrintoutServiceOptions _printoutServiceOptions;
		private readonly ILogger _logger;

		public PrintoutService(IRepositoryService repositoryService, IOptions<PrintoutServiceOptions> printoutServiceOptions, ILogger<PrintoutService> logger)
		{
			this._repositoryService = repositoryService;
			this._printoutServiceOptions = printoutServiceOptions.Value;
			this._logger = logger;
		}

		public async Task Execute(CancellationToken token)
		{
			while (!token.IsCancellationRequested)
			{
				if (!this._repositoryService.IsEmpty())
				{
					this._logger.LogInformation("Printing Statistics");
					this.PrintStatistics(this._repositoryService.GetCurrencyPairs().ToList());
				}
				else
				{
					this._logger.LogInformation("Memory is empty");
				}

				try
				{
					await Task.Delay(TimeSpan.FromSeconds(_printoutServiceOptions.Interval));
				}
				catch (TaskCanceledException)
				{
					break;
				}
			}
		}

		public void PrintStatistics(List<CurrencyPair> currencyPairs)
		{
			Console.Clear();
			Console.SetCursorPosition(0, 0);

			foreach (CurrencyPair currencyPair in currencyPairs)
			{
				this.PrintStatistics(currencyPair);
				Console.WriteLine();
			}
		}

		public void PrintStatistics(CurrencyPair pair)
		{
			IBitcoinPrice allHistoryMax = this._repositoryService.Max(pair);
			IBitcoinPrice allHistoryMin = this._repositoryService.Min(pair);


			Console.WriteLine(pair.ToString());
			Console.WriteLine("-------");
			
			decimal price = this._repositoryService.GetLastValue(pair, out TrendType trend);
			Console.ForegroundColor = trend == TrendType.Up ? ConsoleColor.Green : ConsoleColor.Red;
			Console.WriteLine("LAST: {0:N2}", price);
			Console.ResetColor();
			Console.WriteLine();

			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average, pair));
			Console.WriteLine("MAX: {0:N2} at {1} (UTC)", allHistoryMax.Value, allHistoryMax.Timestamp.DateTimeFromUnixTimestampSeconds());
			Console.WriteLine("MIX: {0:N2} at {1} (UTC)", allHistoryMin.Value, allHistoryMin.Timestamp.DateTimeFromUnixTimestampSeconds());
			Console.WriteLine();

			Console.WriteLine("Last 5 prices");
			Console.WriteLine("-------------");
			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average, pair, 5));
			Console.WriteLine("MAX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Max, pair, 5));
			Console.WriteLine("MIX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Min, pair, 5));
			Console.WriteLine();

			Console.WriteLine("Last 2 minutes");
			Console.WriteLine("--------------");
			Console.WriteLine("AVERAGE: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Average, pair, x => x.Timestamp.BelongsToLastMinutes(2)));
			Console.WriteLine("MAX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Max, pair, x => x.Timestamp.BelongsToLastMinutes(2)));
			Console.WriteLine("MIX: {0:N2}", this._repositoryService.GetAggregatedValue(RepositoryService.AggregateType.Min, pair, x => x.Timestamp.BelongsToLastMinutes(2)));
		}
	}
}
