namespace BaseTools.Core.Storage
{
    using BaseTools.Core.FileSystem;
using BaseTools.Core.Info;
using BaseTools.Core.Ioc;
using BaseTools.Core.Security;
using BaseTools.Core.Threading;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

    public class CacheProvider
    {
        private string CachedFilesDirectoryName;

        private string CachedFilePathPattern;

        private string MemoryCacheSearchPattern;

        private string CachedFilePathSearchPattern;

        private const string StarCode = "%42";

        private const string Star = "*";

        private const int MaxFullPathLength = 260;

        private const string CacheLifetimeMapStorageKey = "CacheLifetimeMap";

        private static readonly CacheProvider instance = new CacheProvider();

        private static readonly TimeSpan StoringDataPeriod = TimeSpan.FromDays(30);

        private IStorageProvider storageProvider;

        private IFileSystemProvider fileSystemProvider;

        private BufferedStorageProvider bufferedStorageProvider = BufferedStorageProvider.Instance;

        private static readonly DateTime ExpirationValue = new DateTime(1600, 1, 1);

        private Dictionary<string, DateTime> cacheLifetimeMap;

        private Task initializationTask;

        private object cacheLifetimeMapLock = new object();

        private RepeatedOperation lifetimesWriter;

        private EnvironmentInfo environmentInfo = EnvironmentInfo.Current;

        public CacheProvider()
        {
            this.storageProvider = Factory.Common.GetInstance<IStorageProvider>();
            this.fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
            this.lifetimesWriter = new RepeatedOperation
            {
                Operation = this.WriteCacheLifetimeMap
            };

            this.CacheDuration = TimeSpan.FromMinutes(30);
            this.DirectoryName = "Cache";
        }

        public static CacheProvider Instance
        {
            get
            {
                return instance;
            }
        }

        public string DirectoryName
        {
            get
            {
                return this.CachedFilesDirectoryName;
            }

            set
            {
                if (this.CachedFilesDirectoryName != value)
                {
                    this.CachedFilesDirectoryName = value;
                    this.CachedFilePathPattern = "/" + CachedFilesDirectoryName + "/{0}";
                    this.MemoryCacheSearchPattern = "/" + CachedFilesDirectoryName;
                    this.CachedFilePathSearchPattern = "/" + CachedFilesDirectoryName + "/*";

                    this.InitializeOnStart();
                }
            }
        }

        public TimeSpan CacheDuration { get; set; }

        public async Task<List<string>> LoadAllStoredKeys()
        {
            await this.InitializeOnStart();
            List<string> allKeys = new List<string>(this.cacheLifetimeMap.Keys);
            return allKeys;
        }

        public async Task<bool> IsExpired(string cacheKey)
        {
            await this.InitializeOnStart();
            var putedInCache = DateTime.MinValue;
            this.cacheLifetimeMap.TryGetValue(cacheKey, out putedInCache);
            var cacheLifetimeValue = DateTime.UtcNow - putedInCache;
            bool isExpired = cacheLifetimeValue > CacheDuration;
            return isExpired;
        }

        public async Task MarkAsLoaded(string cacheKey)
        {
            await this.InitializeOnStart();
            lock (cacheLifetimeMapLock)
            {
                this.cacheLifetimeMap[cacheKey] = DateTime.UtcNow;
            }

            this.lifetimesWriter.Perform();
        }

        public async Task MarkAsExpired(string cacheKey)
        {
            await this.InitializeOnStart();
            if (this.cacheLifetimeMap.ContainsKey(cacheKey))
            {
                lock (cacheLifetimeMapLock)
                {
                    if (environmentInfo.OperatingSystemType == OperatingSystemType.WindowsPhoneSilverlight)
                    {
                        this.cacheLifetimeMap.Remove(cacheKey);
                    }
                    else if (environmentInfo.OperatingSystemType == OperatingSystemType.Windows)
                    {
                        this.cacheLifetimeMap[cacheKey] = ExpirationValue;
                    }
                }

                this.lifetimesWriter.Perform();
            }
        }

        public Task MarkAsExpired(Regex expiredKeyRegex)
        {
            return Task.Run(async () =>
            {
                await this.InitializeOnStart();
                var keys = this.cacheLifetimeMap.Keys.ToList();
                var listRemovedKeys = new List<string>();
                foreach (var key in keys)
                {
                    var isMatch = expiredKeyRegex.IsMatch(key);
                    if (isMatch)
                    {
                        listRemovedKeys.Add(key);
                    }
                }

                lock (cacheLifetimeMapLock)
                {
                    foreach (var removedItem in listRemovedKeys)
                    {
                        if (environmentInfo.OperatingSystemType == OperatingSystemType.WindowsPhoneSilverlight)
                        {
                            this.cacheLifetimeMap.Remove(removedItem);
                        }
                        else if (environmentInfo.OperatingSystemType == OperatingSystemType.Windows)
                        {
                            this.cacheLifetimeMap[removedItem] = ExpirationValue;
                        }
                    }
                }

                this.lifetimesWriter.Perform();
            });
        }

        public async Task<T> ReadFromCache<T>(string cacheKey)
        {
            var filePath = ConvertToFilePath(cacheKey);
            bool needCheckFile = true;

            if (environmentInfo.OperatingSystemType == OperatingSystemType.Windows)
            {
                await this.InitializeOnStart();
                needCheckFile = this.cacheLifetimeMap.ContainsKey(cacheKey);
            }

            return await this.bufferedStorageProvider.ReadFromFile<T>(filePath, needCheckFile);
        }

        public async Task WriteIntoCache<T>(string cacheKey, T value)
        {
            await this.InitializeOnStart();
            var filePath = ConvertToFilePath(cacheKey);
            await this.bufferedStorageProvider.WriteToFile<T>(filePath, value);
            //await this.MarkAsLoaded(cacheKey);
        }

        public void ClearMemoryCache()
        {
            var allKeys = this.bufferedStorageProvider.MemoryCache.Keys.ToList();
            foreach (var key in allKeys)
            {
                if (key.StartsWith(MemoryCacheSearchPattern, StringComparison.Ordinal))
                {
                    this.bufferedStorageProvider.ClearInMemoryCache(key);
                }
            }
        }

        private Task WriteCacheLifetimeMap()
        {
            Dictionary<string, DateTime> savedData = null;
            lock (cacheLifetimeMapLock)
            {
                savedData = new Dictionary<string, DateTime>(this.cacheLifetimeMap);
            }

            return storageProvider.WriteToFileAsync(CacheLifetimeMapStorageKey, savedData);
        }

        private Task InitializeOnStart()
        {
            if (this.initializationTask == null)
            {
                this.initializationTask = this.DoInitialize();
            }

            return initializationTask;
        }

        private async Task DoInitialize()
        {
            await this.fileSystemProvider.CreateDirectoryAsync(CachedFilesDirectoryName);
            if (this.cacheLifetimeMap == null)
            {
                this.cacheLifetimeMap = await storageProvider.ReadFromFileAsync<Dictionary<string, DateTime>>(CacheLifetimeMapStorageKey);
                if (this.cacheLifetimeMap == null)
                {
                    this.cacheLifetimeMap = new Dictionary<string, DateTime>();
                }
            }
        }

        private string ConvertToFilePath(string key)
        {
            key = ConvertCacheKey(key);
            return String.Format(CultureInfo.InvariantCulture, CachedFilePathPattern, key);
        }

        private string ConvertCacheKey(string cacheKey)
        {
            cacheKey = cacheKey.Replace(Star, StarCode);
            cacheKey = Uri.EscapeUriString(cacheKey);
            cacheKey = Uri.EscapeDataString(cacheKey);
            cacheKey = GetHashOfCacheKeyIfLong(cacheKey);

            return cacheKey;
        }

        private string GetHashOfCacheKeyIfLong(string cacheKey)
        {
            var localPath = string.Format(CultureInfo.InvariantCulture, CachedFilePathPattern, cacheKey);
            var fullPath = fileSystemProvider.GetFullFilePath(localPath);
            if (fullPath.Length >= MaxFullPathLength)
            {
                cacheKey = MD5Calculator.GetMD5Hash(cacheKey);
            }

            return cacheKey;
        }


        public Task DeleteOldCacheFiles()
        {
            return Task.Run(async () =>
            {
                await this.InitializeOnStart();
                var nowTime = DateTime.Now;
                var cacheFiles = await this.fileSystemProvider.FindFilesAsync(CachedFilePathSearchPattern);
                var deletedFiles = new List<string>();
                foreach (var file in cacheFiles)
                {
                    var fileProperties = await file.GetPropertiesAsync();
                    var lastAccess = fileProperties.LastAccessDate;
                    if (nowTime > lastAccess.Add(StoringDataPeriod))
                    {
                        var filePath = String.Format(CachedFilePathPattern, file.Name);
                        deletedFiles.Add(filePath);
                        await this.bufferedStorageProvider.RemoveFromFile(filePath);
                    }
                }

                lock (cacheLifetimeMapLock)
                {
                    if (deletedFiles.Count > 0)
                    {
                        var nameMapping = new Dictionary<string, string>();
                        foreach (var item in this.cacheLifetimeMap)
                        {
                            var filePath = this.ConvertCacheKey(item.Key);
                            nameMapping[filePath] = item.Key;
                        }

                        foreach (var oldFile in deletedFiles)
                        {
                            string key = null;
                            var isExist = nameMapping.TryGetValue(oldFile, out key);
                            if (isExist)
                            {
                                this.cacheLifetimeMap.Remove(key);
                            }
                        }
                    }
                }

                this.lifetimesWriter.Perform();
            });
        }

        public Task ClearCache()
        {
            return Task.Run(async () =>
            {
                await this.InitializeOnStart();
                this.cacheLifetimeMap = new Dictionary<string, DateTime>();
                this.lifetimesWriter.Perform();

                var cacheFiles = await this.fileSystemProvider.FindFilesAsync(CachedFilePathSearchPattern);
                var deleteTasks = new List<Task>();
                foreach (var file in cacheFiles)
                {
                    var filePath = String.Format(CachedFilePathPattern, file.Name);
                    var task = this.bufferedStorageProvider.RemoveFromFile(filePath);
                    deleteTasks.Add(task);
                }

                await Task.WhenAll(deleteTasks);
            });
        }
    }
}
