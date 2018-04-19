using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Options
{
	public class PrintoutServiceOptions
	{
		public int Interval { get; set; } = 5;
		public int LastMinutes { get; set; } = 1;
		public int LastPrices { get; set; } = 3;
	}
}
