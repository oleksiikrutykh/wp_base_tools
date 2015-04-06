namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represent a file that can be transmitted to server.
    /// </summary>
    public class HttpFile
    {
        public HttpFile()
        {
            this.ContentType = "application/octet-stream";
        }

        public Stream Content { get; set; }

        public string FieldName { get; set; }

        public string Name { get; set; }

        public string ContentType { get; set; }
    }
}
