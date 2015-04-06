namespace BaseTools.Core.Diagnostics
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;

    [DataContract]
    public class StorableLoggingMessage
    {
        private int count;

        [DataMember]
        public LoggingMessage Message { get; set; }

        [DataMember]
        public int Count
        {
            get
            {
                return this.count;
            }

            set
            {
                this.count = value;
            }
        }

        [DataMember]
        public DateTime SuccessfullSendDate { get; set; }

        internal void IncrementCount()
        {
            Interlocked.Increment(ref this.count);
        }

        internal void DecrementCount()
        {
            Interlocked.Decrement(ref this.count);
            this.SuccessfullSendDate = DateTime.UtcNow;
        }
        
    }
}
