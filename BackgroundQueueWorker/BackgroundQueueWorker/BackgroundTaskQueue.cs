using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

namespace BackgroundQueueWorker
{
    public delegate Task BackgroundWork(CancellationToken token);
    public delegate Task BackgroundWorkWithScope(CancellationToken token, IServiceScope scope);

    public struct BackgroundWorkExecute
    {
        public BackgroundWorkExecute(Delegate work) { _work = work; }

        private Delegate _work;

        public bool RequiresScope => _work is BackgroundWorkWithScope;

        public Task Execute(CancellationToken token, IServiceScope scope = null)
        {
            return _work switch
            {
                BackgroundWorkWithScope sw when scope is null => throw new ArgumentNullException(nameof(scope)),
                BackgroundWorkWithScope sw => sw(token, scope),
                BackgroundWork w => w(token),
                _ => throw new ArgumentNullException("Invalid work delegate type")
            };
        }
    }

    public interface IBackgroundTaskQueue
    {
        void QueueBackgroundWorkItem(BackgroundWork work);
        void QueueBackgroundWorkItem(BackgroundWorkWithScope work);

        Task<BackgroundWorkExecute> DequeueAsync(CancellationToken cancellationToken);
    }

    public class BackgroundTaskQueue : IBackgroundTaskQueue
    {
        private ConcurrentQueue<Delegate> _workItems = new ConcurrentQueue<Delegate>();
        private SemaphoreSlim _signal = new SemaphoreSlim(0);

        private void _Queue(Delegate work)
        {
            if (work == null)
                throw new ArgumentNullException(nameof(work));

            _workItems.Enqueue(work);
            _signal.Release();
        }

        public void QueueBackgroundWorkItem(BackgroundWork work) => _Queue(work);

        public void QueueBackgroundWorkItem(BackgroundWorkWithScope work) => _Queue(work);

        public async Task<BackgroundWorkExecute> DequeueAsync(CancellationToken cancellationToken)
        {
            await _signal.WaitAsync(cancellationToken);
            _workItems.TryDequeue(out var workItem);

            return new BackgroundWorkExecute(workItem);
        }
    }
}
