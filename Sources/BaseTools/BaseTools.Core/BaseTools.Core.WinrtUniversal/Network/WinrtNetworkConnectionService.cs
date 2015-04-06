namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.Networking.Connectivity;

    /// <summary>
    /// Provides information about existed network connection. 
    /// </summary>
    internal class WinrtNetworkConnectionService : NetworkConnectionService
    {
        private bool isNetworkAvaliable;

        public WinrtNetworkConnectionService()
        {
            NetworkInformation.NetworkStatusChanged += OnNetworkStatusChanged;
            DetermineConnection();
        }

        private void DetermineConnection()
        {
            var newValue = false;
            var networkProfile = NetworkInformation.GetInternetConnectionProfile();
            if (networkProfile != null)
            {
                var connectivityLevel = networkProfile.GetNetworkConnectivityLevel();
                if (connectivityLevel == Windows.Networking.Connectivity.NetworkConnectivityLevel.InternetAccess)
                {
                    newValue = true;
                }
            }

            this.isNetworkAvaliable = newValue;
        }

        private void OnNetworkStatusChanged(object sender)
        {
            this.DetermineConnection();
            this.InvokeConnectionChanged();
        }

        public override Task<bool> IsNetworkAvailable()
        {
            return Task.FromResult(this.isNetworkAvaliable);
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
