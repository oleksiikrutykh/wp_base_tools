namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    internal class EventWaiterBase
    {
        private TaskCompletionSource<bool> taskSource = new TaskCompletionSource<bool>();

        protected object eventSender { get; private set; }

        protected string attachedEventName { get; private set; }

        public Task WaitOnEvent(object sender, string eventName)
        {
            this.eventSender = sender;
            this.attachedEventName = eventName;
            this.AttachToEvent();
            return this.taskSource.Task;
        }

        protected virtual void AttachToEvent()
        {
        }

        protected void Cleanup()
        {
            this.eventSender = null;
            this.taskSource.SetResult(true);
        }
    }
}
