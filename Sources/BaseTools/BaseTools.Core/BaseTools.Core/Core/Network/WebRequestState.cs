namespace BaseTools.Core.Network
{
    using BaseTools.Core.DataAccess;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Information about performed http request.
    /// </summary>
    internal class WebRequestState
    {
        public WebRequestState()
        {
            this.TaskSource = new TaskCompletionSource<DataResponse<HttpResponse>>();
        }

        public TaskCompletionSource<DataResponse<HttpResponse>> TaskSource { get; set; }

        public HttpWebRequest Request { get; set; }

        public string Boundary { get; set; }

        public HttpMessage Message { get; set; }
    }
}
