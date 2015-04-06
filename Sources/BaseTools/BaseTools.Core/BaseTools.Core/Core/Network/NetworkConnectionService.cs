namespace BaseTools.Core.Network
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    

    /// <summary>
    /// Provides information about existed network connection.
    /// </summary>
    public abstract class NetworkConnectionService
    {
        public event EventHandler<ConnectionChangedEventArgs> ConnectionChanged;

        public abstract Task<bool> IsNetworkAvailable();

        protected void InvokeConnectionChanged()
        {
            var args = new ConnectionChangedEventArgs();
            EventHelper.CallEvent(this.ConnectionChanged, this, args);
        }

        public abstract Task<string> DetermineClientIpAddress();
        

        //using Windows.Networking.Connectivity;

        //{
        //    return Task.Run(() =>
        //    {
        //        var hostNames = NetworkInformation.GetHostNames();
        //        string ipAddress = null;
        //        foreach (var host in hostNames)
        //        {
        //            if (host.IPInformation != null)
        //            {
        //                ipAddress = host.DisplayName;
        //                break;
        //            }
        //        }

        //        return ipAddress;
        //    });
        //}
    }

    
}
