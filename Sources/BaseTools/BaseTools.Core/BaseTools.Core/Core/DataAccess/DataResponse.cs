namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represetns result of fetching data.
    /// </summary>
    /// <typeparam name="T">The type of fetched data.</typeparam>
    public class DataResponse<T>
    {
        public DataResponse()
        {
        }

        public DataResponse(ResponseError error)
        {
            this.Error = error;
        }

        public T Result { get; set; }

        public ResponseError Error { get; set; }

        public bool IsSuccess
        {
            get
            {
                return this.Error == null;
            }
        }

        public DataResponse<TOther> Convert<TOther>()
        {
            var response = new DataResponse<TOther>();
            if (this.IsSuccess)
            {
                //response.Result = successConverter.Invoke(this.Result);
            }
            else
            {
                response.Error = this.Error;
            }

            return response;
        }
    }
}
