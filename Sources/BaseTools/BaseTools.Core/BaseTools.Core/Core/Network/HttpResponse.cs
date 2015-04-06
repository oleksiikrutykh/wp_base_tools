namespace BaseTools.Core.Network
{
    using BaseTools.Core.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

    /// <summary>
    /// Http response from server.
    /// </summary>
    public sealed class HttpResponse : IDisposable
    {
        private IDisposable[] additionalItems;

        public HttpResponse(Stream responseStream, params IDisposable[] additionalDisposeItems)
        {
            this.ResponseStream = responseStream;
            this.additionalItems = additionalDisposeItems;
        }

        public Stream ResponseStream { get; set; }

        public HttpStatusCode StatusCode { get; set; }

        public void Dispose()
        {
            DisposableHelper.DisposeSafe(this.ResponseStream);
            foreach (var item in this.additionalItems)
            {
                item.Dispose();
            }
        }
    }
}
