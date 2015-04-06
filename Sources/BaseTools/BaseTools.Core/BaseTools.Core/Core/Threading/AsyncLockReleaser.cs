namespace BaseTools.Core.Threading
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Allows to unlock <see cref="AsyncLock"/>
    /// </summary>
    public sealed class AsyncLockReleaser : IDisposable
    {
        private AsyncSemaphore semaphoreForReleasing;

        private readonly object disposeLocker = new object();


        internal AsyncLockReleaser(AsyncSemaphore semaphore)
        {
            this.semaphoreForReleasing = semaphore;
        }

        public void Dispose()
        {
            AsyncSemaphore releasedSemaphore = null;
            if (this.semaphoreForReleasing != null)
            {
                lock (disposeLocker)
                {
                    if (this.semaphoreForReleasing != null)
                    {
                        releasedSemaphore = this.semaphoreForReleasing;
                        this.semaphoreForReleasing = null;
                    }
                }
            }

            if (releasedSemaphore != null)
            {
                releasedSemaphore.Release();
            }
        }
    }
}
