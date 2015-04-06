namespace BaseTools.Core.Storage
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Models.Concurrent;
    using BaseTools.Core.Serialization;

    /// <summary>
    /// Provides logic for storing data in file system. Allow buffering used data in memory.
    /// </summary>
    public class BufferedStorageProvider
    {
        private IStorageProvider storageProvider = Factory.Common.GetInstance<IStorageProvider>();

        private ConcurrentDictionary<string, object> memoryCache = new ConcurrentDictionary<string, object>();

        private ConcurrentDictionary<string, AsyncLock> readOrCreateOperationLocks = new ConcurrentDictionary<string, AsyncLock>();

        private static BufferedStorageProvider instance = new BufferedStorageProvider();

        public static BufferedStorageProvider Instance
        {
            get
            {
                return instance;
            }
        }

        internal ConcurrentDictionary<string, object> MemoryCache
        {
            get
            {
                return this.memoryCache;
            }
        }

        public Task<T> ReadFromFile<T>(string filePath) 
        {
            return this.ReadFromFile<T>(filePath, true);
        }

        public Task<T> ReadFromFile<T>(string filePath, ISerializer serializer)
        {
            return ReadFromFile<T>(filePath, true, true, serializer);
        }

        public Task<T> ReadFromFile<T>(string filePath, bool needCheckFileStorage)
        {
            return ReadFromFile<T>(filePath, needCheckFileStorage, true, this.storageProvider.Serializer);
        }

        private async Task<T> ReadFromFile<T>(string filePath, bool needCheckFileStorage, bool needCreateDefaultValue, ISerializer serializer)
        {
            object result = default(T);
            bool containsInMemoryCache = this.memoryCache.TryGetValue(filePath, out result);
            if (!containsInMemoryCache)
            {
                result = default(T);
                if (needCheckFileStorage)
                {
                    result = await this.storageProvider.ReadFromFileAsync<T>(filePath, serializer);
                }

                bool hasValue = !Object.Equals(result, default(T));
                if (!hasValue)
                {
                    if (needCreateDefaultValue)
                    {
                        var type = typeof(T);
                        var typeInfo = type.GetTypeInfo();
                        bool isCollection = typeInfo.ImplementedInterfaces.Contains(typeof(ICollection));
                        if (isCollection)
                        {
                            result = Activator.CreateInstance(typeof(T));
                            hasValue = true;
                        }
                    }
                }

                if (hasValue)
                {
                    // Try put data into cache, if it isn't already contains it.
                    result = this.memoryCache.GetOrAdd(filePath, () => result);
                }
            }

            return (T)result;
        }

        public Task WriteToFile<T>(string filePath, T value)
        {
            return WriteToFile<T>(filePath, value, this.storageProvider.Serializer);
        }

        public async Task WriteToFile<T>(string filePath, T value, ISerializer serializer)
        {
            this.memoryCache[filePath] = value;
            var type = typeof(T);
            var writedValue = CreateFileWritedValue<T>(value);
            await this.storageProvider.WriteToFileAsync<T>(filePath, writedValue, serializer);
        }

        public Task<bool> RemoveFromFile(string filePath)
        {
            this.ClearInMemoryCache(filePath);
            return this.storageProvider.DeleteFromFileAsync(filePath);
        }

        public async Task<T> ReadOrCreate<T>(string filePath, Func<T> generationFunction)
        {
            T result = default(T);
            object objectResult = null;
            bool containsInMemoryCache = this.memoryCache.TryGetValue(filePath, out objectResult);
            if (!containsInMemoryCache)
            {
                var asyncLock = this.GetInitializeLock(filePath);
                using (await asyncLock.LockAsync())
                {
                    containsInMemoryCache = this.memoryCache.TryGetValue(filePath, out objectResult);
                    if (!containsInMemoryCache)
                    {
                        result = await this.ReadFromFile<T>(filePath, true, false, this.storageProvider.Serializer);
                        if (Object.Equals(result, default(T)))
                        {
                            result = generationFunction.Invoke();
                            await this.WriteToFile(filePath, result);
                        }
                    }
                    else
                    {
                        result = (T)objectResult;
                    }
                }
            }
            else
            {
                result = (T)objectResult;
            }

            return result;
        }

        private AsyncLock GetInitializeLock(string dataKey)
        {
            return this.readOrCreateOperationLocks.GetOrAdd(dataKey);
        }

        private static T CreateFileWritedValue<T>(T value)
        {
            var result = value;
            var type = typeof(T);
            var typeInfo = type.GetTypeInfo();
            bool isList = typeInfo.ImplementedInterfaces.Contains(typeof(IList));
            if (isList)
            {
                var list = (IList)Activator.CreateInstance(type);
                var enumerable = (IEnumerable)value;
                if (enumerable != null)
                {
                    foreach (var item in enumerable)
                    {
                        list.Add(item);
                    }
                }

                result = (T)list;
            }
            else
            {
                bool isDictionary = typeInfo.ImplementedInterfaces.Contains(typeof(IDictionary));
                if (isDictionary)
                {
                    var dictionary = (IDictionary)Activator.CreateInstance(type);
                    var sourceDictionary = (IDictionary)value;
                    if (sourceDictionary != null)
                    {
                        foreach (DictionaryEntry item in sourceDictionary)
                        {
                            dictionary.Add(item.Key, item.Value);
                        }
                    }

                    result = (T)dictionary;
                }
            }

            return result;
        }

        public void ClearInMemoryCache(string path)
        {
            this.memoryCache.Remove(path); 
        }

        public void ClearMemoryCache()
        {
            this.memoryCache = new ConcurrentDictionary<string, object>();
        }
    }
}
