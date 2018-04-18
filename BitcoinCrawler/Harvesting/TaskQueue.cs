using System.Collections.Concurrent;

namespace BitcoinCrawler.Harvesting
{
    public class TaskQueue : BlockingCollection<HarvestTask>
    {
    }
}
