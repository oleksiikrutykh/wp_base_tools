namespace BaseTools.Core.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to device location. 
    /// </summary>
    public interface IGeoCoordinateWatcher : IDisposable
    {
        Task<GeoCoordinate> TakeGeoCoordinateAsync();

        bool? IsAccessAllowed { get; set; }

        Task<WatcherState> StartAsync();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1716:IdentifiersShouldNotMatchKeywords", MessageId = "Stop", Justification = "Use same name as real implementation.")]
        void Stop();

        event EventHandler<GeoPositionChangedEventArgs> PositionChanged;
    }
}
