using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BitcoinCrawler
{
	public static class Extensions
	{
		// Ex: collection.TakeLast(5);
		public static IEnumerable<T> TakeLast<T>(this IEnumerable<T> source, int N)
		{
			return source.Skip(Math.Max(0, source.Count() - N));
		}

		private static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		public static bool BelongsToLastMinutes(this long value, int minutes)
		{
			return value > Extensions.GetUnixTimestampInSeconds(DateTime.UtcNow.AddMinutes(-minutes));
		}

		public static long GetUnixTimestampInSeconds(this DateTime value)
		{
			return (long)(value - Extensions.UnixEpoch).TotalSeconds;
		}

		public static DateTime DateTimeFromUnixTimestampSeconds(this long seconds)
		{
			return Extensions.UnixEpoch.AddSeconds(seconds);
		}
	}
}
