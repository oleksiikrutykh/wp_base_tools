namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Data passed to server via http request.
    /// </summary>
    public class HttpMessage
    {
        public HttpMessage()
        {
            this.Parameters = new ParametersCollection();
            this.PostContent = new List<object>();
            this.Files = new List<HttpFile>();
            this.Cookies = new CookieCollection();
            this.Headers = new Dictionary<string, string>();
        }

        public string FullRequestPath { get; set; }

        public ParametersCollection Parameters { get; set; }

        // TODO: refactor post content.
        public List<object> PostContent { get; set; }

        public RequestContent RequestContent { get; set; }

        public Dictionary<string, string> Headers { get; set; }

        public List<HttpFile> Files { get; set; }

        public HttpMethod Method { get; set; }

        public CookieCollection Cookies { get; set; }
    }
}
