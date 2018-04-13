using System;
using System.Collections.Generic;
using System.Text;

namespace BitcoinCrawler.Services
{
    public class UnixTimeService
    {
		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public bool BelongsToLastTwoMinutes(long value)
		{
			return value > this.GetUnixTimestampSeconds(DateTime.UtcNow.AddMinutes(-2));
		}

		public long GetUnixTimestampSeconds(DateTime value)
		{
			return (long)(value - UnixTimeService.UnixEpoch).TotalSeconds;
		}

		public long GetCurrentUnixTimestampSeconds()
		{
			return this.GetUnixTimestampSeconds(DateTime.UtcNow);
		}

		public DateTime DateTimeFromUnixTimestampSeconds(long seconds)
		{
			return UnixTimeService.UnixEpoch.AddSeconds(seconds);
		}
	}
}
