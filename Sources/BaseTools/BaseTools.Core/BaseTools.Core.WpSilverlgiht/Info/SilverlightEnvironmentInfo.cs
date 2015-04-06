namespace BaseTools.Core.Info
{
    using BaseTools.Core.Info;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Storage;
    using Microsoft.Phone.Info;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;

    /// <summary>
    /// Contains information about execution environment (info about device, operation system, etc.). 
    /// Can be used for Windows Phone Silverlight apps.
    /// </summary>
    internal class SilverlightEnvironmentInfo : EnvironmentInfo
    {
        private const string EmulatorDeviceId = "EmulatorDeviceId";

        private string deviceUniqueId;

        public SilverlightEnvironmentInfo()
        {
            this.OperatingSystemType = OperatingSystemType.WindowsPhoneSilverlight;
            var applicationInstanceType = Application.Current.GetType();
            var isAgent = applicationInstanceType.Namespace.StartsWith("Microsoft.Phone", StringComparison.Ordinal);
            if (isAgent)
            {
                this.EnvironmentType = EnvironmentType.BackgroundAgent;
            }
        }

        public override string DeviceName
        {
            get 
            {
                object deviceName = null; 
                bool isExist = DeviceExtendedProperties.TryGetValue("DeviceName",  out deviceName);
                if (!isExist)
                {
                    deviceName = string.Empty;
                }

                return (string)deviceName;
                //return DeviceStatus.DeviceName;
            }
        }

        public override string DeviceManufacturer
        {
            get 
            {
                object deviceManufacturer = null; 
                bool isExist = DeviceExtendedProperties.TryGetValue("DeviceManufacturer",  out deviceManufacturer);
                if (!isExist)
                {
                    deviceManufacturer = string.Empty;
                }

                return (string)deviceManufacturer;
            }
        }

        public override string DeviceUniqueId
        {
            get 
            { 
                if (this.deviceUniqueId == null)
                {
                    if (!this.IsInDesignMode)
                    {
                        object uniqueId;
                        bool isExist = DeviceExtendedProperties.TryGetValue("DeviceUniqueId", out uniqueId);
                        if (isExist)
                        {
                            byte[] idBytes = (byte[])uniqueId;
                            this.deviceUniqueId = Convert.ToBase64String(idBytes);
                        }
                        else
                        {
                            var storageProvider = Factory.Common.GetInstance<IStorageProvider>();
                            this.deviceUniqueId = storageProvider.ReadFromSettings<string>(EmulatorDeviceId);
                            if (String.IsNullOrEmpty(this.deviceUniqueId))
                            {
                                this.deviceUniqueId = Guid.NewGuid().ToString();
                                storageProvider.WriteToSettings<string>(EmulatorDeviceId, this.deviceUniqueId);
                            }
                        }
                    }
                    else
                    {
                        this.deviceUniqueId = Guid.NewGuid().ToString();
                    }

                }

                return this.deviceUniqueId;
            }
        }

        public override double DevicePixelsWidth
        {
            get 
            { 
                return Application.Current.Host.Content.ActualWidth;
            }
        }

        public override double DevicePixelsHeight
        {
            get 
            { 
                return Application.Current.Host.Content.ActualHeight;
            }
        }

        public override Version OperatingSystemVersion
        {
            get 
            {
                return Environment.OSVersion.Version;
            }
        }

        public override bool IsInDesignMode
        {
            get 
            {
                return DesignerProperties.IsInDesignTool;
            }
        }
    }
}
