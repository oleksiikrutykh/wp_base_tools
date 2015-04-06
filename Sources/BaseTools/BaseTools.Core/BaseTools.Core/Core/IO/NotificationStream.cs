namespace BaseTools.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;
    using BaseTools.Core.Utility;

    /// <summary>
    /// Stream wrapper that sends event about different operations.
    /// </summary>
    public class NotificationStream : WrapperStream
    {
        public NotificationStream(Stream internalStream)
        {
            this.InternalStream = internalStream;
        }

        public event EventHandler Disposing;

        protected override void Dispose(bool disposing)
        {
            this.Disposing.CallEvent(this, EventArgs.Empty);
            base.Dispose(disposing);
        }
    }
}
