namespace BaseTools.Core.Info
{
    using BaseTools.Core.Info;
    using BaseTools.Core.Info.ManifestModels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Linq;
    using System.Xml.Serialization;

    /// <summary>
    /// Class for reading app manifest properties
    /// </summary>
    internal class SilverlightApplicationInfo : ApplicationInfo
    {
        /// <summary>
        /// Initializes a new instance of the ManifestAppInfo class
        /// </summary>
        public SilverlightApplicationInfo()
        {
            var manifest = this.ParseMainifest();
            this.Version = manifest.App.Version; 
            var originProductId = manifest.App.ProductId;
            this.ProductId = originProductId.Trim('{', '}');
            this.ProductName = manifest.App.Title; 
            this.ApplicationInternalName = manifest.App.Title;
            this.StoreLink = "http://www.windowsphone.com/s?appid=" + this.ProductId;
            this.FillCapabilities(manifest);
        }

        private DeploymentNode ParseMainifest()
        {
            DeploymentNode manifest = null;
            using (var manifestStream = File.Open("WMAppManifest.xml", FileMode.Open, FileAccess.Read))
            {
                var serializer = new XmlSerializer(typeof(DeploymentNode));
                manifest = (DeploymentNode)serializer.Deserialize(manifestStream);
            }

            return manifest;
        }

        private void FillCapabilities(DeploymentNode manifest)
        {
            //<Capability Name="ID_CAP_NETWORKING" />
            //<Capability Name="ID_CAP_MEDIALIB_AUDIO" />
            //<Capability Name="ID_CAP_MEDIALIB_PLAYBACK" />
            //<Capability Name="ID_CAP_SENSORS" />
            //<Capability Name="ID_CAP_WEBBROWSERCOMPONENT" />
            //<Capability Name="ID_CAP_IDENTITY_USER" />
            //<Capability Name="ID_CAP_IDENTITY_DEVICE" />
            //<Capability Name="ID_CAP_APPOINTMENTS" />
            //<Capability Name="ID_CAP_CONTACTS" />
            //<Capability Name="ID_CAP_GAMERSERVICES" />
            //<Capability Name="ID_CAP_ISV_CAMERA" />
            //<Capability Name="ID_CAP_LOCATION" />
            //<Capability Name="ID_CAP_MAP" />
            //<Capability Name="ID_CAP_MEDIALIB_PHOTO" />
            //<Capability Name="ID_CAP_MICROPHONE" />
            //<Capability Name="ID_CAP_PHONEDIALER" />
            //<Capability Name="ID_CAP_PROXIMITY" />
            //<Capability Name="ID_CAP_PUSH_NOTIFICATION" />
            //<Capability Name="ID_CAP_REMOVABLE_STORAGE" />
            //<Capability Name="ID_CAP_SPEECH_RECOGNITION" />
            //<Capability Name="ID_CAP_VOIP" />
            //<Capability Name="ID_CAP_WALLET" />

            this.Capabilities = new Capabilities();
            foreach (var capability in manifest.App.Capabilities)
            {
                switch (capability.Name)
                {
                    case "ID_CAP_NETWORKING":
                        this.Capabilities.AllowNetworkAccess = true;
                        break;

                    case "ID_CAP_IDENTITY_DEVICE":
                        this.Capabilities.AllowDeviceInfoAccess = true;
                        break;

                    case "ID_CAP_LOCATION":
                        this.Capabilities.AllowAccessToLocation = true;
                        break;

                    case "ID_CAP_WEBBROWSERCOMPONENT":
                        this.Capabilities.AllowWebBrowserControl = true;
                        break;

                    case "ID_CAP_PHONEDIALER":
                        this.Capabilities.AllowCallsAndSms = true;
                        break;

                    case "ID_CAP_MEDIALIB_PHOTO":
                        this.Capabilities.AllowPhotoMediaLibrary = true;
                        break;
                }
            }
        }
    }
}

