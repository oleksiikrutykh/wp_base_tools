﻿namespace PhoneApp14.SampleData
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;

    [DataContract]
    public class Item
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Name { get; set; }
    }
}
