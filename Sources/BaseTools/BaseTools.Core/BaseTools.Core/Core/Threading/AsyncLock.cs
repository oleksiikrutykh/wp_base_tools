namespace BaseTools.Core.Threading
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides a mechanism that synchronizes access to objects. Can be used in async methods.
    /// </summary>
    public class AsyncLock
    {
        private readonly AsyncSemaphore m_semaphore;

        public AsyncLock()
        {
            m_semaphore = new AsyncSemaphore(1);
        }

        public Task<AsyncLockReleaser> LockAsync()
        {
            var wait = m_semaphore.WaitAsync();
            var releaserTask = wait.ContinueWith((_, state) => new AsyncLockReleaser(((AsyncLock)state).m_semaphore),
                    this, CancellationToken.None,
                    TaskContinuationOptions.ExecuteSynchronously, TaskScheduler.Default);
            return releaserTask;
        }
    }
}
