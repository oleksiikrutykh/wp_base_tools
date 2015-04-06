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
    /// Represents file in WinRT apps. 
    /// </summary>
    internal class WinRTFile : IFile
    {
        private const string DateAccessedProperty = "System.DateAccessed";

        private const string DateModifiedProperty = "System.DateModified";

        private static readonly List<string> PropertyNames = new List<string> 
        { 
            DateAccessedProperty,
            DateModifiedProperty 
        };

        public string Name { get; set; }

        public string Path { get; set; }

        internal StorageFile AssociatedFile { get; set; }

        public DateTimeOffset CreationDate { get; set; }

        public async Task<FileProperties> GetPropertiesAsync()
        {
            if (this.AssociatedFile == null)
            {
                // TODO: not tested this logic.
                this.AssociatedFile = await StorageFile.GetFileFromPathAsync(this.Path);
            }

            var basicProperties = await this.AssociatedFile.Properties.RetrievePropertiesAsync(PropertyNames);
            var fileProperties = new FileProperties
            {
                ModifiedDate = (DateTimeOffset)basicProperties[DateModifiedProperty],
                LastAccessDate = (DateTimeOffset)basicProperties[DateAccessedProperty],
            };

            return fileProperties;
        }
    }
}
