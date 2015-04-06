namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Parameter of http request.
    /// </summary>
    public class RequestParameter
    {
        public string Key { get; set; }

        public string Value { get; set; }

        public bool NeedEscapeValue { get; set; }
    }
}
