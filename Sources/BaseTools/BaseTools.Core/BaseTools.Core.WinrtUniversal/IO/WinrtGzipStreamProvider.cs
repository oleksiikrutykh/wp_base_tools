namespace BaseTools.Core.IO
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.IO.Compression;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Performs gzip compression and decompression
    /// </summary>
    internal class WinrtGzipStreamProvider : GZipStreamProvider
    {
        public override System.IO.Stream GetCompressedStream(Stream originStream)
        {
            return new GZipStream(originStream, CompressionMode.Compress);
        }

        public override System.IO.Stream GetDecompressedStream(Stream originStream)
        {
            return new GZipStream(originStream, CompressionMode.Decompress);
        }
    }
}
