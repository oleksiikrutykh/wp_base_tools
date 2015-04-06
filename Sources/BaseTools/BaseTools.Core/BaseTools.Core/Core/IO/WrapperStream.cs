namespace BaseTools.Core.IO
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// A wrapper that allows to change or extend capabilities of any stream. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Class inherited from stream.")]
    public class WrapperStream : Stream
    {
        public WrapperStream()
        {
        }

        protected Stream InternalStream { get; set; }

        public override bool CanRead
        {
            get
            {
                return this.InternalStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return this.InternalStream.CanSeek;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return this.InternalStream.CanWrite;
            }
        }

        public override void Flush()
        {
            this.InternalStream.Flush();
        }

        public override long Length
        {
            get
            {
                return this.InternalStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return this.InternalStream.Position;
            }
            set
            {
                this.InternalStream.Position = value;
            }
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return this.InternalStream.Read(buffer, offset, count);
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return this.InternalStream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            this.InternalStream.SetLength(value);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            this.InternalStream.Write(buffer, offset, count);
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DisposableHelper.DisposeSafe(this.InternalStream);
        }
    }
}
