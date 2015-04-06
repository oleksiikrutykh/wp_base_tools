namespace BaseTools.Core
{
#if WINRT
    using BaseTools.Core.Network;
#endif

    using BaseTools.Core.Common;
    using BaseTools.Core.Diagnostics;
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Geolocation;
    using BaseTools.Core.Info;
    using BaseTools.Core.IO;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Network;
    using BaseTools.Core.Storage;
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents IOC initilaizer for all abstractions in toolkit.
    /// </summary>
    public class CoreFactoryInitializer : FactoryInitializer
    {
        public override void SetupBindnings(Factory initializedFactory)
        {
            var assemblyName = typeof(CoreFactoryInitializer).AssemblyQualifiedName;

            Guard.CheckIsNotNull(initializedFactory, "initializedFactory");
            initializedFactory.Bind<IStorageProvider, StorageProvider>();
            initializedFactory.Bind<Logger, GoogleAnalyticsLogger>();
            
            initializedFactory.Bind<IFileSystemProvider, FileSystemProvider>();
            initializedFactory.Bind<IGeoCoordinateWatcher, GeoCoordinateWatcher>();
            initializedFactory.Bind<DateTimeProvider, DateTimeProvider>();
            initializedFactory.Bind<RandomProvider, RandomProvider>();

#if SILVERLIGHT
            initializedFactory.Bind<NetworkConnectionService, SilverlightNetworkConnectionService>();
            initializedFactory.Bind<EnvironmentInfo, SilverlightEnvironmentInfo>();
            initializedFactory.Bind<ApplicationInfo, SilverlightApplicationInfo>();
#endif

#if WINRT
            initializedFactory.Bind<NetworkConnectionService, WinrtNetworkConnectionService>();
            initializedFactory.Bind<GZipStreamProvider, WinrtGzipStreamProvider>();
            initializedFactory.Bind<HttpRequestSender, HttpClientRequestSender>();
            initializedFactory.Bind<EnvironmentInfo, WinrtEnvironmentInfo>();
            initializedFactory.Bind<ApplicationInfo, WinrtApplicationInfo>();
#endif

        }
    }
}
