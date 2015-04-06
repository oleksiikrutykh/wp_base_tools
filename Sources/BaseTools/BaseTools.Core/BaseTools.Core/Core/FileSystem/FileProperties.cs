namespace BaseTools.Core.FileSystem
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains additional information about file.
    /// </summary>
    public class FileProperties 
    {
        public DateTimeOffset ModifiedDate { get; set; }

        public DateTimeOffset LastAccessDate { get; set; }
    }
}
