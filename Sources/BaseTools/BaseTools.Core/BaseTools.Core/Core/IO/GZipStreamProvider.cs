namespace BaseTools.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Text;

    /// <summary>
    /// Performs gzip compression and decompression 
    /// </summary>
    public abstract class GZipStreamProvider
    {
        public abstract Stream GetCompressedStream(Stream origin);

        public abstract Stream GetDecompressedStream(Stream origin);
    }
}
