using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Options;
using BitcoinCrawler.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinCrawler
{
    class Program
    {
		private static readonly CancellationTokenSource cts = new CancellationTokenSource();
		 
		static void Main(string[] args)
        {
			//configure services
			IServiceCollection serviceCollection = new ServiceCollection()
				.AddSingleton<Harvesting.BitstampService>()
				.AddSingleton<Harvesting.GdaxService>()
				.AddSingleton<GdaxSerializer>()
				.AddSingleton<BitstampSerializer>()
				.AddSingleton<Harvesting.HarvestingOrchestratorService>()
				.AddTransient<Harvesting.HarvestingWorkerService>()
				.AddSingleton<Harvesting.TaskQueue>()
				.AddSingleton<Reporting.PrintoutService>()
				.AddSingleton<IRepositoryService, RepositoryService>()
				.AddTransient<AppService>()
				.AddSingleton<Harvesting.HarvestingServiceFactory>()
				;

			var configuration = new ConfigurationBuilder()
				.SetBasePath(Directory.GetCurrentDirectory())
				.AddJsonFile("appsettings.json", false)
				.Build();

			serviceCollection.AddOptions();
			serviceCollection.Configure<PrintoutServiceOptions>(configuration.GetSection("printout_service"));
			serviceCollection.Configure<HarvestingOrchestratorServiceOptions>(configuration.GetSection("harvesting_orchestrator_service"));
			serviceCollection.Configure<AppServiceOptions>(configuration.GetSection("app_service"));
			serviceCollection.Configure<RepositoryServiceOptions>(configuration.GetSection("repository_service"));
			serviceCollection.Configure<BitstampServiceOptions>(configuration.GetSection("bitstamp_service"));
			serviceCollection.Configure<GdaxServiceOptions>(configuration.GetSection("gdax_service"));

			serviceCollection.AddLogging(configure => configure.AddDebug());

			Console.CancelKeyPress += new ConsoleCancelEventHandler(Console_CancelKeyPress);

			//create a service provider from the service collection
			var serviceProvider = serviceCollection.BuildServiceProvider();

			//resolve the dependency graph
			var appService = serviceProvider.GetService<AppService>();

			//run the application
			Task[] tasks = appService.Run(cts.Token);

			//wait for signal
			cts.Token.WaitHandle.WaitOne();

			//wait all tasks to finish
			Task.WaitAll(tasks);

			cts.Dispose();
		}

		static void Console_CancelKeyPress(object sender, ConsoleCancelEventArgs e)
		{
			Console.WriteLine("Exiting");
			if (e.SpecialKey == ConsoleSpecialKey.ControlC)
			{
				cts.Cancel();
				e.Cancel = true;
			}
		}
	}
}
