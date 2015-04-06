namespace BaseTools.Core.Threading
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;

    /// <summary>
    /// Allows to perform asynchronous operations synchronously.
    /// </summary>
    public static class AsyncWaiter
    {
        public static void WaitSynchronously(this IAsyncAction action)
        {
            var task = action.AsTask();
            TaskWaiter.WaitSynchronously(task);
        }

        public static TResult WaitSynchronously<TResult>(this IAsyncOperation<TResult> operation)
        {
            var task = operation.AsTask();
            return TaskWaiter.WaitSynchronously(task);
        }
    }
}

