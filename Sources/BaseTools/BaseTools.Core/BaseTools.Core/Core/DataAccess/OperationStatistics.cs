namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [DataContract]
    public class OperationStatistics
    {
        [DataMember]
        public int Count { get; set; }


        public DateTime LastSuccessExecutionDate { get; set; }

        [DataMember]
        public long LastSuccessExecutionTicks
        {
            get
            {
                return this.LastSuccessExecutionDate.Ticks;
            }

            set
            {
                this.LastSuccessExecutionDate = new DateTime(value);
            }
        }

        [DataMember]
        public int UnsuccessfulTriesCount { get; set; }

        public OperationStatistics Clone()
        {
            var clone = new OperationStatistics
            {
                Count = this.Count,
                LastSuccessExecutionDate = this.LastSuccessExecutionDate,
                UnsuccessfulTriesCount = this.UnsuccessfulTriesCount
            };

            return clone;
        }
    }
}
