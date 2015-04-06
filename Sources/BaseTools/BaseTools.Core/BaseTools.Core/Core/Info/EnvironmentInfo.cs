namespace BaseTools.Core.Info
{
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains information about execution environment (info about device, operation system, etc.)
    /// </summary>
    public abstract class EnvironmentInfo
    {
        private static EnvironmentInfo current;

        public static EnvironmentInfo Current
        {
            get
            {
                if (current == null)
                {
                    current = Factory.Common.GetInstance<EnvironmentInfo>();
                }

                return current;
            }
        }

        public EnvironmentType EnvironmentType { get; protected set; }

        public OperatingSystemType OperatingSystemType { get; protected set; }

        public abstract Version OperatingSystemVersion { get; }

        public abstract string DeviceName { get; }

        public abstract string DeviceManufacturer { get; }

        public abstract string DeviceUniqueId { get; }

        public abstract double DevicePixelsWidth { get; }

        public abstract double DevicePixelsHeight { get; }

        public abstract bool IsInDesignMode { get; }
    }
}
