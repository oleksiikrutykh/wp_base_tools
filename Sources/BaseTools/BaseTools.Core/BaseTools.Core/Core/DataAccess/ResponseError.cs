namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents an error that can be occured during fetching data.
    /// </summary>
    public class ResponseError
    {
        public ResponseError()
        { 
        }

        public ResponseError(DataErrorType errorType)
        {
            this.ErrorType = errorType;
        }

        public ResponseError(DataErrorType error, string message)
        {
            this.ErrorType = error;
            this.Message = message;
        }

        public ResponseError(DataErrorType errorType, object associatedData)
        {
            this.ErrorType = errorType;
            this.AssociatedData = associatedData;
        }

        public DataErrorType ErrorType { get; set; }

        public object AssociatedData { get; set; }

        public string Message { get; set; }
    }
}
