namespace BaseTools.Core.Info.ManifestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the Capability node which is used in application manifest. 
    /// </summary>
    public class CapabilityNode
    {
        /// <summary>
        /// Gets or sets the Name attribute.
        /// </summary>
        [XmlAttribute(AttributeName = "Name")]
        public string Name { get; set; }
    }
}
