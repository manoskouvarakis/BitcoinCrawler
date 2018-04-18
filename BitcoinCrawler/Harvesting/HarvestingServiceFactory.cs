using BitcoinCrawler.Model;
using System;

namespace BitcoinCrawler.Harvesting
{
    public class HarvestingServiceFactory
    {
		private readonly BitstampService _bitstampService;
		private readonly GdaxService _gdaxService;

		public HarvestingServiceFactory(BitstampService bitstampService, GdaxService gdaxService)
		{
			this._bitstampService = bitstampService;
			this._gdaxService = gdaxService;
		}

		public IHarvestingService GetHarvestingService(Origin serviceOrigin)
		{
			switch (serviceOrigin)
			{
				case Origin.Bitstamp: return this._bitstampService;
				case Origin.GDAX: return this._gdaxService;
				default: throw new Exception("Unrecognised harvesting service");
			}
		}
	}
}
