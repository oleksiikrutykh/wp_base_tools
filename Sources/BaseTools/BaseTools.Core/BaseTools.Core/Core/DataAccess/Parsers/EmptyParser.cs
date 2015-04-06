namespace BaseTools.Core.DataAccess
{
    using BaseTools.Core.Network;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EmptyParser : ResponseParser<HttpResponse>
    {
        public override DataResponse<HttpResponse> ParseSafe(HttpResponse response)
        {
            var dataResponse = new DataResponse<HttpResponse>();
            dataResponse.Result = response;
            return dataResponse;
        }

        protected override DataResponse<HttpResponse> Parse(Stream content)
        {
            throw new NotImplementedException();
        }
    }
}
