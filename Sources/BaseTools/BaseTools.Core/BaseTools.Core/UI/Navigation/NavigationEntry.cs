using System.Runtime.Serialization;
namespace BaseTools.UI.Navigation
{
    [DataContract]
    public class NavigationEntry
    {
        [DataMember]
        public object Source { get; set; }

        [DataMember]
        public object Parameter { get; set; }

        [DataMember]
        public bool IsUnknown { get; set; }


        public object Content { get; set; }
    }
}
