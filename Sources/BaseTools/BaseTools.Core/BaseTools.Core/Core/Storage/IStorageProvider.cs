namespace BaseTools.Core.Storage
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Runtime.Serialization.Json;
    using System.Threading.Tasks;
    using BaseTools.Core.Serialization;
    using BaseTools.Core.Models;

    /// <summary>
    /// Provides logic for storing data in file system.
    /// </summary>
    public interface IStorageProvider
    {
        /// <summary>
        /// Gets or sets serializer used for storing data into file.
        /// </summary>
        ISerializer Serializer { get; set; }

        bool UseContentValidation { get; set; }

        bool NeedSuppressErrors { get; set; }

        event EventHandler<ErrorOccurredEventArgs> ErrorOccurred;

        /// <summary>
        /// Read object from IsolatedStorageSettings collection
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="key">The key of value to get.</param>
        /// <returns>The value associated with the specified key. If there is no value with specified key, method return default(T).</returns>
        T ReadFromSettings<T>(string key);

        /// <summary>
        /// Write object to IsolatedStorageSettings collection.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="key">The key for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        void WriteToSettings<T>(string key, T value);

        /// <summary>
        /// Deleted value from settings.
        /// </summary>
        /// <param name="key">The key of deleted object</param>
        /// <returns>Value indicates whether key was founded.</returns>
        bool DeleteFromSetting(string key);

        /// <summary>
        /// Read object from IsolatedStorageFile. Use json deserialization.
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="fileName">The fileName of value to get.</param>
        /// <returns>An asyncronous operation, returned object stored in specified file. If there is no such file, method default(T) will returned.</returns>
        Task<T> ReadFromFileAsync<T>(string fileName);

        /// <summary>
        /// Read object from IsolatedStorageFile. Use your deserialization.
        /// </summary>
        /// <typeparam name="T">Type of readed object.</typeparam>
        /// <param name="fileName">The fileName of value to get.</param>
        /// <param name="serializer">Serializer used for storing data into file.</param>
        /// <returns>An asyncronous operation, returned object stored in specified file. If there is no such file, method default(T) will returned.</returns>        
        Task<T> ReadFromFileAsync<T>(string fileName, ISerializer serializer);

        /// <summary>
        /// Write object to IsolatedStorageFile. Use json Serialization.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="fileName">The fileName for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        /// <returns>An asyncronous operation ended when write to file completed.</returns>
        Task WriteToFileAsync<T>(string fileName, T value);

        /// <summary>
        /// Write object to IsolatedStorageFile. Use your Serialization.
        /// </summary>
        /// <typeparam name="T">Type of stored object.</typeparam>
        /// <param name="fileName">The fileName for the entry to be stored.</param>
        /// <param name="value">The entry to be stored.</param>
        /// /// <param name="serializer">Serializer used for storing data into file.</param>
        /// <returns>An asyncronous operation ended when write to file completed.</returns>        
        Task WriteToFileAsync<T>(string fileName, T value, ISerializer serializer);

        /// <summary>
        /// Deleted the value, stored in file.
        /// </summary>
        /// <param name="fileName">Name of the deleted file.</param>
        /// <returns>An asyncronous operation returns value indicates whether file was founded.</returns>
        Task<bool> DeleteFromFileAsync(string fileName);

        void SuspendStorage();

        void ActivateStorage();
    }
}
