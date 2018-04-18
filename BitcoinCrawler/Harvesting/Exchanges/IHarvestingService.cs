using BitcoinCrawler.Model;
using System.Threading.Tasks;

namespace BitcoinCrawler.Harvesting
{
    public interface IHarvestingService
    {
		Task<IBitcoinPrice> GetPriceAsync(HarvestTask task);
	}
}
