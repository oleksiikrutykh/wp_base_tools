namespace BaseTools.Core.DataAccess
{
    using System.IO;
    using System.Runtime.Serialization.Json;

    /// <summary>
    /// Parser that can convert json to data. 
    /// </summary>
    /// <typeparam name="T">Type of requested data.</typeparam>
    public class JsonParser<T> : ResponseParser<T>
    {
        protected override DataResponse<T> Parse(System.IO.Stream contentStream)
        {
            DataResponse<T> response = null;
            if (contentStream.CanSeek)
            {
                response = this.ParseSeekableStream(contentStream);
            }
            else
            {
                using (var memoryStream = new MemoryStream())
                {
                    contentStream.CopyTo(memoryStream);
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    response = this.ParseSeekableStream(memoryStream);
                }
            }

            return response;
        }

        private DataResponse<T> ParseSeekableStream(Stream contentStream)
        {
            var response = new DataResponse<T>();
            if (contentStream.Length > 0)
            {
                var isValidJson = this.ValidateJson(contentStream);
                if (isValidJson)
                { 
                    contentStream.Seek(0, SeekOrigin.Begin);
                    response = this.ParseJson(contentStream);
                }
                else
                {
                    response.Error = new ResponseError(DataErrorType.Parsing, "The content is not a valid json");
                }
            }

            return response;
        }

        protected bool ValidateJson(Stream contentStream)
        {
            bool isValid = false;
            // TODO: logic for checking utf headers - for winrt.
            var reader = new StreamReader(contentStream);
            char firstSymbol = (char)reader.Read();
            if (firstSymbol == '{' ||
                firstSymbol == '[')
            {
                isValid = true;    
            }

            return isValid;
        }

        protected virtual DataResponse<T> ParseJson(Stream contentStream)
        {
            var response = new DataResponse<T>();
            var serializer = new DataContractJsonSerializer(typeof(T));
            var parsedData = (T)serializer.ReadObject(contentStream);
            response.Result = parsedData;
            return response;
        }
    }
}
