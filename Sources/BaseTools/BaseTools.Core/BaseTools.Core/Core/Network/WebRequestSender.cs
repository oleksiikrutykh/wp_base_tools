namespace BaseTools.Core.Network
{
    using BaseTools.Core.DataAccess;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Perform http requests sending. Use <see cref="System.Net.HttpWebRequest"/> API for this.
    /// </summary>
    public class WebRequestSender : HttpRequestSender
    {
        private static NetworkConnectionService networkConnectionService = Factory.Common.GetInstance<NetworkConnectionService>();

        private static CookieContainer cookieContainer;

        private CookieStorage cookieStorage = new CookieStorage();

        private AsyncLock cookieContainerInitializeLock = new AsyncLock();

        public override Task<DataResponse<HttpResponse>> SendRequest(HttpMessage message)
        {
            var state = new WebRequestState();
            state.Message = message;
            if (message.Method == HttpMethod.Get)
            {
                var urlParameters = this.BuildParameters(message);
                message.FullRequestPath += "?" + urlParameters;
            }

            this.StartSendingRequest(state);
            return state.TaskSource.Task;
        }

        private async void StartSendingRequest(WebRequestState state)
        {
            var isNetworkAvailable = await networkConnectionService.IsNetworkAvailable();
            if (isNetworkAvailable)
            {
                var message = state.Message;
                var request = (HttpWebRequest)HttpWebRequest.Create(message.FullRequestPath);
                state.Request = request;

                var isGzipSupported = Factory.Common.IsBindingExist(typeof(BaseTools.Core.IO.GZipStreamProvider));
                if (isGzipSupported)
                {
                    request.Headers[HttpRequestHeader.AcceptEncoding] = "gzip";
                }

                request.Method = HttpMethodConverter.ConvertToString(message.Method);
                if (cookieContainer == null)
                {
                    using (var releaser = await cookieContainerInitializeLock.LockAsync())
                    {
                        if (cookieContainer == null)
                        {
                            cookieContainer = await this.cookieStorage.LoadCookies();
                        }
                    }
                }

                request.CookieContainer = cookieContainer;
                Uri requestUri = new Uri(message.FullRequestPath);
                if (message.Cookies.Count > 0)
                {
                    var cookieUriSource = string.Format(CultureInfo.InvariantCulture, "{0}://{1}", requestUri.Scheme, requestUri.Host);
                    Uri cookieUri = new Uri(cookieUriSource);
                    request.CookieContainer.Add(cookieUri, message.Cookies);
                }

                if (message.Method == HttpMethod.Post)
                {
                    if (message.Files.Count > 0)
                    {
                        state.Boundary = String.Format(CultureInfo.InvariantCulture, "--{0:N}", Guid.NewGuid());
                        var contentType = String.Format(CultureInfo.InvariantCulture, "multipart/form-data; boundary=\"{0}\"", state.Boundary);
                        request.ContentType = contentType;
                    }
                    else if (message.RequestContent != null)
                    {
                        this.SetupHeaders(request, message.RequestContent.Headers);
                    }
                    else if (message.PostContent.Count > 0)
                    {
                        // TODO: use different serializers and content types.
                        request.ContentType = "text/xml; charset=utf-8";
                    }
                    else
                    {
                        request.ContentType = "application/x-www-form-urlencoded";
                    }

                    this.SetupHeaders(request, message.Headers);
                    request.BeginGetRequestStream(this.OnRequestStreamRetrieved, state);
                }
                else
                {
                    this.SetupHeaders(request, message.Headers);
                    this.GetResponse(state);
                }
            }
            else
            { 
                var result = new DataResponse<HttpResponse>();
                result.Error = new ResponseError
                {
                    ErrorType = DataErrorType.NoInternetConnection,
                    Message = "There isn't internet connection",
                };

                state.TaskSource.SetResult(result);
            }
        }

        private void SetupHeaders(HttpWebRequest request, Dictionary<string, string> headers)
        {
            foreach (var header in headers)
            {
                switch (header.Key.ToLower())
                {

                    case "content-type":
                        if (request.Method != HttpMethodConverter.ConvertToString(HttpMethod.Get))
                        {
                            request.ContentType = header.Value;
                        }
                        break;

                    case "accept":
                        request.Accept = header.Value;
                        break;

                    default:
                        request.Headers[header.Key] = header.Value;
                        break;
                }
            }
        }

        private void OnRequestStreamRetrieved(IAsyncResult asyncResult)
        {
            var state = (WebRequestState)asyncResult.AsyncState;
            try
            {
                using (Stream requestStream = state.Request.EndGetRequestStream(asyncResult))
                {
                    this.PrepareContentData(state, requestStream);
                }

                GetResponse(state);
            }
            catch (Exception ex)
            {
                this.HandleError(ex, state);
            }
        }

        private void PrepareContentData(WebRequestState state, Stream requestStream)
        {
            var message = state.Message;
            if (message.Files.Count == 0)
            {
                if (message.RequestContent != null)
                {
                    message.RequestContent.SaveToStream(requestStream);
                }
                else if (message.PostContent.Count > 0)
                {
                    using (TextWriter writer = new StreamWriter(requestStream, new System.Text.UTF8Encoding(false), 4096, true))
                    {
                        foreach (var item in message.PostContent)
                        {
                            var contentType = item.GetType();
                            var serialzier = new XmlSerializer(contentType);
                            serialzier.Serialize(writer, item);
                            writer.Flush();
                        }
                    }
                }
                else
                {
                    using (var writer = new StreamWriter(requestStream))
                    {
                        var urlParameters = this.BuildParameters(message);
                        writer.Write(urlParameters);
                    }
                }
            }
            else
            {
                //http://www.w3.org/TR/html401/interact/forms.html#h-17.13.4

                using (var writer = new StreamWriter(requestStream))
                {
                    foreach (var parameter in message.Parameters)
                    {
                        var parameterData = String.Format(CultureInfo.CurrentCulture,
                                                          "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"\r\n\r\n{2}\r\n",
                                                          state.Boundary,
                                                          parameter.Key,
                                                          parameter.Value);
                        writer.Write(parameterData);
                    }

                    writer.Flush();

                    foreach (var file in message.Files)
                    {
                        var fileHeader = string.Format(CultureInfo.CurrentCulture,
                                                       "--{0}\r\nContent-Disposition: form-data; name=\"{1}\"; filename=\"{2}\"\r\nContent-Type: {3}\r\n\r\n",
                                                       state.Boundary,
                                                       file.FieldName,
                                                       file.Name,
                                                       file.ContentType ?? "application/octet-stream"
                                                       );
                        writer.Write(fileHeader);
                        writer.Flush();
                        file.Content.CopyTo(requestStream);
                        writer.Write(Environment.NewLine);
                        writer.Flush();
                    }

                    var contentEnding = String.Format("--{0}--", state.Boundary);
                    writer.Write(contentEnding);
                    writer.Flush();
                }
            }
        }

        private void GetResponse(WebRequestState state)
        {
            state.Request.BeginGetResponse(this.OnResponse, state);
        }

        private void OnResponse(IAsyncResult asyncResult)
        {
            var state = (WebRequestState)asyncResult.AsyncState;
            try
            {
                var result = new DataResponse<HttpResponse>();
                var response = (HttpWebResponse)state.Request.EndGetResponse(asyncResult);
                result.Result = this.CreateResponse(response);
                state.TaskSource.SetResult(result);
            }
            catch (Exception ex)
            {
                this.HandleError(ex, state);
            }
        }

        private void HandleError(Exception ex, WebRequestState state)
        {
            var webException = ex as WebException;
            if (webException != null)
            {
                if (webException.Status == WebExceptionStatus.RequestCanceled)
                {
                    this.StartSendingRequest(state);
                }
                else
                {
                    var responseData = new DataResponse<HttpResponse>();
                    
                    var networkError = new NetworkError();
                    networkError.Data = webException;

                    var errorResponse = webException.Response as HttpWebResponse;
                    if (errorResponse != null)
                    {
                        networkError.StatusCode = errorResponse.StatusCode;
                        responseData.Result = this.CreateResponse(errorResponse);
                    }

                    responseData.Error = new ResponseError
                    {
                        ErrorType = DataErrorType.Network,
                        Message = "An error occured during sending request",
                        AssociatedData = networkError
                    };

                    state.TaskSource.SetResult(responseData);
                }
            }
            else
            {
                state.TaskSource.SetException(ex);
            }
        }

        private HttpResponse CreateResponse(HttpWebResponse webResponse)
        {
            if (webResponse.Headers.AllKeys.Contains("Set-Cookie"))
            {
                var header = webResponse.Headers["Set-Cookie"];
                this.cookieStorage.AddKnownDomain(webResponse.ResponseUri, header);
                this.cookieStorage.StoreCookies(cookieContainer);
            }

            HttpResponse response = null;
            Stream stream = null;
            Stream decompressedStream = null;
            try
            {
                stream = webResponse.GetResponseStream();
                decompressedStream = ResponseDecompressor.Instance.TryDecompressWebResponseStream(webResponse, stream);
                response = new HttpResponse(decompressedStream, stream);
                response.StatusCode = webResponse.StatusCode;
            }
            catch
            {
                DisposableHelper.DisposeSafe(decompressedStream);
                DisposableHelper.DisposeSafe(stream);
                throw;
            }

            return response;
        }
    }
}
