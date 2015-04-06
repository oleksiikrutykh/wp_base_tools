namespace BaseTools.Core.Info.ManifestModels
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;

    /// <summary>
    /// Represents the Deployment node which is used in application manifest. 
    /// </summary>
    [XmlRoot(ElementName = "Deployment", Namespace = "http://schemas.microsoft.com/windowsphone/2012/deployment")]
    public class DeploymentNode
    {
        /// <summary>
        /// Gets or sets and "App" child node.
        /// </summary>
        [XmlElement(ElementName = "App", Namespace = "")]
        public AppNode App { get; set; }
    }
}
