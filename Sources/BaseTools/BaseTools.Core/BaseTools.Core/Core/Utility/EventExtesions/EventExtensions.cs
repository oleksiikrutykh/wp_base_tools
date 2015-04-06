
namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.InteropServices.WindowsRuntime;
    using System.Text;
    using System.Reflection;
    using System.Threading.Tasks;

    public static class EventExtensions
    {
        public static Task WaitOnEvent<TEventArgs>(this object eventSource, string eventName)
        {
            var waiter = new EventWaiter<TEventArgs>();
            var waitTask = waiter.WaitOnEvent(eventSource, eventName);
            return waitTask;
        }

        public static Task WaitOnEvent(this object eventSource, string eventName)
        {
            var waiter = new EventWaiter();
            var waitTask = waiter.WaitOnEvent(eventSource, eventName);
            return waitTask;
        }

        public static void AttachToEvent(object eventSender, string eventName, Delegate handler)
        {
            var methodInfo = handler.GetMethodInfo();
            var eventInfo = FindEventInfo(eventSender, eventName);
            var castedHandler = handler;
            if (castedHandler.GetType() != eventInfo.EventHandlerType)
            {
                castedHandler = methodInfo.CreateDelegate(eventInfo.EventHandlerType, handler.Target);
            }

            try
            {
                eventInfo.AddEventHandler(eventSender, castedHandler);
            }
            catch (InvalidOperationException)
            {
                AddWindowsRuntimeHandler(eventSender, eventName, castedHandler);
            }
        }

        public static void DetachFromEvent(object eventSender, string eventName, Delegate handler)
        {
            var eventInfo = FindEventInfo(eventSender, eventName);
            try
            {
                eventInfo.RemoveEventHandler(eventSender, handler);
            }
            catch (InvalidOperationException)
            {
                RemoveWindowsRuntimeHandler(eventSender, eventName, handler);
            }
        }

        private static EventInfo FindEventInfo(object eventSender, string eventName)
        {
            EventInfo eventInfo = null;
            eventInfo = eventSender.GetType().GetRuntimeEvent(eventName);

            return eventInfo;
        }


        private static void AddWindowsRuntimeHandler(object eventSender, string eventName, Delegate handler)
        {
            var bindActions = CreateBindActions(eventSender, eventName, handler);
            WindowsRuntimeMarshal.AddEventHandler(bindActions.ActionAdd, bindActions.ActionRemove, bindActions.ActionInvoke);
        }

        private static void RemoveWindowsRuntimeHandler(object eventSender, string eventName, Delegate handler)
        {
            var bindActions = CreateBindActions(eventSender, eventName, handler);
            WindowsRuntimeMarshal.RemoveEventHandler(bindActions.ActionRemove, bindActions.ActionInvoke);
        }

        private static WindowsRuntimeEventBindActions CreateBindActions(object eventSender, string eventName, Delegate handler)
        {
            var actionsContainer = new WindowsRuntimeEventBindActions();
            var eventInfo = eventSender.GetType().GetRuntimeEvent(eventName);
            var handlerLink = handler;
            MethodInfo addMethod = eventInfo.AddMethod;
            MethodInfo removeMethod = eventInfo.RemoveMethod;
            var addParameters = addMethod.GetParameters();
            actionsContainer.ActionAdd = (a) =>
            {
                var parametersArray = new object[] { handlerLink };
                var result = (EventRegistrationToken)addMethod.Invoke(eventSender, parametersArray);
                return result;
            };

            actionsContainer.ActionRemove = t => removeMethod.Invoke(eventSender, new object[] { handlerLink });
            actionsContainer.ActionInvoke = (sender, e) =>
            {
                handlerLink.DynamicInvoke(sender, e);
            };

            return actionsContainer;
        }

        private class WindowsRuntimeEventBindActions
        {
            public Func<object, EventRegistrationToken> ActionAdd { get; set; }

            public Action<EventRegistrationToken> ActionRemove { get; set; }

            public Action<object, object> ActionInvoke { get; set; }
        }
    }
}
