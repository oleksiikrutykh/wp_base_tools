namespace BaseTools.Core.Info.ManifestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the Package node used which is in application manifest. 
    /// </summary>
    [XmlRoot(ElementName = "Package", Namespace = "http://schemas.microsoft.com/appx/2010/manifest")]
    public class PackageNode
    {
        /// <summary>
        /// Gets or sets a collection of the "Capability" child nodes.
        /// </summary>
        [XmlArray(ElementName = "Capabilities")]
        [XmlArrayItem(ElementName = "Capability", Type = typeof(CapabilityNode))]
        [XmlArrayItem(ElementName = "DeviceCapability", Type = typeof(DeviceCapabilityNode))]
        public List<CapabilityNode> Capabilities { get; set; }
    }
}
