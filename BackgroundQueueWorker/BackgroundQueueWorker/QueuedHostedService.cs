using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundQueueWorker
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger<QueuedHostedService> _logger;
        private readonly IServiceProvider _services;

        public QueuedHostedService(IBackgroundTaskQueue taskQueue, 
            ILogger<QueuedHostedService> logger,
            IServiceProvider services)
        {
            TaskQueue = taskQueue;
            _logger = logger;
            _services = services;
        }

        public IBackgroundTaskQueue TaskQueue { get; }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) =>
            await BackgroundProcessing(stoppingToken);

        private async Task BackgroundProcessing(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                var workItem = await TaskQueue.DequeueAsync(stoppingToken);

                try
                {
                    if (workItem.RequiresScope is true)
                        using (var scope = _services.CreateScope())
                            await workItem.Execute(stoppingToken, scope);
                    else
                        await workItem.Execute(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing {WorkItem}.", nameof(workItem));
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Queued Hosted Service is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}
