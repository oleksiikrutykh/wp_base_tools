namespace BaseTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents info about error.
    /// </summary>
    public class ErrorOccurredEventArgs : EventArgs
    {
        public ErrorOccurredEventArgs(Exception exception)
        {
            this.Exception = exception;
        }

        public Exception Exception { get; private set; }
    }
}
