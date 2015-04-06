namespace BaseTools.Core.Info
{
    using BaseTools.Core.Info;
    using BaseTools.Core.Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.ApplicationModel;
    using Windows.Security.ExchangeActiveSyncProvisioning;
    using Windows.UI.Xaml;

    /// <summary>
    /// Contains information about execution environment (info about device, operation system, etc.) Can be used in WinRT apps.
    /// </summary>
    internal class WinrtEnvironmentInfo : EnvironmentInfo
    {
        private const string DeviceUniqueIdStorageKey = "IconPeakDeviceId";

        private static object deviceIdLock = new object();

        private EasClientDeviceInformation deviceInfo = new EasClientDeviceInformation();

        public WinrtEnvironmentInfo()
        {
            try
            {
                var currentApp = Application.Current;
                if (currentApp == null && Window.Current == null)
                {
                    this.EnvironmentType = EnvironmentType.BackgroundAgent;
                }
            }
            catch (Exception ex)
            {
                // The exception is occured in background agent during access Application.Current property 
                this.EnvironmentType = EnvironmentType.BackgroundAgent;
            }
        }

        public override string DeviceName
        {
            get
            {
                return this.deviceInfo.SystemProductName; 
            }
        }

        public override string DeviceManufacturer
        {
            get 
            {
                return this.deviceInfo.SystemManufacturer;
            }
        }

        public override string DeviceUniqueId
        {
            get 
            {
                string deviceId = null;
#if SILVERLIGHT_APP
                var storageProvider = BaseTools.Core.Ioc.Factory.Common.GetInstance<IStorageProvider>();
                deviceId = storageProvider.ReadFromSettings<string>(DeviceUniqueIdStorageKey);
                if (deviceId == null)
                {
                    lock (deviceIdLock)
                    {
                        deviceId = storageProvider.ReadFromSettings<string>(DeviceUniqueIdStorageKey);
                        if (deviceId == null)
                        {
                            deviceId = Guid.NewGuid().ToString();
                            storageProvider.WriteToSettings<string>(DeviceUniqueIdStorageKey, deviceId);
                        }
                    }
                }
#endif

#if WINDOWS_APP
                deviceId = this.deviceInfo.Id.ToString();
#endif
                return deviceId;
            }
        }

        public override Version OperatingSystemVersion
        {
            get 
            {
                return new Version("8.1.0.0");
            }
        }

        public override double DevicePixelsWidth
        {
            get 
            { 
                return Window.Current.Bounds.Width;
            }
        }

        public override double DevicePixelsHeight
        {
            get
            {
                return Window.Current.Bounds.Height;
            }
        }

        public override bool IsInDesignMode
        {
            get 
            {
                return DesignMode.DesignModeEnabled;
            }
        }
    }
}
