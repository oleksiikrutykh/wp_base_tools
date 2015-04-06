namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Parse file path string.
    /// </summary>
    public class FileSystemPath
    {
        private static readonly char[] FilePathSeparators = new char[2] { '/', '\\' };

        public StorageType StorageType { get; private set; }

        public string FormattedPath { get; private set; }

        public string[] FolderTree { get; private set; }

        public string ItemName { get; private set; }

        public FileSystemPath(string path)
        {
            Guard.CheckIsNotNull(path, "path");
            path = this.FormatPath(path);
            var pathParts = path.Split(FilePathSeparators, StringSplitOptions.RemoveEmptyEntries);
            var folderStackLength = pathParts.Length - 1;
            var folderTree = new string[folderStackLength];
            for (int i = 0; i < folderStackLength; i++)
            {
                folderTree[i] = pathParts[i];
            }

            this.FormattedPath = String.Join("\\", pathParts);
            this.FolderTree = folderTree;
            this.ItemName = pathParts[pathParts.Length - 1];
        }

        public FileSystemPath(FileSystemPath root, string childName)
        {
            // TODO: implement relative path.
            this.StorageType = root.StorageType;
            this.FormattedPath = root.FormattedPath + "/" + childName;
            this.FolderTree = new string[root.FolderTree.Length + 1];
            Array.Copy(root.FolderTree, this.FolderTree, root.FolderTree.Length);
            this.FolderTree[this.FolderTree.Length - 1] = root.ItemName;
            this.ItemName = childName;
        }

        private string FormatPath(string inputPath)
        {
            this.StorageType = StorageType.Local;
            string resultPath = inputPath;
            var storage = RootStorage.DetermineStorageByFileName(inputPath);
            if (storage != null)
            {
                this.StorageType = storage.Type;
                resultPath = storage.DetermineRelativePath(inputPath);
            }

            this.FormattedPath = resultPath;
            return resultPath;
        }
    }
}
