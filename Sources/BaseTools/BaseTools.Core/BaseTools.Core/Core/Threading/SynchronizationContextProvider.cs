namespace BaseTools.Core.Threading
{
    using BaseTools.Core.Info;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides services for managing the queue of work items for a thread.
    /// </summary>
    public class SynchronizationContextProvider
    {
        private static TaskCompletionSource<bool> initializationTask = new TaskCompletionSource<bool>();

        private static bool isInitialized;

        private static SynchronizationContext syncContext;

        /// <summary>
        /// Initializes the instance. Must be called only in UI Thread
        /// </summary>
        public static void Initialize()
        {
            if (!isInitialized)
            {
                syncContext = SynchronizationContext.Current;
                if (syncContext != null)
                {
                    initializationTask.SetResult(true);
                    isInitialized = true;
                }
            }
        }

        public static Task PostAsync(Action action)
        {
            return PerformPostAsync(action);
        }

        public static Task PostAsync(Func<Task> asyncFunction)
        {
            return PerformPostAsync(asyncFunction);
        }

        private static async Task PerformPostAsync(object action)
        {
            var environmentInfo = EnvironmentInfo.Current;
            var taskCompletitionSource = new TaskCompletionSource<bool>();
            if (environmentInfo.OperatingSystemType == OperatingSystemType.WindowsPhoneSilverlight)
            {
                // If phone - sync context accesssinble from any part of application.
                await initializationTask.Task;
                syncContext.Post((state) =>
                {
                    InvokeAction(action, taskCompletitionSource);
                },
                null);
            }
            else
            {
                // If winrt - sync context not work in background agent.
                if (environmentInfo.EnvironmentType == EnvironmentType.Application)
                {
                    await initializationTask.Task;
                    syncContext.Post((state) =>
                    {
                        InvokeAction(action, taskCompletitionSource);
                    },
                    null);
                }
                else
                {
                    InvokeAction(action, taskCompletitionSource);
                }
            }

            await taskCompletitionSource.Task;
        }


        private static async void InvokeAction(object executedDelegate, TaskCompletionSource<bool> taskCompletitionSource)
        {
            try
            {
                var asyncFunction = executedDelegate as Func<Task>;
                if (asyncFunction != null)
                {
                    await asyncFunction.Invoke();
                }
                else
                {
                    var action = executedDelegate as Action;
                    if (action != null)
                    {
                        action.Invoke();
                    }
                }

                taskCompletitionSource.TrySetResult(true);
            }
            catch (Exception ex)
            {
                taskCompletitionSource.TrySetException(ex);
            }
        }

    }
}
