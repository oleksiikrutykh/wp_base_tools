namespace BaseTools.Core.FileSystem
{
    using System.Collections.Generic;
    using System.IO;
    using System.Threading.Tasks;

    /// <summary>
    /// Provide access to file system.
    /// </summary>
    public interface IFileSystemProvider
    {
        Task<Stream> OpenFileAsync(string filePath);

        Task<Stream> OpenFileAsync(string filePath, FileOpeningMode mode);

        Task<bool> DeleteFileAsync(string filePath);

        Task CreateDirectoryAsync(string directoryPath);

        Task CreateDirectoryAsync(string directoryPath, bool errorIfExist);

        Task<bool> DeleteDirectoryAsync(string directoryPath);

        Task CopyAsync(string fromPath, string toPath);

        Task CopyDirectoryAsync(string fromPath, string toPath, bool needReplaceExisted);

        Task<bool> IsFileExist(string filePath);

        Task<bool> IsDirectoryExist(string directoryPath);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures", Justification="Return Task")]
        Task<List<IFile>> FindFilesAsync(string searchPattern);

        string GetFullFilePath(string filePath);
    }
}
