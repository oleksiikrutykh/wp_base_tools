namespace BaseTools.Core.Network
{
    using BaseTools.Core.IO;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains logic for decompressing server responses.
    /// </summary>
    internal class ResponseDecompressor
    {
        private const string GZipHeader = "gzip";

        private static readonly ResponseDecompressor instance = new ResponseDecompressor();

        public static ResponseDecompressor Instance
        {
            get
            {
                return instance;
            }
        }

        public Stream TryDecompressWebResponseStream(WebResponse response, Stream responseStream)
        {
            var resultStream = DecompressWebResponseStream(response, responseStream);
            if (resultStream == null)
            {
                resultStream = responseStream;
            }

            return resultStream;
        }

        public Stream DecompressWebResponseStream(WebResponse response, Stream responseStream)
        {
            Stream decompressedStream = null;
            bool isGzipStream = response.Headers.AllKeys.Contains("Content-Encoding") &&
                                response.Headers["Content-Encoding"] == GZipHeader;
            if (isGzipStream)
            {
                if (responseStream.CanSeek)
                {
                    var gzipHeaderBytes = new byte[2];
                    var readedBytesCount = responseStream.Read(gzipHeaderBytes, 0, 2);
                    if (readedBytesCount > 0)
                    {
                        responseStream.Seek(-readedBytesCount, SeekOrigin.Current);
                    }

                    if (gzipHeaderBytes[0] != 31 || gzipHeaderBytes[1] != 139)
                    {
                        isGzipStream = false;
                    }
                }
            }

            if (isGzipStream)
            {
                bool isGzipDecompressionSupported = Factory.Common.IsBindingExist(typeof(GZipStreamProvider));
                if (isGzipDecompressionSupported)
                {
                    var streamProvider = Factory.Common.GetInstance<GZipStreamProvider>();
                    decompressedStream = streamProvider.GetDecompressedStream(responseStream);
                }
            }

            return decompressedStream;
        }

        public bool CanAutoDecompress()
        {
            bool isGzipDecompressionSupported = Factory.Common.IsBindingExist(typeof(GZipStreamProvider));
            return isGzipDecompressionSupported;
        }

    }
}
