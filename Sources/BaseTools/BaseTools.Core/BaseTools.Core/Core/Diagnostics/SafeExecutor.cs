namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Allows to handle errors which can be occured during execution. 
    /// </summary>
    public static class SafeExecutor
    {
        private static Logger logger = Factory.Common.GetInstance<Logger>();

        public static void ExecuteSafe(Action action)
        {
            try
            {
                action.Invoke();
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else

                WriteErrorIntoLogs(ex);
#endif
            }
        }

        public static TResult ExecuteSafe<TResult>(Func<TResult> function)
        {
            return ExecuteSafe(function, default(TResult));
        }

        public static TResult ExecuteSafe<TResult>(Func<TResult> function, TResult errorResult)
        {
            return ExecuteSafe(function, () => errorResult);
        }

        public static TResult ExecuteSafe<TResult>(Func<TResult> function, Func<TResult> errorResultGenerator)
        {
            TResult result = default(TResult);
            try
            {
                result = function.Invoke();
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else

                WriteErrorIntoLogs(ex);
                result = errorResultGenerator.Invoke();
#endif
            }

            return result;
        }

        public static async Task ExecuteSafeAsync(Func<Task> asyncAction)
        {
            try
            {
                await asyncAction.Invoke();
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else

                WriteErrorIntoLogs(ex);
#endif
            }
        }

        public static Task<TResult> ExecuteSafeAsync<TResult>(Func<Task<TResult>> function)
        {
            return ExecuteSafeAsync(function, default(TResult));
        }

        public static Task<TResult> ExecuteSafeAsync<TResult>(Func<Task<TResult>> function, TResult errorResult)
        {
            return ExecuteSafeAsync(function, () => errorResult);
        }

        public static async Task<TResult> ExecuteSafeAsync<TResult>(Func<Task<TResult>> function, Func<TResult> errorResultGenerator)
        {
            TResult result = default(TResult);
            try
            {
                result = await function.Invoke();
            }
            catch (Exception ex)
            {
#if DEBUG
                throw;
#else
                WriteErrorIntoLogs(ex);
                result = errorResultGenerator.Invoke();
#endif
            }

            return result;
        }

        private static void WriteErrorIntoLogs(Exception ex)
        {
            var errorText = ex.ToString();
            logger.Write(errorText);
        }
    }
}
