namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class RequestContent
    {
        public RequestContent()
        {
            this.Headers = new Dictionary<string, string>();
        }

        public Dictionary<string, string> Headers { get; set; }

        protected abstract void Write(Stream stream);

        public void SaveToStream(Stream responseStream)
        {
            this.Write(responseStream);
        }
    }
}
