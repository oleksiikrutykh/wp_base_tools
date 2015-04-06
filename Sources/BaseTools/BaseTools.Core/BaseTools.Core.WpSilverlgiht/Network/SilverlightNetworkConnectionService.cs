namespace BaseTools.Core.Network
{
    using Microsoft.Phone.Net.NetworkInformation;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Networking.Connectivity;

    /// <summary>
    /// Provides information about existed network connection. Can be used in Windows Phone Silverlight apps.
    /// </summary>
    internal class SilverlightNetworkConnectionService : NetworkConnectionService
    {

        public SilverlightNetworkConnectionService()
        {
            DeviceNetworkInformation.NetworkAvailabilityChanged += OnNetworkAvailabilityChanged;
        }

        public bool IsWiFiEnabled
        {
            get
            {
                return DeviceNetworkInformation.IsWiFiEnabled;
            }
        }

        private void OnNetworkAvailabilityChanged(object sender, NetworkNotificationEventArgs e)
        {
            this.InvokeConnectionChanged();
        }

        public override async Task<bool> IsNetworkAvailable()
        {
            var result = false;
            using (var cancellationTaskSource = new CancellationTokenSource())
            {
                var networkCheckTask = Task.Factory.StartNew<bool>(this.GetIsNetworkAvailable, cancellationTaskSource.Token);
                // Wait 5 seconds, then cancel operation timeout.
                var timeoutTask = Task.Delay(5000);
                var completedTask = await Task.WhenAny(networkCheckTask, timeoutTask);
                if (completedTask == networkCheckTask)
                {
                    result = await networkCheckTask;
                }
                else
                {
                    cancellationTaskSource.Cancel();
                }
            }

            return result;
        }

        private bool GetIsNetworkAvailable()
        {
            var isNetworkAvailable = NetworkInterface.GetIsNetworkAvailable() &&
                                    (NetworkInterface.NetworkInterfaceType != NetworkInterfaceType.None);
            return isNetworkAvailable;
        }

        public override Task<string> DetermineClientIpAddress()
        {
            return Task.Run(() =>
            {
                var hostNames = NetworkInformation.GetHostNames();
                string ipAddress = null;
                foreach (var host in hostNames)
                {
                    if (host.IPInformation != null)
                    {
                        ipAddress = host.DisplayName;
                        break;
                    }
                }

                return ipAddress;
            });
        }
    }
}
