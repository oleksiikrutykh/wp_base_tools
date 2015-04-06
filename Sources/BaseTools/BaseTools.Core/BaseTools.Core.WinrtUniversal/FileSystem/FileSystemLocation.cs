namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Storage;

    /// <summary>
    /// Info about location of file system item.
    /// </summary>
    internal class FileSystemLocation
    {
        public FileSystemLocation(FileSystemPath path, StorageFolder folder, string itemName)
        {
            this.Path = path;
            this.Folder = folder;
            this.ItemName = itemName;
        }

        public FileSystemPath Path { get; private set; }

        public StorageFolder Folder { get; private set; }

        public string ItemName { get; private set; }
    }
}
