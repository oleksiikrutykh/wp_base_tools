namespace BaseTools.Core.Utility
{
    using BaseTools.Core.Threading;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Allows to invoke events.
    /// </summary>
    public static class EventHelper
    {
        public static void CallEvent(this EventHandler eventHandler, object sender, EventArgs args)
        {
            if (eventHandler != null)
            {
                eventHandler.Invoke(sender, args);
            }
        }

        public static void CallEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs args)
        {
            if (eventHandler != null)
            {
                eventHandler.Invoke(sender, args);
            }
        }

        public static Task SafeCallEvent(this EventHandler eventHandler, object sender, EventArgs args, bool onUIThread)
        {
            // Call event that can be handled by external code. All exceptions, that was thrown in external code, don't fail invoking code.
            Task eventInvokeTask = null;
            if (onUIThread)
            {
                eventInvokeTask = SynchronizationContextProvider.PostAsync(() =>
                {
                    CallEvent(eventHandler, sender, args);
                });
            }
            else
            {
                eventInvokeTask = Task.Run(() =>
                {
                    CallEvent(eventHandler, sender, args);
                });
            }

            eventInvokeTask.ContinueWith(TryThrowTask);
            return eventInvokeTask.ContinueWith((t) => { });
        }

        public static Task SafeCallEvent<TEventArgs>(this EventHandler<TEventArgs> eventHandler, object sender, TEventArgs args, bool onUIThread)
        {
            Task eventInvokeTask = null;
            if (onUIThread)
            {
                eventInvokeTask = SynchronizationContextProvider.PostAsync(() =>
                {
                    CallEvent(eventHandler, sender, args);
                });
            }
            else
            {
                eventInvokeTask = Task.Run(() =>
                {
                    CallEvent(eventHandler, sender, args);
                });
            }

            eventInvokeTask.ContinueWith(TryThrowTask);
            return eventInvokeTask.ContinueWith((t) => { });
        }

        private async static void TryThrowTask(Task task)
        {
            await task;    
        }
    }
}
