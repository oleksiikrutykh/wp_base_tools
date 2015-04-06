namespace BaseTools.Core.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents info about file.
    /// </summary>
    public interface IFile
    {
        string Name { get; }

        string Path { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Justification="Name is correct")]
        Task<FileProperties> GetPropertiesAsync(); 
    }
}
