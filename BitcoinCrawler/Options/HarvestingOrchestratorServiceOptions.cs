using BitcoinCrawler.Harvesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Options
{
	public class HarvestingOrchestratorServiceOptions
	{
		public int Interval { get; set; } = 5;

		public List<HarvestTask> Tasks { get; set; }
	}
}
