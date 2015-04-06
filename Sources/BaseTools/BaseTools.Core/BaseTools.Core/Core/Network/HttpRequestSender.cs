namespace BaseTools.Core.Network
{
    using BaseTools.Core.DataAccess;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Base type for sending http requests. 
    /// </summary>
    public abstract class HttpRequestSender
    {
        protected string BuildParameters(HttpMessage message)
        {
            var builder = new StringBuilder();
            foreach (var parameter in message.Parameters)
            {
                if (parameter.Value != null && parameter.Key != null)
                {
                    var escapedKey = Uri.EscapeDataString(parameter.Key);
                    builder.Append(escapedKey);
                    builder.Append("=");

                    var escapedValue = parameter.Value;
                    if (parameter.NeedEscapeValue)
                    {
                        escapedValue = Uri.EscapeDataString(parameter.Value);
                    }
                    
                    builder.Append(escapedValue);
                    builder.Append("&");
                }
            }

            return builder.ToString();
        }

        public static Task<DataResponse<HttpResponse>> Send(string requestPath)
        {
            var message = new HttpMessage();
            message.FullRequestPath = requestPath;
            return Send(message);
        }

        public static async Task<DataResponse<string>> LoadString(string requestPath)
        {
            var response = new DataResponse<string>();
            var message = new HttpMessage();
            message.FullRequestPath = requestPath;
            var requestResponse = await Send(message).ConfigureAwait(false);
            if (requestResponse.IsSuccess)
            {
                using (requestResponse.Result)
                {
                    var reader = new StreamReader(requestResponse.Result.ResponseStream);
                    response.Result = reader.ReadToEnd();
                }
            }
            else
            {
                response.Error = requestResponse.Error;
            }

            return response; 
        }

        public static Task<DataResponse<HttpResponse>> Send(HttpMessage message)
        {
            var httpSender = Factory.Common.GetInstance<HttpRequestSender>();
            return httpSender.SendRequest(message);
        }

        public abstract Task<DataResponse<HttpResponse>> SendRequest(HttpMessage message);
    }
}
