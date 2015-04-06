namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Storage;
    using Windows.Storage.Search;
    using Windows.Storage.Streams;

    /// <summary>
    /// Allows to access file system for WinRT apps. 
    /// </summary>
    internal class FileSystemProvider : FileSystemBaseProvider
    {
        private const int StreamBufferSize = 4096;

        public override async Task<Stream> OpenFileAsync(string filePath, FileOpeningMode mode)
        {
            Stream syncronizedStream = null;
            var randomAccessStream = (RandomAccessSynchronizedStream)await this.OpenFileAsRandomAccessAsync(filePath, mode);
            if (randomAccessStream != null)
            {
                try
                {
                    var convertedStream = randomAccessStream.InternalStream.AsStream(StreamBufferSize);
                    syncronizedStream = new SynchronizedStream(convertedStream, randomAccessStream.AccessLocker);
                }
                catch
                {
                    if (randomAccessStream != null)
                    {
                        randomAccessStream.Dispose();
                    }

                    throw;
                }
            }

            return syncronizedStream;
        }

        public async Task<IRandomAccessStream> OpenFileAsRandomAccessAsync(string filePath, FileOpeningMode mode)
        {
            var parsedPath = new FileSystemPath(filePath);
            var needThrowFileNotFoundException = true;
            if (mode == FileOpeningMode.OpenIfExist)
            {
                mode = FileOpeningMode.Open;
                needThrowFileNotFoundException = false;
            }

            bool isCreateOperation = true;
            CreationCollisionOption collisionOption = CreationCollisionOption.FailIfExists;
            switch (mode)
            {
                case FileOpeningMode.CreateNew:
                    collisionOption = CreationCollisionOption.FailIfExists;
                    break;

                case FileOpeningMode.Create:
                    collisionOption = CreationCollisionOption.ReplaceExisting;
                    break;

                case FileOpeningMode.OpenOrCreate:
                    collisionOption = CreationCollisionOption.OpenIfExists;
                    break;

                case FileOpeningMode.Append:
                    collisionOption = CreationCollisionOption.OpenIfExists;
                    break;

                case FileOpeningMode.Open:
                case FileOpeningMode.Truncate:
                    isCreateOperation = false;
                    break;
            }

            // Can't create files into package.
            if (parsedPath.StorageType != StorageType.Local)
            {
                isCreateOperation = false;
            }

            var fileLock = this.GetLocker(parsedPath);
            var releaser = await fileLock.LockAsync();
            IRandomAccessStream stream = null;
            try
            {
                var fileLocation = await this.OpenLocation(parsedPath);
                StorageFile file = null;
                if (isCreateOperation)
                {
                    file = await fileLocation.Folder.CreateFileAsync(fileLocation.ItemName, collisionOption);
                }
                else
                {
                    file = await fileLocation.Folder.GetFileAsync(fileLocation.ItemName);
                }

                var fileAccessMode = FileAccessMode.ReadWrite;
                if (parsedPath.StorageType == StorageType.Package)
                {
                    fileAccessMode = FileAccessMode.Read;
                }

                stream = await file.OpenAsync(fileAccessMode);
                if (mode == FileOpeningMode.Append)
                {
                    stream.Seek(stream.Size - 1);
                }

                if (mode == FileOpeningMode.Truncate)
                {
                    stream.Seek(0);
                }

                stream = new RandomAccessSynchronizedStream(stream, releaser);

            }
            catch (FileNotFoundException)
            {
                DisposableHelper.DisposeSafe(stream);
                DisposableHelper.DisposeSafe(releaser);
                if (needThrowFileNotFoundException)
                {
                    throw;
                }
            }
            catch
            {
                DisposableHelper.DisposeSafe(stream);
                DisposableHelper.DisposeSafe(releaser);
                throw;
            }

            return stream;
        }

        public override async Task<bool> DeleteFileAsync(string filePath)
        {
            var parsedPath = new FileSystemPath(filePath);
            CheckReadonlyAccess(parsedPath);
            bool isExisted = false;
            var fileLock = this.GetLocker(parsedPath);
            using (await fileLock.LockAsync())
            {
                StorageFile file = null;
                try
                {
                    var fileLocation = await this.OpenLocation(parsedPath);
                    file = await fileLocation.Folder.GetFileAsync(fileLocation.ItemName);
                }
                catch (FileNotFoundException)
                { 
                    // Ignore file not found exception. Allow remove files without exceptions.
                }

                if (file != null)
                {
                    await file.DeleteAsync();
                    isExisted = true;
                }
            }

            return isExisted;
        }

        public override async Task CreateDirectoryAsync(string directoryPath, bool errorIfExist)
        {
            var parsedPath = new FileSystemPath(directoryPath);
            CheckReadonlyAccess(parsedPath);
            var collision = CreationCollisionOption.OpenIfExists;
            if (errorIfExist)
            {
                collision = CreationCollisionOption.FailIfExists;
            }


            var directoryLocation = await this.OpenLocation(parsedPath);
            var folder = await directoryLocation.Folder.CreateFolderAsync(directoryLocation.ItemName, collision);

        }

        public override async Task<bool> DeleteDirectoryAsync(string directoryPath)
        {
            bool isDirectoryExist = true;
            var parsedPath = new FileSystemPath(directoryPath);
            parsedPath = new FileSystemPath(parsedPath, "*");
            CheckReadonlyAccess(parsedPath);
            try
            {
                var directoryLocation = await this.OpenLocation(parsedPath);
                await directoryLocation.Folder.DeleteAsync();
            }
            catch (FileNotFoundException ex)
            {
                isDirectoryExist = false;
            }

            return isDirectoryExist;
        }

        public override async Task CopyAsync(string fromPath, string toPath)
        {
            var parsedPathFrom = new FileSystemPath(fromPath);
            var parsedPathTo = new FileSystemPath(toPath);
            CheckReadonlyAccess(parsedPathTo);
            if (parsedPathFrom.FormattedPath != parsedPathTo.FormattedPath || parsedPathFrom.StorageType != parsedPathTo.StorageType)
            {
                var fromFileLock = this.GetLocker(parsedPathFrom);
                using (await fromFileLock.LockAsync())
                {
                    var toFileLock = this.GetLocker(parsedPathTo);
                    using (await toFileLock.LockAsync())
                    {
                        var fromFileLocation = await this.OpenLocation(parsedPathFrom);
                        var toFileLocation = await this.OpenLocation(parsedPathTo);
                        var fromFile = await fromFileLocation.Folder.GetFileAsync(fromFileLocation.ItemName);
                        var toFile = await toFileLocation.Folder.CreateFileAsync(toFileLocation.ItemName, CreationCollisionOption.ReplaceExisting);
                        await fromFile.CopyAndReplaceAsync(toFile);
                    }
                }
            }
        }

        public override async Task<bool> IsFileExist(string filePath)
        {
            bool isFileExist = false;
            var parsedPath = new FileSystemPath(filePath);
            var fileLock = this.GetLocker(parsedPath);
            using (await fileLock.LockAsync())
            {
                try
                {
                    var fileLocation = await this.OpenLocation(parsedPath);
                    var file = await fileLocation.Folder.GetFileAsync(parsedPath.ItemName);
                    isFileExist = file != null;
                }
                catch (FileNotFoundException)
                {
                }
            }

            return isFileExist;
        }

        public override async Task<bool> IsDirectoryExist(string directoryPath)
        {
            bool isDirectoryExist = false;
            var parsedPath = new FileSystemPath(directoryPath);
            try
            {
                var directoryLocation = await this.OpenLocation(parsedPath);
                var directory = await directoryLocation.Folder.GetFolderAsync(directoryLocation.ItemName);
                isDirectoryExist = directory != null;
            }
            catch (FileNotFoundException)
            {
            }

            return isDirectoryExist;
        }

        public override async Task CopyDirectoryAsync(string fromPath, string toPath, bool needReplaceExisted)
        {
            var parsedFromPath = new FileSystemPath(fromPath);
            var fromFolderLocation = await this.OpenLocation(parsedFromPath);
            var fromFolder = await fromFolderLocation.Folder.GetFolderAsync(fromFolderLocation.ItemName);
            var parsedToPath = new FileSystemPath(toPath);
            CheckReadonlyAccess(parsedToPath);
            var toFolderLocation = await this.OpenLocation(parsedToPath);

            var folderCollision = CreationCollisionOption.OpenIfExists;
            if (needReplaceExisted)
            {
                folderCollision = CreationCollisionOption.ReplaceExisting;
            }

            var toFolder = await toFolderLocation.Folder.CreateFolderAsync(toFolderLocation.ItemName, folderCollision);
            fromFolderLocation = new FileSystemLocation(parsedFromPath, fromFolder, String.Empty);
            toFolderLocation = new FileSystemLocation(parsedToPath, toFolder, String.Empty);
            await this.CopyFolderChilds(fromFolderLocation, toFolderLocation, folderCollision);
        }

        private async Task CopyFolderChilds(FileSystemLocation fromFolder, FileSystemLocation toFolder, CreationCollisionOption copyCollision)
        {
            await this.CopyFiles(fromFolder, toFolder);
            var allChildFolders = await fromFolder.Folder.GetFoldersAsync();
            foreach (var childFromFolder in allChildFolders)
            {
                var childFromFolderPath = new FileSystemPath(fromFolder.Path, childFromFolder.Name);
                var childToFolder = await toFolder.Folder.CreateFolderAsync(childFromFolder.Name, copyCollision);
                var childToFolderPath = new FileSystemPath(toFolder.Path, childFromFolder.Name);

                var childFromLocation = new FileSystemLocation(childFromFolderPath, childFromFolder, String.Empty);
                var childToLocation = new FileSystemLocation(childToFolderPath, childToFolder, String.Empty);
                await this.CopyFolderChilds(childFromLocation, childToLocation, copyCollision);
            }
        }

        private async Task CopyFiles(FileSystemLocation fromFolder, FileSystemLocation toFolder)
        {
            var allFiles = await fromFolder.Folder.GetFilesAsync();
            foreach (var childFile in allFiles)
            {
                var fromFolderPath = new FileSystemPath(fromFolder.Path, childFile.Name);
                var fromFileLock = this.GetLocker(fromFolderPath);
                using (await fromFileLock.LockAsync())
                {
                    var fileToPath = new FileSystemPath(toFolder.Path, childFile.Name);
                    var toFileLock = this.GetLocker(fileToPath);
                    using (await toFileLock.LockAsync())
                    {
                        await childFile.CopyAsync(toFolder.Folder, childFile.Name, NameCollisionOption.ReplaceExisting);
                    }
                }
            }
        }

        public override async Task<List<IFile>> FindFilesAsync(string searchPattern)
        {
            var filePath = new FileSystemPath(searchPattern);
            var storage = RootStorage.GetStorage(filePath.StorageType);
            var location = await this.OpenLocation(filePath);

            IEnumerable<StorageFile> foundedFiles = null;
#if SILVERLIGHT_APP
            //TODO: select matched files with regex.
            foundedFiles = await location.Folder.GetFilesAsync();
#endif
            
#if WINDOWS_APP
            var options = new QueryOptions();
            options.FolderDepth = FolderDepth.Shallow;
            options.UserSearchFilter = filePath.ItemName;
            var query = location.Folder.CreateFileQueryWithOptions(options);
            foundedFiles = await query.GetFilesAsync();
#endif

            var result = new List<IFile>();
            foreach (var file in foundedFiles)
            {
                var realRootPath = storage.RootFolder.Path;
                var realFilePath = file.Path;
                var relativePath = realFilePath.Remove(0, realRootPath.Length);
                var path = storage.FolderPath + relativePath.TrimStart('\\');
                var fileInfo = new WinRTFile
                {
                    Name = file.Name,
                    Path = path,
                    CreationDate = file.DateCreated,
                    AssociatedFile = file
                };

                result.Add(fileInfo);
            }

            return result;
        }

        internal async Task<FileSystemLocation> OpenLocation(FileSystemPath path)
        {
            var storage = RootStorage.GetStorage(path.StorageType);
            var folder = storage.RootFolder;
            foreach (var folderName in path.FolderTree)
            {
                folder = await folder.GetFolderAsync(folderName);
            }

            var location = new FileSystemLocation(path, folder, path.ItemName);
            return location;
        }

        private AsyncLock GetLocker(FileSystemPath filePath)
        {
            var asyncItemKey = filePath.StorageType + filePath.FormattedPath;
            return this.GetLockForFile(asyncItemKey);
        }

        private static void CheckReadonlyAccess(FileSystemPath path)
        {
            if (path.StorageType == StorageType.Package)
            {
                throw new InvalidOperationException("Can't change read only file");
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification="Called externally")]
        internal async Task<IStorageItem> GetStorageItem(string path)
        {
            var filePath = new FileSystemPath(path);
            var location = await this.OpenLocation(filePath);
            var storageItem = await location.Folder.GetItemAsync(location.ItemName);
            return storageItem;
        }

        public override string GetFullFilePath(string filePath)
        {
            var formattedPath = new FileSystemPath(filePath);
            var storage = RootStorage.GetStorage(formattedPath.StorageType);
            var fullPath = storage.RootFolder.Path + "\\" + formattedPath.FormattedPath;
            return fullPath;
        }
    }
}
