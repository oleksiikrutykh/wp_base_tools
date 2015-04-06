namespace BaseTools.Core.Geolocation
{
    using BaseTools.Core.Math;
    using BaseTools.Core.Utility;
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Represents a location expressed as a geographic coordinate. The values exposed by this class use the WGS 84 standard.
    /// </summary>
    [DataContract]
    public class GeoCoordinate
    {
        public const int EarthRadius = 6371009;

        public GeoCoordinate()
        {
        }

        public GeoCoordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        /// <summary>
        /// The altitude of the location, in meters.
        /// </summary>
        /// <value>The altitude.</value>
        public double Altitude { get; set; }


        /// <summary>
        /// Gets or sets the latitude of the GeoCoordinate
        /// </summary>
        /// <value>The latitude.</value>
        [DataMember]
        public double Latitude { get; set; }


        /// <summary>
        /// Gets or sets the longitude of the GeoCoordinate
        /// </summary>
        /// <value>The longitude.</value>
        [DataMember]
        public double Longitude { get; set; }


        /// <summary>
        /// Gets the speed of the GeoCoordinate in meters per second.
        /// </summary>
        /// <value>The speed.</value>
        public double Speed { get; set; }

        /// <summary>
        /// Gets or sets the heading in degrees, relative to true north. GeoCoordinate. In Windows 8 called Heading property.
        /// </summary>
        /// <value>The course.</value>
        public double Course { get; set; }

        public double Accuracy { get; set; }

        public double GetDistanceTo(GeoCoordinate other)
        {
            return this.GetDistanceTo(other, DistanceMeasureUnit.Meter);
        }

        public double GetDistanceTo(double latitude, double longitude)
        {
            return this.GetDistanceTo(latitude, longitude, DistanceMeasureUnit.Meter);
        }

        public double GetDistanceTo(double latitude, double longitude, DistanceMeasureUnit measureUnit)
        {
            var measuredPoint = new GeoCoordinate(latitude, longitude);
            return this.GetDistanceTo(measuredPoint, measureUnit);
        }

        public double GetDistanceTo(GeoCoordinate other, DistanceMeasureUnit measureUnit)
        {
            Guard.CheckIsNotNull(other, "other");
            double result = 0d;
            
            // Distance calculating: http://www.movable-type.co.uk/scripts/latlong.html
            var dLat = AngleMeasurementConverter.DegreeToRadian(other.Latitude - this.Latitude); 
            var dLon = AngleMeasurementConverter.DegreeToRadian(other.Longitude - this.Longitude);
            var radianLatitude1 = AngleMeasurementConverter.DegreeToRadian(this.Latitude);
            var radianLatitude2 = AngleMeasurementConverter.DegreeToRadian(other.Latitude);

            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                    Math.Cos(radianLatitude1) * Math.Cos(radianLatitude2) *
                    Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            var metersDistance = EarthRadius * c;
            result = DistanceConverter.ConvertFromMeters(metersDistance, measureUnit);

            return result;
        }
    }
}
