namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.DataAccess;
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Security;
    using BaseTools.Core.Storage;
    using BaseTools.Core.Threading;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class StorageLogger : Logger
    {
        private static readonly TimeSpan StoringPeriodLimit = TimeSpan.FromDays(30);

        private string logsStoragePath;

        private string logItemNamePattern;

        private string logSearchPattern;

        private Task initialziationTask;

        private AsyncLock sendingLock = new AsyncLock();

        public string StoragePath
        {
            get
            {
                return this.logsStoragePath;
            }

            set
            {
                this.logsStoragePath = value;
                this.logItemNamePattern = logsStoragePath + "/{0}";
                this.logSearchPattern = logsStoragePath + "/*";
            }
        }

        public StorageLogger()
        {
            this.StoragePath = "Logs";
        }

        public Task WhenInitialize()
        {
            if (initialziationTask == null)
            {
                initialziationTask = this.PerfomInitilaize();
            }

            return initialziationTask;
        }

        public Task PerfomInitilaize()
        {
            var fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
            return fileSystemProvider.CreateDirectoryAsync(logsStoragePath);
        }

        private string DetermineMessageStorageKey(LoggingMessage message)
        {
            var hashInput = message.ApplicationName + message.ApplicationVersion + message.Message;
            var key = MD5Calculator.GetMD5Hash(hashInput);
            return key;
        }

        protected async override Task WriteMessage(LoggingMessage message)
        {
            await this.WhenInitialize();
            var key = this.DetermineMessageStorageKey(message);
            var fileName = String.Format(this.logItemNamePattern, key);
            var storedData = await BufferedStorageProvider.Instance.ReadFromFile<StorableLoggingMessage>(fileName);
            if (storedData == null)
            {
                storedData = new StorableLoggingMessage();
                storedData.Message = message;
                storedData.SuccessfullSendDate = DateTime.UtcNow;
            }

            storedData.IncrementCount();
            await BufferedStorageProvider.Instance.WriteToFile(fileName, storedData);
        }

        protected override Task SendMessages()
        {
            return Task.Run(async () =>
            {
                using (await this.sendingLock.LockAsync())
                {
                    await this.WhenInitialize();
                    var networkProvider = Factory.Common.GetInstance<BaseTools.Core.Network.NetworkConnectionService>();
                    var isNetworkAvailable = await networkProvider.IsNetworkAvailable();
                    if (isNetworkAvailable)
                    {
                        var fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
                        var errors = await fileSystemProvider.FindFilesAsync(logSearchPattern);
                        foreach (var error in errors)
                        {
                            HandleMessage(error.Name);
                        }
                    }
                }
            });
        }

        private async void HandleMessage(string messageKey)
        {
            bool needDelete = false;
            var filePath = String.Format(logItemNamePattern, messageKey);
            var storageData = await BufferedStorageProvider.Instance.ReadFromFile<StorableLoggingMessage>(filePath);
            if (storageData != null)
            {
                var notSendedPeriod = DateTime.UtcNow - storageData.SuccessfullSendDate;
                if (notSendedPeriod < StoringPeriodLimit)
                {
                    await TrySendSingleMessage(storageData.Message, storageData.Count);
                    if (storageData.Count == 0)
                    {
                        needDelete = true;
                    }
                }
                else
                {
                    needDelete = true;
                }
            }
            else
            {
                needDelete = true;
            }

            if (needDelete)
            {
                await BufferedStorageProvider.Instance.RemoveFromFile(filePath);
            }
        }

        protected virtual async Task TrySendSingleMessage(LoggingMessage message, int errorsCount)
        {
            await ReportMessageSended(message, errorsCount);
        }

        protected async Task ReportMessageSended(LoggingMessage message, int successSend)
        {
            var key = this.DetermineMessageStorageKey(message);
            var filePath = String.Format(this.logItemNamePattern, key);

            var storedData = await BufferedStorageProvider.Instance.ReadFromFile<StorableLoggingMessage>(filePath);
            for (int i = 0; i < successSend; i++)
            {
                storedData.DecrementCount();
            }

            await BufferedStorageProvider.Instance.WriteToFile(filePath, storedData);
        }
    }
}
