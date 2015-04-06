namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents basic logic for fetching data from different sources.
    /// </summary>
    /// <typeparam name="TResponse">The type of fetched data.</typeparam>
    public abstract class DataRequest<TResponse>
    {
        public abstract Task<DataResponse<TResponse>> SendAsync();
    }
}
