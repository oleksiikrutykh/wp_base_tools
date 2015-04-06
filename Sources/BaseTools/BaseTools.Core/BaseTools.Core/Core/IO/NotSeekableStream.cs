namespace BaseTools.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Wrapper that blocks seek capability on origin stream.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Class inherited from stream.")]
    public class NotSeekableStream : WrapperStream
    {
        public NotSeekableStream(Stream internalStream)
        {
            this.InternalStream = internalStream;
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }
    }
}
