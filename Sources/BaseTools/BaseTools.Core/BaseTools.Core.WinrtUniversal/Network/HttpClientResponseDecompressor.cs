namespace BaseTools.Core.Network
{
    using BaseTools.Core.IO;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Perform decompression of http responses.
    /// </summary>
    internal class HttpClientResponseDecompressor
    {
        private const string GZipHeader = "gzip";

        private static HttpClientResponseDecompressor instance = new HttpClientResponseDecompressor();

        public static HttpClientResponseDecompressor Instance
        {
            get
            {
                return instance;
            }
        }

        public Stream TryDecompressStream(Stream originStream, HttpResponseMessage message)
        {
            Stream decompressedStream = originStream;
            try
            {
                IEnumerable<string> contentEncodingHeaders;
                var isHasContentEncoding = message.Content.Headers.TryGetValues("Content-Encoding", out contentEncodingHeaders);
                if (isHasContentEncoding && contentEncodingHeaders.Contains(GZipHeader))
                {
                    var streamProvider = Factory.Common.GetInstance<GZipStreamProvider>();
                    decompressedStream = streamProvider.GetDecompressedStream(originStream);
                }
            }
            catch
            {
                if (decompressedStream != null && decompressedStream != originStream)
                {
                    decompressedStream.Dispose();
                }

                throw;
            }

            return decompressedStream;
        }
    }
}
