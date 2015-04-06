namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides base logic for logging.
    /// </summary>
    public abstract class Logger
    {
        private static Logger logger = Factory.Common.GetInstance<Logger>();

        public static Logger Instance
        {
            get
            {
                return logger;
            }
        }

        public Logger()
        {
            this.IsLoggingEnabled = true;
        }



        public bool IsLoggingEnabled { get; set; }

        public Task Write(Exception ex)
        {
            var message = new LoggingMessage(ex);
            this.PrepareMessage(message);
            return this.Write(message);
        }

        public Task Write(string logMessage)
        {

            var message = new LoggingMessage(logMessage);
            this.PrepareMessage(message);
            return this.Write(message);
        }

        public Task Write(LoggingMessage message)
        {
            Task task = null;
            if (this.IsLoggingEnabled)
            {
                task = this.WriteMessage(message);
            }
            else
            {
                task = Task.FromResult(true);
            }

            return task;
        }

        public Task SendAsync()
        {
            Task task = null;
            if (this.IsLoggingEnabled)
            {
                task = this.SendMessages();
            }
            else
            {
                task = Task.FromResult(true);
            }

            return task;
        }

        protected virtual void PrepareMessage(LoggingMessage message)
        {
        }

        protected abstract Task WriteMessage(LoggingMessage message);

        protected abstract Task SendMessages();
    }
}
