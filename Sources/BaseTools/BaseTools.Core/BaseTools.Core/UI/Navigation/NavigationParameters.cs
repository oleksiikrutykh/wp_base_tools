namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract]
    public class NavigationParameters
    {
        public virtual void InitializeFromUriParameters(Dictionary<string, string> parameters)
        {
            
        }

        public virtual Dictionary<string, string> StoreToUriParameters()
        {
            return new Dictionary<string, string>();
        }
    }
}
