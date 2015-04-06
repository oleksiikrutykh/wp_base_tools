namespace BaseTools.Core.Info
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Information about manifest capabilities.
    /// </summary>
    public class Capabilities
    {
        public bool AllowNetworkAccess { get; set; }

        public bool AllowDeviceInfoAccess { get; set; }

        public bool AllowAccessToLocation { get; set; }

        public bool AllowWebBrowserControl { get; set; }

        public bool AllowCallsAndSms { get; set; }

        public bool AllowPhotoMediaLibrary { get; set; }
    }
}
