namespace BaseTools.Core.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// An empty stub for logger.
    /// </summary>
    public class EmptyLogger : Logger
    {
        protected override Task WriteMessage(LoggingMessage message)
        {
            return Task.FromResult(true);
        }

        protected override Task SendMessages()
        {
            return Task.FromResult(true);
        }
    }
}
