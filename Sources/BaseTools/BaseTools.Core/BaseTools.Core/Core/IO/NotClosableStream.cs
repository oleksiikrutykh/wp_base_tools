namespace BaseTools.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Wrapper that allows to protect internal stream from closing.
    /// </summary>
    public class NotClosableStream : WrapperStream
    {
        public NotClosableStream(Stream internalStream)
        {
            this.InternalStream = internalStream;
        }

        protected override void Dispose(bool disposing)
        {

        }
    }
}
