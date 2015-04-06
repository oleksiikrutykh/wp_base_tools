namespace BaseTools.Core.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Contains info about location changes.
    /// </summary>
    public class GeoPositionChangedEventArgs : EventArgs
    {
        public GeoPosition Position { get; set; }
    }
}
