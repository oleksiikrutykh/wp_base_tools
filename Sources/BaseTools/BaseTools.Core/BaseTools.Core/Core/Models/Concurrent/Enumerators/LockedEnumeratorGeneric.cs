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
    /// Generic enumerator for concurrent collection
    /// </summary>
    /// <typeparam name="T">Type of collection item</typeparam>
    internal class LockedEnumerator<T> : IEnumerator<T>, IDisposable
    {
        private object lockObject;

        private IEnumerator<T> internalEnumerator;

        private bool isLocked;

        private readonly object localLocker = new object(); 

        public LockedEnumerator(Func<IEnumerator<T>> enumeratorGenerator, object syncValue)
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

        public T Current
        {
            get
            {
                return this.internalEnumerator.Current;
            }
        }

        object System.Collections.IEnumerator.Current
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
                this.internalEnumerator.Dispose();
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

        protected IEnumerator<T> InternalEnumerator
        {
            get
            {
                return this.internalEnumerator;
            }
        }

        ~LockedEnumerator()
        {
            this.Dispose(false);
        }
        
    }
}
