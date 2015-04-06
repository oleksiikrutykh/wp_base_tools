namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.Threading;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Foundation;
    using Windows.Storage.Streams;

    /// <summary>
    /// Wrapper for lock access to <see cref="IRandomAccessStream"/> instanse.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Justification = "Class is stream.")]
    internal sealed class RandomAccessSynchronizedStream : IRandomAccessStream
    {
        private IRandomAccessStream internalStream;

        private AsyncLockReleaser accessLocker;

        public RandomAccessSynchronizedStream(IRandomAccessStream stream, AsyncLockReleaser lockObject)
        {
            this.internalStream = stream;
            this.accessLocker = lockObject;
        }

        public IRandomAccessStream InternalStream
        {
            get
            {
                return this.internalStream;
            }
        }

        public AsyncLockReleaser AccessLocker
        {
            get
            {
                return this.accessLocker;
            }
        }

        public bool CanRead
        {
            get 
            { 
                return this.internalStream.CanRead;
            }
        }

        public bool CanWrite
        {
            get 
            {
                return this.internalStream.CanWrite;
            }
        }

        public IRandomAccessStream CloneStream()
        {
            var clonedStream = new RandomAccessSynchronizedStream(this.internalStream, accessLocker);
            return clonedStream;
        }

        public IInputStream GetInputStreamAt(ulong position)
        {
            return this.internalStream.GetInputStreamAt(position);
        }

        public IOutputStream GetOutputStreamAt(ulong position)
        {
            return this.GetOutputStreamAt(position);
        }

        public ulong Position
        {
            get 
            {
                return this.internalStream.Position;
            }
        }

        public void Seek(ulong position)
        {
            this.internalStream.Seek(position);
        }

        public ulong Size
        {
            get
            {
                return this.internalStream.Size;
            }
            set
            {
                this.internalStream.Size = value;
            }
        }

        public void Dispose()
        {
            this.internalStream.Dispose();
            this.accessLocker.Dispose();
        }

        public IAsyncOperationWithProgress<IBuffer, uint> ReadAsync(IBuffer buffer, uint count, InputStreamOptions options)
        {
            return this.internalStream.ReadAsync(buffer, count, options);
        }

        public Windows.Foundation.IAsyncOperation<bool> FlushAsync()
        {
            return this.internalStream.FlushAsync();
        }

        public Windows.Foundation.IAsyncOperationWithProgress<uint, uint> WriteAsync(IBuffer buffer)
        {
            return this.internalStream.WriteAsync(buffer);
        }
    }
}
