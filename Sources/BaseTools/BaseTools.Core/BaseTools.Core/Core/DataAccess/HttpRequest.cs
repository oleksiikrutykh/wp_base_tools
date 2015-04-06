namespace BaseTools.Core.DataAccess
{
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Network;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml;
    using System.Xml.Serialization;
    using System.Reflection;
    using BaseTools.Core.Utility;

    /// <summary>
    /// Represents logic for fetching data from network via http request.
    /// </summary>
    /// <typeparam name="TResponse">The type of fetched data.</typeparam>
    public class HttpRequest<TResponse> : DataRequest<TResponse>
    {
        private string fullPath;

        private string domainPath;

        private string path;

        public HttpRequest()
        {
            this.Parser = new JsonParser<TResponse>();
            this.Parameters = new ParametersCollection();
            this.PostContent = new List<object>();
            this.Files = new List<HttpFile>();
            this.Headers = new Dictionary<string, string>();
            this.CookieCollection = new CookieCollection();
        }

        public HttpMethod Method { get; set; }

        public string DomainPath 
        {
            get
            {
                return this.domainPath;
            }

            set
            {
                this.domainPath = value;
                this.CalculateFullPath();
            }
        }

        public string Path 
        {
            get
            {
                return this.path;
            }

            set
            {
                this.path = value;
                this.CalculateFullPath();
            }
        }

        public string FullPath
        {
            get
            {
                return this.fullPath;
            }
            set
            {
                this.fullPath = value;
            }
        }

        public ResponseParser<TResponse> Parser { get; set; }

        public ResponseParser<TResponse> ErrorParser { get; set; }

        public List<object> PostContent { get; private set; }

        public RequestContent Content { get; set; }

        public List<HttpFile> Files { get; private set; }

        //public List<RequestParameter> Parameters { get; private set; }

        public ParametersCollection Parameters { get; private set; }

        public CookieCollection CookieCollection { get; private set; }

        public Dictionary<string, string> Headers { get; private set; }

        public void AddNotDefaultParameter<T>(string key, T value)
        {
            bool isParameterEmpty = true;
            var stringValue = value as string;
            if (stringValue != null)
            {
                isParameterEmpty = stringValue == String.Empty;
            }
            else
            {
                isParameterEmpty = Object.Equals(value, default(T));
            }

            if (!isParameterEmpty)
            {
                this.AddParameter(key, value);
            }
        }

        public void AddNotDefaultParameter(string key, object value)
        {
            bool isParameterEmpty = true;
            if (value != null)
            {
                var stringValue = value as string;
                if (stringValue != null)
                {
                    isParameterEmpty = stringValue == String.Empty;
                }
                else
                {
                    var type = value.GetType();
                    var typeInfo = type.GetTypeInfo();
                    bool isValueType = typeInfo.IsValueType;
                    if (isValueType)
                    {
                        var defaultValue = Activator.CreateInstance(type);
                        isParameterEmpty = Object.Equals(value, defaultValue);
                    }
                    else
                    {
                        isParameterEmpty = false;
                    }
                }
            }

            if (!isParameterEmpty)
            {
                this.AddParameter(key, value);
            }
        }

        public void AddParameter(string key, object value)
        {
            this.AddParameter(key, value, true);
        }

        public void AddParameter(string key, object value, bool allowsUriEscape)
        {
            var stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
            var parameter = new RequestParameter
            {
                Key = key,
                Value = stringValue,
                NeedEscapeValue = allowsUriEscape
            };

            this.Parameters.Add(parameter);
        }

        public override async Task<DataResponse<TResponse>> SendAsync()
        {
            DataResponse<TResponse> result = new DataResponse<TResponse>();
            var message = new HttpMessage();
            message.Files = this.Files;
            message.FullRequestPath = this.FullPath;
            message.Method = this.Method;
            message.Parameters = this.Parameters;
            message.PostContent = this.PostContent;
            message.Cookies = this.CookieCollection;
            message.Headers = this.Headers;
            message.RequestContent = this.Content;

            var httpSender = Factory.Common.GetInstance<HttpRequestSender>();
            var response = await httpSender.SendRequest(message);
            if (response.IsSuccess)
            {
                var httpResponse = response.Result;
                result = this.Parser.ParseSafe(httpResponse);

                //using (var httpResponse = response.Result)
                //{
                //    //view response as text.
                //    //using (var memoryStream = new MemoryStream())
                //    //{
                //    //    httpResponse.ResponseStream.CopyTo(memoryStream);
                //    //    memoryStream.Seek(0, SeekOrigin.Begin);

                //    //    var reader = new StreamReader(memoryStream);
                //    //    var res = reader.ReadToEnd();
                //    //    res.ToString();
                //    //    memoryStream.Seek(0, SeekOrigin.Begin);

                //    //    result = this.Parser.ParseSafe(memoryStream);
                //    //}

                //    result = this.Parser.ParseSafe(httpResponse.ResponseStream);
                //}
            }
            else
            {
                result.Error = response.Error;
                if (response.Result != null)
                {
                    if (this.ErrorParser != null)
                    {
                        var httpResponse = response.Result;
                        var parsedResponse = this.ErrorParser.ParseSafe(httpResponse);
                        if (parsedResponse != null)
                        {
                            result = parsedResponse;
                        }
                    }
                    else
                    {
                        response.Result.Dispose();
                    }
                }
            }
            
            return result;
        }

        private void CalculateFullPath()
        {
            this.FullPath = this.DomainPath + this.Path;
        }
    }
}
