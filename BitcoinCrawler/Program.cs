using BitcoinCrawler.DataAccess;
using BitcoinCrawler.Exchange;
using BitcoinCrawler.Serializers;
using BitcoinCrawler.Services;
using Microsoft.Extensions.DependencyInjection;

namespace BitcoinCrawler
{
    class Program
    {
        static void Main(string[] args)
        {
			//configure services
			var services = new ServiceCollection()
				.AddTransient<IAppService, AppService>()
				.AddTransient<IVisualizationService, PrintToConsoleService>()
				.AddTransient<UnixTimeService>()
				.AddTransient<GdaxSerializer>()
				.AddTransient<BitstampSerializer>()
				.AddSingleton<IRepositoryService, RepositoryService>()
				.AddSingleton<IBitstampService, BitstampService>()
				.AddSingleton<IGdaxService, GdaxService>();

			//create a service provider from the service collection
			var serviceProvider = services.BuildServiceProvider();

			//resolve the dependency graph
			var appService = serviceProvider.GetService<IAppService>();

			//run the application
			//appService.RunAsync().Wait();
			appService.RunAsync().GetAwaiter().GetResult();
		}
    }
}
