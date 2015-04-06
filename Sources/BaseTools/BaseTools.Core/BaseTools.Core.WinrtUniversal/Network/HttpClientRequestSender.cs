namespace BaseTools.Core.Network
{
    using BaseTools.Core.DataAccess;
    using BaseTools.Core.Network;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using BaseTools.Core.Utility;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Diagnostics;

    /// <summary>
    /// Allows to send http requests. Use <see cref="System.Net.Http.HttpClient" /> class for this.
    /// </summary>
    internal class HttpClientRequestSender : HttpRequestSender
    {
        private const string GZipHeader = "gzip";

        private static HttpClientHandler clientHandler;

        private AsyncLock clientHandlerInitializerLock = new AsyncLock();

        private CookieStorage cookieStorage = new CookieStorage();

        public override async Task<DataResponse<HttpResponse>> SendRequest(HttpMessage message)
        {
            if (clientHandler == null)
            {
                using (var releaser = await clientHandlerInitializerLock.LockAsync())
                {
                    var cookieContainer = await this.cookieStorage.LoadCookies();
                    clientHandler = new HttpClientHandler
                    {
                        CookieContainer = cookieContainer
                    };
                }
            }

            if (message.Method == BaseTools.Core.Network.HttpMethod.Get)
            {
                var urlParameters = this.BuildParameters(message);
                message.FullRequestPath += "?" + urlParameters;
            }

            var result = new DataResponse<HttpResponse>();

            Uri requestUri = new Uri(message.FullRequestPath);
            if (message.Cookies.Count > 0)
            {
                var cookieUriSource = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", requestUri.Scheme, requestUri.Host);
                Uri cookieUri = new Uri(cookieUriSource);
                clientHandler.CookieContainer.Add(cookieUri, message.Cookies);
                clientHandler.UseCookies = true;
                // TODO: check is needed?
                clientHandler.UseDefaultCredentials = false;
            }

            var httpClient = new HttpClient(clientHandler);
            var gzipHeader = new StringWithQualityHeaderValue(GZipHeader);
            httpClient.DefaultRequestHeaders.AcceptEncoding.Add(gzipHeader);
            HttpResponseMessage response = null;
            this.AddHeaders(httpClient.DefaultRequestHeaders, message.Headers);
            try
            {
                if (message.Method == BaseTools.Core.Network.HttpMethod.Get)
                {
                    response = await httpClient.GetAsync(requestUri);
                }
                else
                {
                    var content = this.PrepearePostContent(message);
                    this.AddHeaders(content.Headers, message.Headers);
                    response = await httpClient.PostAsync(requestUri, content);
                }
            }
            catch (Exception e)
            {
                this.HandleError(e, result);
            }

            if (response != null)
            {
                if (response.IsSuccessStatusCode)
                {
                    result.Result = await this.CreateResponse(response);
                }
                else
                {
                    var networkProvider = Factory.Common.GetInstance<NetworkConnectionService>();
                    var isNetworkAvailable = await networkProvider.IsNetworkAvailable();
                    if (isNetworkAvailable)
                    {
                        var networkError = new NetworkError();
                        networkError.StatusCode = response.StatusCode;
                        networkError.Data = response;

                        result.Error = new ResponseError
                        {
                            ErrorType = DataErrorType.Network,
                            Message = "An error occured during sending request",
                            AssociatedData = networkError
                        };

                        if (response.Content != null)
                        {
                            result.Result = await this.CreateResponse(response);
                        }
                    }
                    else
                    {
                        result.Error = new ResponseError
                        {
                            ErrorType = DataErrorType.NoInternetConnection,
                            Message = "There isn't internet connection",
                        };
                    }
                }

                IEnumerable<string> setCookieHeaderValues = null;
                var headerFounded = response.Headers.TryGetValues("Set-Cookie", out setCookieHeaderValues);
                if (headerFounded)
                {
                    cookieStorage.AddKnownDomain(requestUri, setCookieHeaderValues);
                    cookieStorage.StoreCookies(clientHandler.CookieContainer);
                }
            }

            return result;
        }

        private async Task<HttpResponse> CreateResponse(HttpResponseMessage responseMesaage)
        {
            HttpResponse response = null;
            Stream stream = null;
            Stream decompressedStream = null;
            try
            {
                stream = await responseMesaage.Content.ReadAsStreamAsync();
                decompressedStream = HttpClientResponseDecompressor.Instance.TryDecompressStream(stream, responseMesaage);
                response = new HttpResponse(decompressedStream, stream);
                response.StatusCode = responseMesaage.StatusCode;
            }
            catch
            {
                DisposableHelper.DisposeSafe(stream);
                DisposableHelper.DisposeSafe(decompressedStream);
                throw;
            }

            return response;
        }

        protected virtual HttpContent PrepearePostContent(HttpMessage message)
        {
            HttpContent content = null;
            if (message.Files.Count > 0)
            {
                MultipartFormDataContent multipartContent = new MultipartFormDataContent();
                foreach (var paremeter in message.Parameters)
                {
                    var parameterContent = new StringContent(paremeter.Value.ToString());
                    parameterContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    parameterContent.Headers.ContentDisposition.Name = "\"" + paremeter.Key + "\"";
                    parameterContent.Headers.Remove("Content-Type");
                    multipartContent.Add(parameterContent);
                }

                foreach (var file in message.Files)
                {
                    var fileContent = new StreamContent(file.Content);
                    fileContent.Headers.ContentDisposition = new ContentDispositionHeaderValue("form-data");
                    fileContent.Headers.ContentDisposition.Name = "\"" + file.FieldName + "\"";
                    fileContent.Headers.ContentDisposition.FileName = "\"" + file.Name + "\"";
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(file.ContentType);
                    multipartContent.Add(fileContent, file.Name);
                }

                content = multipartContent;
            }
            else if (message.PostContent.Count > 0)
            {
                using (var stream = new MemoryStream())
                {
                    foreach (var item in message.PostContent)
                    {
                        var contentType = item.GetType();
                        var serialzier = new XmlSerializer(contentType);
                        serialzier.Serialize(stream, item);
                    }

                    var bytes = stream.ToArray();
                    content = new System.Net.Http.ByteArrayContent(bytes);
                    content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                }
            }
            else if (message.RequestContent != null)
            {
                using (var stream = new MemoryStream())
                {
                    message.RequestContent.SaveToStream(stream);
                    var bytes = stream.ToArray();
                    content = new System.Net.Http.ByteArrayContent(bytes);
                    this.AddHeaders(content.Headers, message.RequestContent.Headers);
                    //content.Headers.ContentType = new MediaTypeHeaderValue("text/xml");
                }
            }
            else
            {
                var parametersData = this.BuildParameters(message);
                content = new StringContent(parametersData, Encoding.UTF8, "application/x-www-form-urlencoded");
            }

            return content;
        }

        private void HandleError(Exception ex, DataResponse<HttpResponse> result)
        {
            var webException = ex as HttpRequestException;
            if (webException != null)
            {
                var networkError = new NetworkError();
                networkError.Data = webException;

                result.Error = new ResponseError
                {
                    ErrorType = DataErrorType.Network,
                    Message = "An error occured during sending request",
                    AssociatedData = networkError,
                };
            }
            else
            {
                if (ex.Message.Contains("ID_CAP_NETWORKING"))
                {
                    result.Error = new ResponseError
                    {
                        ErrorType = DataErrorType.UnautorizedAccess,
                        Message = "Please set network capability in manifest file",
                        AssociatedData = ex.Message
                    };
                }
                else
                {
                    result.Error = new ResponseError
                    {
                        ErrorType = DataErrorType.Unknown,
                        Message = "An unknown error is occurred",
                        AssociatedData = ex.Message
                    };

                    Logger.Instance.Write(ex);
                }
            }
        }

        private void AddHeaders(HttpHeaders headersCollection, Dictionary<string, string> addedHeaders)
        {
            foreach (var header in addedHeaders)
            {
                headersCollection.TryAddWithoutValidation(header.Key, header.Value);  
            }
        }
    }
}
