namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    public class NetworkError
    {
        public HttpStatusCode StatusCode { get; set; }

        public object Data { get; set; }
    }
}
