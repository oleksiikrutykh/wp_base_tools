namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Storage;

    /// <summary>
    /// Contains information about storage type (local, remote, app package).
    /// </summary>
    internal class RootStorage
    {
        private const string LocalFolderPrefix = "ms-appdata:///Local/";

        private const string PackageFolderPrefix = "ms-appx:///";

        private const string RoamingFolderPrefix = "ms-appdata:///Roaming/";

        private static readonly RootStorage LocalStorage = new RootStorage(StorageType.Local);

        private static readonly RootStorage PackageStorage = new RootStorage(StorageType.Package);

        private static readonly RootStorage RoamingStorage = new RootStorage(StorageType.Roaming);

        private static readonly List<RootStorage> availableStorages = new List<RootStorage>
        {
            LocalStorage,
            PackageStorage,
            RoamingStorage
        };

        private StorageType storageType;

        private StorageFolder rootFolder;

        private string folderPath;

        public RootStorage(StorageType type)
        {
            this.storageType = type;
            switch (this.storageType)
            {
                case StorageType.Local:
                    this.rootFolder = ApplicationData.Current.LocalFolder;
                    this.folderPath = LocalFolderPrefix;
                    break;

                case StorageType.Package:
                    this.rootFolder = Package.Current.InstalledLocation;
                    this.folderPath = PackageFolderPrefix;
                    break;

                case StorageType.Roaming:
#if WINRT
                    this.rootFolder = ApplicationData.Current.RoamingFolder;
#endif

#if SILVERLIGHT
                    this.rootFolder = ApplicationData.Current.LocalFolder;
#endif
                    this.folderPath = RoamingFolderPrefix;
                    break;
            }
        }

        public StorageFolder RootFolder
        {
            get
            {
                return this.rootFolder;
            }
        }

        public StorageType Type
        {
            get
            {
                return this.storageType;
            }
        }

        public string FolderPath
        {
            get
            {
                return this.folderPath;
            }
        }

        public static RootStorage GetStorage(StorageType type)
        {
            RootStorage storage = null;
            switch (type)
            {
                case StorageType.Local:
                    storage = LocalStorage;
                    break;

                case StorageType.Package:
                    storage = PackageStorage;
                    break;

                case StorageType.Roaming:
                    storage = RoamingStorage;
                    break;
            }

            return storage;
        }

        public static RootStorage DetermineStorageByFileName(string filePath)
        {
            RootStorage relatedStorage = null;
            foreach (var storage in availableStorages)
            {
                var hasPrefix = filePath.StartsWith(storage.FolderPath, StringComparison.OrdinalIgnoreCase);
                if (hasPrefix)
                {
                    relatedStorage = storage;
                    break;
                }
            }

            return relatedStorage;
        }

        public string DetermineRelativePath(string absolutePath)
        {
            var relativePath = absolutePath.Remove(0, this.folderPath.Length);
            return relativePath;
        }
    }
}
