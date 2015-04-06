namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.Info;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;

    /// <summary>
    /// Info that logger can store.
    /// </summary>
    [DataContract]
    public class LoggingMessage
    {
        //private int count;

        public LoggingMessage()
        {
            var info = Factory.Common.GetInstance<ApplicationInfo>();
            this.ApplicationVersion = info.Version;
            this.ApplicationName = info.ProductName;
        }

        public LoggingMessage(string message)
            : this()
        {
            this.Message = message;
        }

        public LoggingMessage(Exception ex)
            : this()
        {
            this.Message = ex.ToString();
        }

        [DataMember]
        public string Message { get; set; }

        [DataMember]
        public string ApplicationVersion { get; set; }

        [DataMember]
        public string ApplicationName { get; set; }
    }
}
