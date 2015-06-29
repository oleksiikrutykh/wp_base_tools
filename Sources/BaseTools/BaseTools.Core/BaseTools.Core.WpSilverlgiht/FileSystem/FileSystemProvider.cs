namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Info;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage;

    /// <summary>
    /// Allows to access file system for Windows Phone Silverlight apps. 
    /// </summary>
    internal class FileSystemProvider : FileSystemBaseProvider
    {
        private const string LocalFolderPrefix = "ms-appdata:///Local/";

        private IsolatedStorageFile store;

        private bool isDesignMode;

        public FileSystemProvider()
        {
            isDesignMode = EnvironmentInfo.Current.IsInDesignMode;
            if (!this.isDesignMode)
            {
                store = IsolatedStorageFile.GetUserStoreForApplication();
            }
        }

        public string InitialFolder { get; set; }

        public override Task<System.IO.Stream> OpenFileAsync(string filePath, FileOpeningMode mode)
        {
            return Task.Run(async () =>
            {
                if (this.isDesignMode)
                {
                    return new MemoryStream();
                }

                bool mustFailOnFileNotFound = true;
                if (mode == FileOpeningMode.OpenIfExist)
                {
                    mode = FileOpeningMode.Open;
                    mustFailOnFileNotFound = false;
                }

                var systemFileMode = FileModeConverter.ConvertToSystem(mode);
                filePath = FormatPath(filePath);

                var fileLock = this.GetLockForFile(filePath);
                var releaser = await fileLock.LockAsync();

                bool needOpenFile = true;
                if (!mustFailOnFileNotFound)
                {
                    var isFileExist = this.store.FileExists(filePath);
                    if (!isFileExist)
                    {
                        needOpenFile = false;
                    }
                }

                System.IO.Stream resultStream = null;
                if (needOpenFile)
                {
                    try
                    {
                        resultStream = this.store.OpenFile(filePath, systemFileMode);
                        resultStream = new SynchronizedStream(resultStream, releaser);
                    }
                    catch
                    {
                        DisposableHelper.DisposeSafe(resultStream);
                        releaser.Dispose();
                        throw;
                    }
                }
                else
                {
                    releaser.Dispose();
                }

                return resultStream;
            });
        }

        public override Task<bool> DeleteFileAsync(string filePath)
        {
            return Task.Run(async () =>
            {
                if (this.isDesignMode)
                {
                    return false;
                }

                filePath = FormatPath(filePath);
                bool isFileExist = false;
                var fileLock = GetLockForFile(filePath);
                using (var releaser = await fileLock.LockAsync())
                {
                    isFileExist = this.store.FileExists(filePath);
                    if (isFileExist)
                    {
                        this.store.DeleteFile(filePath);
                    }
                }

                return isFileExist;
            });
        }

        public override Task CreateDirectoryAsync(string directoryPath, bool errorIfExist)
        {
            return Task.Run(() =>
            {
                if (this.isDesignMode)
                {
                    return;
                }

                directoryPath = FormatPath(directoryPath);
                if (!errorIfExist)
                {
                    // Not create directory if it already exist.
                    bool isExist = this.store.DirectoryExists(directoryPath);
                    if (!isExist)
                    {
                        this.store.CreateDirectory(directoryPath);
                    }
                }
                else
                {
                    // Create directory. An exeption was thrown if directory not exist.
                    this.store.CreateDirectory(directoryPath);
                }
            });
        }

        public override Task<bool> IsFileExist(string filePath)
        {
            return Task.Run(async() =>
            {
                if (this.isDesignMode)
                {
                    return false;
                }

                filePath = FormatPath(filePath);
                bool isFileExist = false;

                var fileLock = GetLockForFile(filePath);
                using (var releaser = await fileLock.LockAsync())
                {
                    isFileExist = this.store.FileExists(filePath);
                }

                return isFileExist;
            });
        }


        public override Task<bool> IsDirectoryExist(string directoryPath)
        {
            return Task.Run(() =>
            {
                if (this.isDesignMode)
                {
                    return false;
                }

                directoryPath = FormatPath(directoryPath);
                return this.store.DirectoryExists(directoryPath);
            });
        }

        public override Task<bool> DeleteDirectoryAsync(string directoryPath)
        {
            if (this.isDesignMode)
            {
                return Task.FromResult(false);
            }

            directoryPath = FormatPath(directoryPath);
            this.store.DeleteDirectory(directoryPath);
            return Task.FromResult(true);
        }

        public override Task CopyAsync(string fromPath, string toPath)
        {
            return Task.Run(async() =>
            {
                if (this.isDesignMode)
                {
                    return;
                }

                fromPath = FormatPath(fromPath);
                toPath = FormatPath(toPath);
                if (fromPath != toPath)
                {
                    var fromFileLock = GetLockForFile(fromPath);
                    using (await fromFileLock.LockAsync())
                    {
                        var toFileLock = GetLockForFile(toPath);
                        using (await toFileLock.LockAsync())
                        {
                            this.store.CopyFile(fromPath, toPath, true);
                        }
                    }
                }
            });
        }

        public override Task CopyDirectoryAsync(string fromPath, string toPath, bool needReplaceExisted)
        {
            throw new NotImplementedException();
        }

        protected static string FormatPath(string inputPath)
        {
            var resultPath = inputPath;
            if (!string.IsNullOrEmpty(inputPath))
            {
                var hasLocalPrefix = inputPath.StartsWith(LocalFolderPrefix, StringComparison.Ordinal);
                if (hasLocalPrefix)
                {
                    resultPath = inputPath.Substring(LocalFolderPrefix.Length);
                }
            }
            return resultPath;
        }

        public override Task<List<IFile>> FindFilesAsync(string searchPattern)
        {
            return Task.Run(() =>
            {
                if (this.isDesignMode)
                {
                    return new List<IFile>();
                }

                searchPattern = FormatPath(searchPattern);
                var searchPatternPath = new FileSystemPath(searchPattern);
                var storage = RootStorage.GetStorage(searchPatternPath.StorageType);

                var foundedFileNames = this.store.GetFileNames(searchPattern);
                var files = new List<IFile>();
                if (foundedFileNames.Length > 0)
                {
                    var folderPath = String.Join("/", searchPatternPath.FolderTree);
                    foreach (var name in foundedFileNames)
                    {
                        string formattedFilePath = name;
                        if (!String.IsNullOrEmpty(folderPath))
                        {
                            formattedFilePath = folderPath + "/" + name;
                        }

                        var file = new WpFile
                        {
                            Name = name,
                            Path = storage.FolderPath + formattedFilePath,
                            InternalAccessPath = formattedFilePath,
                            Store = this.store
                        };

                        files.Add(file);
                    }
                }

                return files;
            });
        }

        public override string GetFullFilePath(string filePath)
        {
            var formattedPath = new FileSystemPath(filePath);
            var storage = RootStorage.GetStorage(formattedPath.StorageType);
            var fullPath = storage.RootFolder.Path + "\\" + formattedPath.FormattedPath;
            return fullPath;
        }
    }

    //public class FileContainer
    //{
    //    public abstract Task<System.IO.Stream> OpenFileAsync(string filePath, FileMode mode);
    //}

    //public class IsolatedStorageContainer : FileContainer 
    //{
    //    public override Task<System.IO.Stream> OpenFileAsync(string filePath, FileMode mode)
    //    {
    //        return Task.Run(async () =>
    //        {
    //            bool mustFailOnFileNotFound = true;
    //            if (mode == FileMode.OpenIfExist)
    //            {
    //                mode = FileMode.Open;
    //                mustFailOnFileNotFound = false;
    //            }

    //            var systemFileMode = FileModeConverter.ConvertToSystem(mode);
    //            filePath = FormatPath(filePath);

    //            var fileLock = this.GetLockForFile(filePath);
    //            var releaser = await fileLock.LockAsync();

    //            bool needOpenFile = true;
    //            if (!mustFailOnFileNotFound)
    //            {
    //                var isFileExist = this.store.FileExists(filePath);
    //                if (!isFileExist)
    //                {
    //                    needOpenFile = false;
    //                }
    //            }

    //            System.IO.Stream resultStream = null;
    //            if (needOpenFile)
    //            {
    //                try
    //                {
    //                    resultStream = this.store.OpenFile(filePath, systemFileMode);
    //                    resultStream = new SynchronizedStream(resultStream, releaser);
    //                }
    //                catch
    //                {
    //                    DisposableHelper.DisposeSafe(resultStream);
    //                    releaser.Dispose();
    //                    throw;
    //                }
    //            }
    //            else
    //            {
    //                releaser.Dispose();
    //            }

    //            return resultStream;
    //        });
    //    }
    //}

}
