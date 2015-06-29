namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Create text representation of http method.
    /// </summary>
    public static class HttpMethodConverter
    {
        public static string ConvertToString(HttpMethod method)
        {
            string methodString = "GET";
            switch (method)
            {
                case HttpMethod.Post:
                    methodString = "POST";
                    break;

                case HttpMethod.Put:
                    methodString = "PUT";
                    break;

                case HttpMethod.Delete:
                    methodString = "DELETE";
                    break;
            }
            
            return methodString;
        }
    }
}
