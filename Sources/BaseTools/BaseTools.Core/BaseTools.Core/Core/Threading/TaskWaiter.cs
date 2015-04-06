namespace BaseTools.Core.Threading
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Allows to perform asynchronous operations synchronously.
    /// </summary>
    public static class TaskWaiter
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void WaitSynchronously(this Task task)
        {
            Guard.CheckIsNotNull(task, "task");
            try
            {
                task.Wait();
            }
            catch (AggregateException ex)
            {
                HandleExceptions(ex);
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static TResult WaitSynchronously<TResult>(this Task<TResult> task)
        {
            Guard.CheckIsNotNull(task, "task");
            TResult result = default(TResult);
            try
            {
               
                result = task.Result;
            }
            catch (AggregateException ex)
            {
                HandleExceptions(ex);
            }

            return result;
        }

        private static void HandleExceptions(AggregateException ex)
        {
            foreach (var innerExeption in ex.InnerExceptions)
            {
                throw innerExeption;
            }
        }
    }
}

