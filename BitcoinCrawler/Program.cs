using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Options;
using BitcoinCrawler.Serializers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.IO;

namespace BitcoinCrawler
{
    class Program
    {
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

			//create a service provider from the service collection
			var serviceProvider = serviceCollection.BuildServiceProvider();

			//resolve the dependency graph
			var appService = serviceProvider.GetService<AppService>();

			//run the application
			appService.Run();
		}
	}
}
