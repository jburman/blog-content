using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NetFrameworkWorker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private readonly int _timeout;

        public Worker(ILogger<Worker> logger, IConfiguration config)
        {
            _logger = logger;
            _timeout = config.GetValue("WorkerTimeout", 5000);
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while(!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation("Executing Worker at {time}", DateTimeOffset.Now);

                await Task.Delay(_timeout, stoppingToken);
            }
        }
    }
}
