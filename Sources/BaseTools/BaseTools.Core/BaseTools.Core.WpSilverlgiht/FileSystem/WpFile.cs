namespace BaseTools.Core.FileSystem
{
    using BaseTools.Core.FileSystem;
    using System;
    using System.Collections.Generic;
    using System.IO.IsolatedStorage;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents file in Windows Phone Silverlight apps. 
    /// </summary>
    internal class WpFile : IFile
    {
        public string Name { get; set; }

        public string Path { get; set; }

        internal IsolatedStorageFile Store { get; set; }

        internal string InternalAccessPath { get; set; }

        public Task<FileProperties> GetPropertiesAsync()
        {
            return Task.Run(() =>
            {
                var fileProperties = new FileProperties
                {
                    ModifiedDate = this.Store.GetLastWriteTime(this.InternalAccessPath),
                    LastAccessDate = this.Store.GetLastAccessTime(this.InternalAccessPath),
                };

                return fileProperties;
            });
        }
    }
}
