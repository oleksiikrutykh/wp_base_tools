namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Runtime.InteropServices.WindowsRuntime;

    internal class EventWaiter<TEventArgs> : EventWaiterBase
    {
        protected override void AttachToEvent()
        {
            base.AttachToEvent();
            EventExtensions.AttachToEvent(this.eventSender, this.attachedEventName, new EventHandler<TEventArgs>(Completed));
        }

        private Delegate CreateDelegate()
        {
            var eventInfo = this.eventSender.GetType().GetRuntimeEvent(this.attachedEventName);
            var methodInfo = this.GetType().GetTypeInfo().GetDeclaredMethod("Completed");
            var handlerDelegate = methodInfo.CreateDelegate(eventInfo.EventHandlerType, this);
            return handlerDelegate;
        }

        public void Completed(object sender, TEventArgs args)
        {
            var handlerLink = this.CreateDelegate();
            EventExtensions.DetachFromEvent(this.eventSender, this.attachedEventName, handlerLink);
            this.Cleanup();
        }
    }
}
