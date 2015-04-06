namespace BaseTools.Core.DataAccess
{
    using BaseTools.Core.Diagnostics;
    using BaseTools.Core.Network;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains basic logic for parsing.
    /// </summary>
    /// <typeparam name="T">Type of requested data.</typeparam>
    public abstract class ResponseParser<T>
    {
        public virtual DataResponse<T> ParseSafe(HttpResponse response)
        {
            using (response)
            {
                return this.ParseSafe(response.ResponseStream);
            }
        }

        private DataResponse<T> ParseSafe(Stream contentStream)
        {
            DataResponse<T> result;
            
            try
            {
                result = Parse(contentStream);
            }
            catch (Exception ex)
            {
                result = new DataResponse<T>();
                result.Error = new ResponseError
                {
                    ErrorType = DataErrorType.Parsing,
                    Message = "An error occured during parsing",
                    AssociatedData = ex
                };

                Logger.Instance.Write(ex);
            }

            return result;
        }

        protected abstract DataResponse<T> Parse(Stream content);
    }
}
