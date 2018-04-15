using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Model
{
    public interface IBitcoinPrice
    {
		decimal GetValue();

		long GetUnixTime();
    }
}
