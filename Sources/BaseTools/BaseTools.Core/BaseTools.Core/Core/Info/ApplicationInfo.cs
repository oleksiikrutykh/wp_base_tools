namespace BaseTools.Core.Info
{
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    /// <summary>
    /// Class for reading app manifest properties
    /// </summary>
    public abstract class ApplicationInfo
    {
        private static ApplicationInfo current;

        public static ApplicationInfo Current
        {
            get
            {
                if (current == null)
                {
                    current = Factory.Common.GetInstance<ApplicationInfo>();
                }

                return current;
            }
        }

        /// <summary>
        /// Gets app manifest Version property
        /// </summary>
        public string Version { get; protected set; }

        /// <summary>
        /// Gets app manifest ProductId property
        /// </summary>
        public string ProductId { get; protected set; }

        /// <summary>
        /// Gets display name of application.
        /// </summary>
        public string ProductName { get; protected set; }

        /// <summary>
        /// Gets or sets name of application that used for internal purposes.
        /// </summary>
        public string ApplicationInternalName { get; set; }

        public string StoreLink { get; protected set; }

        public Capabilities Capabilities { get; protected set; }
    }

}
