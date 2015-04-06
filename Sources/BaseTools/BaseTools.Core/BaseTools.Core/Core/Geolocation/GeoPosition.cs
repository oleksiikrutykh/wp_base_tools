namespace BaseTools.Core.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a geographic position consisting of a location and a time stamp.
    /// </summary>
    public class GeoPosition
    {
        /// <summary>
        /// Gets or sets the location of the GeoPosition object.
        /// </summary>
        /// <value>The location.</value>
        public GeoCoordinate Location { get; set; }

        /// <summary>
        /// Gets or sets the time stamp of the GeoPosition object.
        /// </summary>
        /// <value>The time stamp.</value>
        public DateTimeOffset Timestamp { get; set; }

        public GeoPosition()
        {
            this.Location = new GeoCoordinate();
            this.Timestamp = DateTimeOffset.Now;
        }
    }
}
