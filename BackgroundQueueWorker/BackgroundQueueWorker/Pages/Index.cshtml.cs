using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BackgroundQueueWorker.Pages
{
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly IBackgroundTaskQueue _backgroundWork;

        public IndexModel(ILogger<IndexModel> logger, IBackgroundTaskQueue backgroundWork)
        {
            _logger = logger;
            _backgroundWork = backgroundWork;
        }

        public void OnGet()
        {
            // queue a background work item that doesn't need any scope
            _backgroundWork.QueueBackgroundWorkItem(async (token) =>
            {
                _logger.LogDebug("Starting some background work...");
                await Task.Delay(2000);
                _logger.LogDebug("Finished the background work.");
            });

            // queue a background work item that requires a scope
            _backgroundWork.QueueBackgroundWorkItem(async (token, scope) =>
            {
                _logger.LogDebug("Starting some scoped background work...");
                await Task.Delay(2000);

                var testService = scope.ServiceProvider.GetRequiredService<TestService>();
                _logger.LogDebug(testService.GetId());
                _logger.LogDebug("Finished the scoped background work.");

                // try out nested background work with scope
                _backgroundWork.QueueBackgroundWorkItem(async (token, scope) =>
                {
                    _logger.LogDebug("Starting some nested scoped background work...");

                    await Task.Delay(2000);
                    
                    var testService = scope.ServiceProvider.GetRequiredService<TestService>();
                    _logger.LogDebug("nested: " + testService.GetId());
                    _logger.LogDebug("Finished the nested scoped background work.");
                });
            });
        }
    }
}
