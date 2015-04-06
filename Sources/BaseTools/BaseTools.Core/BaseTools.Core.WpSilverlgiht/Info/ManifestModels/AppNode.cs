namespace BaseTools.Core.Info.ManifestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the Application node which is used in application manifest. 
    /// </summary>
    public class AppNode
    {
        /// <summary>
        /// Gets or sets a value of the "Title" attribute.
        /// </summary>
        [XmlAttribute(AttributeName = "Title")]
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets a value of the "Version" attribute.
        /// </summary>
        [XmlAttribute(AttributeName = "Version")]
        public string Version { get; set; }

        /// <summary>
        /// Gets or sets a value of the "ProductID" attribute.
        /// </summary>
        [XmlAttribute(AttributeName = "ProductID")]
        public string ProductId { get; set; }

        /// <summary>
        /// Gets or sets a collection of the "Capability" child nodes.
        /// </summary>
        [XmlArray(ElementName = "Capabilities")]
        [XmlArrayItem(ElementName = "Capability")]
        public List<CapabilityNode> Capabilities { get; set; }
    }
}
