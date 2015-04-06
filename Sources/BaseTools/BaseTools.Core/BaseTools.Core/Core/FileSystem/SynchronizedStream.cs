namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.IO;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Stream wrapper that blocks multiple access to internal stream.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Class inherited from stream.")]
    public class SynchronizedStream : WrapperStream
    {
        private AsyncLockReleaser accessLocker;

        public SynchronizedStream(Stream stream, AsyncLockReleaser lockObject)
        {
            this.InternalStream = stream;
            this.accessLocker = lockObject;
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            DisposableHelper.DisposeSafe(this.InternalStream);
            DisposableHelper.DisposeSafe(this.accessLocker);
        }
    }
}
