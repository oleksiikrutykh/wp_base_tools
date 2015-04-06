namespace BaseTools.Core.Models.Concurrent
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// Base enumeration for concurrent collection.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1038:EnumeratorsShouldBeStronglyTyped", Justification="Support any type")]
    internal class LockedEnumerator : IEnumerator, IDisposable
    {
        private object lockObject;

        private IEnumerator internalEnumerator;

        private bool isLocked;

        private readonly object localLocker = new object(); 

        public LockedEnumerator(Func<IEnumerator> enumeratorGenerator, object syncValue)
        {
            Guard.CheckIsNotNull(enumeratorGenerator, "enumeratorGenerator");
            this.lockObject = syncValue;
            lock (localLocker)
            {
                Monitor.Enter(this.lockObject);
                this.isLocked = true;
            }
            this.internalEnumerator = enumeratorGenerator.Invoke();
        }

        public object Current
        {
            get
            {
                return this.internalEnumerator.Current;
            }
        }

        public bool MoveNext()
        {
            var canMoveNext = this.internalEnumerator.MoveNext();
            if (!canMoveNext)
            {
                if (this.isLocked)
                {
                    lock (this.lockObject)
                    {
                        if (this.isLocked)
                        {
                            this.isLocked = false;
                            Monitor.Exit(this.lockObject);
                        }
                    }
                }
            }

            return canMoveNext;
        }

        public void Reset()
        {
            if (!this.isLocked)
            {
                lock (this.lockObject)
                {
                    if (!this.isLocked)
                    {
                        this.isLocked = true;
                        Monitor.Enter(this.lockObject);
                    }
                }
            }

            this.internalEnumerator.Reset();
        }

        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            try
            {
                var internalDisposable = this.internalEnumerator as IDisposable;
                if (internalDisposable != null)
                {
                    internalDisposable.Dispose();
                }
            }
            finally
            {
                if (this.isLocked)
                {
                    lock (this.lockObject)
                    {
                        if (this.isLocked)
                        {
                            this.isLocked = false;
                            Monitor.Exit(this.lockObject);
                        }
                    }
                }
            }
        }

        ~LockedEnumerator()
        {
            this.Dispose(false);
        }
    }
}
