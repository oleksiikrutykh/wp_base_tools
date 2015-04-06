namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.Threading;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents base logic for accessing file system.
    /// </summary>
    public abstract class FileSystemBaseProvider : IFileSystemProvider
    {
        private static readonly object FileLocksStorageSyncronizer = new object();

        private Dictionary<string, AsyncLock> allFilesLocks = new Dictionary<string, AsyncLock>();

        public Task<Stream> OpenFileAsync(string filePath)
        {
            return this.OpenFileAsync(filePath, FileOpeningMode.Open);
        }

        public abstract Task<Stream> OpenFileAsync(string filePath, FileOpeningMode mode);

        public abstract Task<bool> DeleteFileAsync(string filePath);


        public Task CreateDirectoryAsync(string directoryPath)
        {
            return this.CreateDirectoryAsync(directoryPath, false);
        }

        public abstract Task CreateDirectoryAsync(string directoryPath, bool errorIfExist);

        public abstract Task<bool> DeleteDirectoryAsync(string directoryPath);

        public abstract Task CopyAsync(string fromPath, string toPath);

        public abstract Task<bool> IsFileExist(string filePath);

        public abstract Task<bool> IsDirectoryExist(string directoryPath);

        public abstract Task CopyDirectoryAsync(string fromPath, string toPath, bool needReplaceExisted);

        public abstract Task<List<IFile>> FindFilesAsync(string searchPattern);

        protected AsyncLock GetLockForFile(string filePath)
        {
            AsyncLock fileLock = null;
            var isExist = this.allFilesLocks.TryGetValue(filePath, out fileLock);
            if (!isExist)
            {
                lock (FileLocksStorageSyncronizer)
                {
                    var isExistInLock = this.allFilesLocks.TryGetValue(filePath, out fileLock);
                    if (!isExistInLock)
                    {
                        fileLock = new AsyncLock();
                        this.allFilesLocks.Add(filePath, fileLock);
                    }
                }
            }

            return fileLock;
        }


        public abstract string GetFullFilePath(string filePath);
        
    }
}
