namespace BaseTools.Core.Info
{
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Info;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Threading;
    using BaseTools.Core.Info.ManifestModels;
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Xml.Serialization;
    using Windows.ApplicationModel;
    using Windows.Storage;

    /// <summary>
    /// Reading app manifest properties in WinRT app.
    /// </summary>
    internal class WinrtApplicationInfo : ApplicationInfo
    {
        public static string ProductDisplayName { get; set; }

        public WinrtApplicationInfo()
        {
            // TODO: parse manifest here.
            var packageVersion = Package.Current.Id.Version;
            this.Version = String.Join(".", packageVersion.Major, packageVersion.Minor, packageVersion.Build, packageVersion.Revision);
            this.ProductId = Package.Current.Id.Name;

            if (String.IsNullOrEmpty(ProductDisplayName))
            {
                this.ProductName = Package.Current.Id.FamilyName;
            }
            else
            {
                this.ProductName = ProductDisplayName;
            }

            this.ApplicationInternalName = this.ProductName;
            this.StoreLink = "ms-windows-store:PDP?PFN=" + Package.Current.Id.FamilyName;
            this.Capabilities = new Capabilities();
            ParseManifest();
        }

        private void ParseManifest()
        {
            var parsingTask = Task.Run(new Func<Task>(this.ParseManifestAsync));
            TaskWaiter.WaitSynchronously(parsingTask);
        }

        private async Task ParseManifestAsync()
        {
            var file = await Package.Current.InstalledLocation.GetFileAsync("AppxManifest.xml");
            using (var randomAccessStream = await file.OpenAsync(FileAccessMode.Read))
            {
                using (var stream = randomAccessStream.AsStreamForRead())
                {
                    var xmlSerializer = new XmlSerializer(typeof(PackageNode));
                    var package = (PackageNode)xmlSerializer.Deserialize(stream);
                    this.ApplyDataFromManifest(package);
                }
            }
        }

        private void ApplyDataFromManifest(PackageNode package)
        {
            if (package != null)
            {
                if (package.Capabilities != null)
                {
                    foreach (var capability in package.Capabilities)
                    {
                        switch (capability.Name)
                        {
                            case "internetClientServer":
                                this.Capabilities.AllowNetworkAccess = true;
                                break;

                            case "internetClient":
                                this.Capabilities.AllowNetworkAccess = true;
                                break;

                            case "location":
                                this.Capabilities.AllowAccessToLocation = true;
                                break;
#if WINRT_TABLET
                            case "picturesLibrary":
                                this.Capabilities.AllowPhotoMediaLibrary = true;
                                break;
#endif
                        }
                    }
                }
            }

            this.Capabilities.AllowWebBrowserControl = true;
#if WINRT_PHONE
            this.Capabilities.AllowCallsAndSms = true;
            this.Capabilities.AllowPhotoMediaLibrary = true;
#endif

            //Windows.ApplicationModel.Calls.PhoneCallManager.ShowPhoneCallUI( ChatMessageManager.ShowComposeSmsMessageAsync(chat);
        }
    }
}
