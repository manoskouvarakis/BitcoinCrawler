using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BitcoinCrawler.Services
{
	public interface IAppService
    {
		Task RunAsync(CancellationToken token = default(CancellationToken));
	}
}
