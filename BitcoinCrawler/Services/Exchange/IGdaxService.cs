﻿using BitcoinCrawler.Model;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BitcoinCrawler.Exchange
{
    public interface IGdaxService
	{
		Task<BitcoinPrice> GetBitcoinPriceAsync();
	}
}