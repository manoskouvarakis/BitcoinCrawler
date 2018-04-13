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

		public class FixedSizedQueue<T> : ConcurrentQueue<T>
		{
			private readonly object syncObject = new object();

			public int Size { get; private set; }

			public FixedSizedQueue(int size)
			{
				Size = size;
			}

			public new void Enqueue(T obj)
			{
				base.Enqueue(obj);
				lock (syncObject)
				{
					while (base.Count > Size)
					{
						T outObj;
						base.TryDequeue(out outObj);
					}
				}
			}
		}
	}
}
