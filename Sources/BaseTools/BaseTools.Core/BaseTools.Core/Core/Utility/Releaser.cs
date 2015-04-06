namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Signalizes about operation end.
    /// </summary>
    public class Releaser : IDisposable
    {
        private bool isDisposed;

        public event EventHandler Disposed;

        public Action OnDisposing { get; set; }

        /// <summary>
        /// Signalizes about operation end.
        /// </summary>
        public void Dispose()
        {
            if (!this.isDisposed)
            {
                this.isDisposed = true;
                var disposingAction = this.OnDisposing;
                if (disposingAction != null)
                {
                    disposingAction.Invoke();
                    this.OnDisposing = null;
                }

                this.Disposed.CallEvent(this, EventArgs.Empty);
            }
        }

        
    }
}
