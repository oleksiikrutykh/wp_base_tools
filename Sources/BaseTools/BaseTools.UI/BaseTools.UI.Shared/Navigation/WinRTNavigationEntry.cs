﻿namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Threading.Tasks;


    [DataContract(Name = "WinrtNavigationEntry")]
    public class WinRTNavigationEntry : NavigationEntry
    {
        [DataMember]
        public string PageType { get; set; }
    }
}
