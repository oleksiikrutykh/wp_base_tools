namespace BaseTools.Core.DataAccess
{
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Serialization;
    using BaseTools.Core.Storage;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class OperationRepeater<T>
    {
        private const string ExecutionFolder = "/PendedOperationsData/";

        private readonly string dataKey;

        private Func<T, Task<bool>> action;

        private AsyncLock dataLock = new AsyncLock();

        private AsyncLock executionLock = new AsyncLock();

        private IEqualityComparer<T> dataEqualityComparer;

        private Dictionary<T, OperationStatistics> storedData;

        private IStorageProvider storageProvider = Factory.Common.GetInstance<IStorageProvider>();

        public OperationRepeater(string operationKey, Func<T, Task<bool>> action)
            : this(operationKey, action, true)
        {
        }

        public OperationRepeater(string operationKey, Func<T, Task<bool>> action, bool isStartAutomatically)
        {
            this.RetriesCountLimit = 50;
            this.RetriesPeriodLimit = TimeSpan.FromDays(30);
            this.dataKey = ExecutionFolder + operationKey;
            this.action = action;
            if (isStartAutomatically)
            {
                this.Start();
            }
        }

        public void Start()
        {
            Task.Run(() => this.ExecuteStoredItems());
        }

        public TimeSpan RetriesPeriodLimit { get; set; }

        public int RetriesCountLimit { get; set; }

        public event EventHandler<OperationCompletedEventArgs<T>> OperationCompleted;

        public IEqualityComparer<T> EqualityComparer
        {
            get
            {
                return this.dataEqualityComparer;
            }

            set
            {
                this.dataEqualityComparer = value;
            }
        }

        public ISerializer Serializer { get; set; }

        private ISerializer CurrentSerializer 
        {
            get
            {
                var serializer = this.Serializer;
                if (serializer == null)
                {
                    serializer = this.storageProvider.Serializer; 
                }

                return serializer;
            }
        }

        public async Task Execute(T item)
        {
            using (await dataLock.LockAsync().ConfigureAwait(false))
            {
                await this.LoadPendedItemsToMemoryCache();
                OperationStatistics statistics = null;
                var isExist = this.storedData.TryGetValue(item, out statistics);
                if (!isExist)
                {
                    statistics = new OperationStatistics
                    {
                        LastSuccessExecutionDate = DateTime.UtcNow
                    };

                    storedData[item] = statistics;
                }

                statistics.Count++;
                await storageProvider.WriteToFileAsync<Dictionary<T, OperationStatistics>>(dataKey, storedData, this.CurrentSerializer);
            }

            await this.ExecuteStoredItems();
        }

        private async Task LoadPendedItemsToMemoryCache()
        {
            if (this.storedData == null)
            {
                var fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
                await fileSystemProvider.CreateDirectoryAsync(ExecutionFolder);
                this.storedData = await storageProvider.ReadFromFileAsync<Dictionary<T, OperationStatistics>>(dataKey, this.CurrentSerializer);
                if (this.storedData == null)
                {
                    this.storedData = new Dictionary<T, OperationStatistics>();
                }

                if (this.EqualityComparer != null)
                {
                    this.storedData = new Dictionary<T, OperationStatistics>(this.storedData, this.EqualityComparer);
                }
            }
        }

        private async Task ExecuteStoredItems()
        {
            using (await this.executionLock.LockAsync().ConfigureAwait(false))
            {
                Dictionary<T, OperationStatistics> executedData = new Dictionary<T, OperationStatistics>();
                using (await dataLock.LockAsync())
                {
                    await this.LoadPendedItemsToMemoryCache();
                    foreach (var item in this.storedData)
                    {
                        executedData[item.Key] = item.Value.Clone();
                    }
                }

                List<Task> allTasks = new List<Task>();
                var currentTime = DateTime.UtcNow;
                foreach (var item in executedData)
                {
                    var dataItem = item.Key;
                    var statistics = item.Value;
                    var expirationDate = statistics.LastSuccessExecutionDate.Add(this.RetriesPeriodLimit);
                    var isExpired = currentTime > expirationDate &&
                                    statistics.UnsuccessfulTriesCount > this.RetriesCountLimit;
                    if (isExpired)
                    {
                        var removingTask = this.RemoveOperation(dataItem);
                        allTasks.Add(removingTask);
                    }
                    else
                    {
                        for (int i = 0; i < statistics.Count; i++)
                        {
                            var executionTask = PerformOperation(dataItem, statistics);
                            allTasks.Add(executionTask);
                        }
                    }
                }

                await Task.WhenAll(allTasks);
            }
        }

        private async Task PerformOperation(T dataItem, OperationStatistics executionStatistics)
        {
            var isSuccess = await this.action.Invoke(dataItem);
            using (await dataLock.LockAsync())
            {
                OperationStatistics itemStatistics = null;
                var isExist = this.storedData.TryGetValue(dataItem, out itemStatistics);
                if (isExist)
                {
                    if (isSuccess)
                    {
                        itemStatistics.Count--;
                        if (itemStatistics.Count == 0)
                        {
                            this.storedData.Remove(dataItem);
                            this.OperationCompleted.CallEvent(this, new OperationCompletedEventArgs<T>(dataItem, true));
                        }
                        else
                        {
                            itemStatistics.LastSuccessExecutionDate = DateTime.UtcNow;
                            itemStatistics.UnsuccessfulTriesCount = 0;
                        }
                    }
                    else
                    {
                        itemStatistics.UnsuccessfulTriesCount++;
                    }

                    await storageProvider.WriteToFileAsync<Dictionary<T, OperationStatistics>>(dataKey, this.storedData, this.CurrentSerializer);
                }
            }
        }

        private async Task RemoveOperation(T item)
        {
            using (await dataLock.LockAsync())
            {
                this.storedData.Remove(item);
                await storageProvider.WriteToFileAsync<Dictionary<T, OperationStatistics>>(dataKey, this.storedData, this.CurrentSerializer);
            }

            this.OperationCompleted.CallEvent(this, new OperationCompletedEventArgs<T>(item, false));
        }
    }
}
