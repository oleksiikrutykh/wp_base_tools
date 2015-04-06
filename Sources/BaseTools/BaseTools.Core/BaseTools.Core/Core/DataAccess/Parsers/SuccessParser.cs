namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Represents parser that return success response with default value. It doesn't read anything from stream 
    /// </summary>
    /// <typeparam name="T">Type of requested data.</typeparam>
    public class SuccessParser<T> : ResponseParser<T>
    {
        protected override DataResponse<T> Parse(System.IO.Stream content)
        {
            return new DataResponse<T>();
        }
    }
}
