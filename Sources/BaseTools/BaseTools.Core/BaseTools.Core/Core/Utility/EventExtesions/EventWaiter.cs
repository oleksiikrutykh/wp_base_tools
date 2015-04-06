namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    internal class EventWaiter : EventWaiterBase
    {
        protected override void AttachToEvent()
        {
            base.AttachToEvent();
            EventExtensions.AttachToEvent(this.eventSender, this.attachedEventName, new EventHandler(this.Compelted));
        }

        private void Compelted(object sender, EventArgs args)
        {
            EventExtensions.DetachFromEvent(this.eventSender, this.attachedEventName, new EventHandler(this.Compelted));
            this.Cleanup();
        }
    }
}
