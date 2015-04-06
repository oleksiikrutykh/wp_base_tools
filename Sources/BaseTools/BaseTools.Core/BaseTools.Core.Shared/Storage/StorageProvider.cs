namespace BaseTools.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using BaseTools.Core.Storage;
    using BaseTools.Core.Serialization;
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Models;
    using BaseTools.Core.IO;
    using Windows.Storage;

#if SILVERLIGHT
    using System.IO.IsolatedStorage;
#endif

    /// <summary>
    /// Provides logic for tombstoning objects to isolated storage.
    /// </summary>
    internal class StorageProvider : IStorageProvider
    {
        /// <summary>
        /// 
        /// </summary>
        public StorageProvider()
        {
            this.Serializer = new BinarySerializer();
            this.NeedSuppressErrors = true;
            this.UseContentValidation = true;
        }

        /// <summary>
        /// Name of file contains application settings.
        /// </summary>
        private const string IsolatedStorageSettingsFileName = "__ApplicationSettings";

        /// <summary>
        /// Object used for synchronized access of fileAccessLocks dictionary.
        /// </summary>
        private object dictionaryLock = new object();

        /// <summary>
        /// Dictionary of lock objects used for every file in isolated storage.
        /// </summary>
        private Dictionary<string, object> fileAccessLocks = new Dictionary<string, object>();

        private IFileSystemProvider fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();

        private bool isApplicationClosed;

        private TaskCompletionSource<bool> applicationReactivatedTaskSource = new TaskCompletionSource<bool>();

        private List<Task> notCompletedWrites = new List<Task>();

        private readonly object writesPendingLock = new object();

        /// <summary>
        /// Gets or sets serializer used for storing data into file.
        /// </summary>
        public ISerializer Serializer { get; set; }

        public bool UseContentValidation { get; set; }

        public bool NeedSuppressErrors { get; set; }

        public event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Read object from IsolatedStorageSettings collection
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="key">The key of value to get.</param>
        /// <returns>The value associated with the specified key. If there is no value with specified key, method return default(T).</returns>
        public T ReadFromSettings<T>(string key)
        {
            T result = default(T);
            try
            {
                var fileAccessLock = GetFileAccessLockObject(IsolatedStorageSettingsFileName);
                lock (fileAccessLock)
                {
#if WINRT
                    var settings = ApplicationData.Current.LocalSettings;
                    object resultObj = null;
                    if (settings.Values.TryGetValue(key, out resultObj))
                    {
                        result = (T)resultObj;
                    }
#endif

#if SILVERLIGHT
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        if (IsolatedStorageSettings.ApplicationSettings[key] is T)
                        {
                            result = (T)IsolatedStorageSettings.ApplicationSettings[key];
                        }
                    }
#endif
                }
            }
            catch (Exception ex)
            {
                RaiseErrorOccured(ex);
                if (!this.NeedSuppressErrors)
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Write object to IsolatedStorageSettings collection.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="key">The key for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        public void WriteToSettings<T>(string key, T value)
        {
            try
            {
                var fileAccessLock = GetFileAccessLockObject(IsolatedStorageSettingsFileName);
                lock (fileAccessLock)
                {
#if WINRT
                    var settings = ApplicationData.Current.LocalSettings;
                    settings.Values[key] = value;
#endif

#if SILVERLIGHT
                    IsolatedStorageSettings.ApplicationSettings[key] = value;
                    IsolatedStorageSettings.ApplicationSettings.Save();
#endif
                }
            }
            catch (Exception ex)
            {
                RaiseErrorOccured(ex);
                if (!this.NeedSuppressErrors)
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deleted value from settings.
        /// </summary>
        /// <param name="key">The key of deleted object</param>
        /// <returns>Value indicates whether key was founded.</returns>
        public bool DeleteFromSetting(string key)
        {
            bool result = false;
            try
            {
                var fileAccessLock = GetFileAccessLockObject(IsolatedStorageSettingsFileName);
                lock (fileAccessLock)
                {
#if WINRT
                    var settings = ApplicationData.Current.LocalSettings;
                    result = settings.Values.Remove(key);
#endif

#if SILVERLIGHT
                    if (IsolatedStorageSettings.ApplicationSettings.Contains(key))
                    {
                        IsolatedStorageSettings.ApplicationSettings.Remove(key);
                        result = true;
                    }
#endif

                }
            }
            catch (Exception ex)
            {
                RaiseErrorOccured(ex);
                if (!this.NeedSuppressErrors)
                {
                    throw;
                }
            }

            return result;
        }

        /// <summary>
        /// Read object from file system. Use global serializer.
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="fileName">The fileName of value to get.</param>
        /// <returns>An asyncronous operation, returned object stored in specified file. If there is no such file, method return default(T).</returns>
        public Task<T> ReadFromFileAsync<T>(string fileName)
        {
            return this.ReadFromFileAsync<T>(fileName, this.Serializer);
        }

        /// <summary>
        /// Read object from file system. Use your deserialization.
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="fileName">The fileName of value to get.</param>
        /// <param name="serializer">Serializer used for storing data into file.</param>
        /// <returns>An asyncronous operation, returned object stored in specified file. If there is no such file, method return default(T).</returns>        
        public Task<T> ReadFromFileAsync<T>(string fileName, ISerializer serializer)
        {
            return Task.Run(async () =>
            {
                T result = default(T);
                try
                {
                    using (var stream = await this.fileSystemProvider.OpenFileAsync(fileName, BaseTools.Core.FileSystem.FileOpeningMode.OpenIfExist))
                    {
                        if (stream != null)
                        {
                            if (this.UseContentValidation)
                            {
                                var check = stream.ReadByte();
                                if (check == 1)
                                {
                                    using (var unseekableStream = new NotSeekableStream(stream))
                                    {
                                        result = serializer.Deserialize<T>(unseekableStream);
                                    }
                                }
                            }
                            else
                            {
                                result = serializer.Deserialize<T>(stream);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    RaiseErrorOccured(ex);
                    if (!this.NeedSuppressErrors)
                    {
                        throw;
                    }
                }

                return result;
            });
        }

        /// <summary>
        /// Write object to file system. Use global serializer.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="fileName">The fileName for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        public Task WriteToFileAsync<T>(string fileName, T value)
        {
            return this.WriteToFileAsync<T>(fileName, value, this.Serializer);
        }

        /// <summary>
        /// Write object to file system. Use custom serializer.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="fileName">The fileName for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        /// <param name="serializer">Serializer used for storing data into file.</param>
        public async Task WriteToFileAsync<T>(string fileName, T value, ISerializer serializer)
        {
            try
            {
                bool isCompleted = false;
                while (!isCompleted)
                {
                    if (!this.isApplicationClosed)
                    {
                        var task = this.PerformWriteToFileAsync<T>(fileName, value, serializer);
                        lock (this.writesPendingLock)
                        {
                            this.notCompletedWrites.Add(task);
                        }

                        await task.ContinueWith((endedTask) =>
                        {
                            try
                            {
                                isCompleted = endedTask.Result;
                            }
                            finally
                            {
                                lock (this.writesPendingLock)
                                {
                                    this.notCompletedWrites.Remove(task);
                                }
                            }
                        });
                    }

                    if (!isCompleted)
                    {
                        await this.applicationReactivatedTaskSource.Task;
                    }
                }
            }
            catch (Exception ex)
            {
                RaiseErrorOccured(ex);
                if (!this.NeedSuppressErrors)
                {
                    throw;
                }
            }
        }
        /// <summary>
        /// Write object to IsolatedStorageFile. Use custom serializer.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="fileName">The fileName for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        /// <param name="serializer">Serializer used for storing data into file.</param>
        public Task<bool> PerformWriteToFileAsync<T>(string fileName, T value, ISerializer serializer)
        {
            return Task.Run(async () =>
            {
                bool isCompleted = false;
                if (!Object.Equals(value, default(T)))
                {

                    using (var stream = await this.fileSystemProvider.OpenFileAsync(fileName, BaseTools.Core.FileSystem.FileOpeningMode.Create))
                    {
                        if (this.UseContentValidation)
                        {
                            stream.WriteByte(0);
                            using (var unseekableStream = new NotSeekableStream(stream))
                            {
                                serializer.Serialize(unseekableStream, value);
                                stream.Seek(0, SeekOrigin.Begin);
                                stream.WriteByte(1);
                            }
                        }
                        else
                        {
                            serializer.Serialize(stream, value);
                        }
                    }
                }
                else
                {
                    await this.fileSystemProvider.DeleteFileAsync(fileName);
                }

                isCompleted = true;
                return isCompleted;
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        public void SuspendStorage()
        {
            Task[] waitedWrites = null;
            lock (this.writesPendingLock)
            {
                this.isApplicationClosed = true;
                this.applicationReactivatedTaskSource = new TaskCompletionSource<bool>();
                waitedWrites = this.notCompletedWrites.ToArray();
            }

            //  not listen any exception here. They must thrown from WriteMetods.
            try
            {
                Task.WaitAll(waitedWrites);
            }
            catch
            {
            }
        }

        public void ActivateStorage()
        {
            this.isApplicationClosed = false;
            this.applicationReactivatedTaskSource.TrySetResult(true);
        }

        /// <summary>
        /// Deleted the value, stored in file.
        /// </summary>
        /// <param name="fileName">Name of the deleted file.</param>
        /// <returns>An asyncronous operation returns value indicates whether file was founded.</returns>
        public Task<bool> DeleteFromFileAsync(string fileName)
        {
            return Task.Run(async () =>
            {
                bool result = false;
                try
                {
                    result = await this.fileSystemProvider.DeleteFileAsync(fileName);
                }
                catch (Exception ex)
                {
                    RaiseErrorOccured(ex);
                    if (!this.NeedSuppressErrors)
                    {
                        throw;
                    }
                }

                return result;
            });
        }

        /// <summary>
        /// Gets lock object for synchronized access specified file.
        /// </summary>
        /// <param name="filePath">Path of accessed file</param>
        /// <returns>Object used for file synchronized access.</returns>
        private object GetFileAccessLockObject(string filePath)
        {
            if (!this.fileAccessLocks.ContainsKey(filePath))
            {
                lock (this.dictionaryLock)
                {
                    if (!this.fileAccessLocks.ContainsKey(filePath))
                    {
                        this.fileAccessLocks[filePath] = new object();
                    }
                }
            }

            return this.fileAccessLocks[filePath];
        }

        public string LocalFolderPath
        {
            get
            {
                return ApplicationData.Current.LocalFolder.Path;
            }
        }

        private void RaiseErrorOccured(Exception ex)
        {
            var handler = this.ErrorOccurred;
            if (handler != null)
            {
                var args = new ErrorOccurredEventArgs(ex);
                handler.Invoke(this, args);
            }
        }
    }
}
