namespace BaseTools.Core.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Represents a rectangle area on map. 
    /// </summary>
    public class GeoRect
    {
        public double North { get; set; }

        public double South { get; set; }

        public double West { get; set; }

        public double East { get; set; }

        private const int Ninety = 90;

        private const int MinusNinety = -90;

        private const int OneHundredEihgty = 180;

        private const int MinusOneHundredEihgty = -180;



        public GeoRect(IEnumerable<GeoCoordinate> placedPoints)
        {   
            this.East = placedPoints.Max(p => p.Longitude);
            this.North = placedPoints.Max(p => p.Latitude);
            this.South = placedPoints.Min(p => p.Latitude);
            this.West = placedPoints.Min(p => p.Longitude);
            this.FormatBounds();
        }

        public GeoRect(GeoCoordinate center, IEnumerable<GeoCoordinate> placedPoints)
        {
            double maxLatitudeChange = 0;
            double maxLogitudeChange = 0;

            if (placedPoints != null && placedPoints.Count() > 0)
            {
                maxLatitudeChange = placedPoints.Select(g => Math.Abs(g.Latitude - center.Latitude)).Max();
                maxLogitudeChange = placedPoints.Select(g => Math.Abs(g.Longitude - center.Longitude)).Max();
            }

            this.North = center.Latitude + maxLatitudeChange;
            this.South = center.Latitude - maxLatitudeChange;
            this.West = center.Longitude - maxLogitudeChange;
            this.East = center.Longitude + maxLogitudeChange;
            this.FormatBounds();

        }

        private void FormatBounds()
        {
            if (this.North > Ninety)
            {
                this.North = Ninety;
            }

            if (this.South < MinusNinety)
            {
                this.South = MinusNinety;
            }

            if (this.West < MinusOneHundredEihgty)
            {
                this.West = MinusOneHundredEihgty;
            }

            if (this.East > OneHundredEihgty)
            {
                this.East = OneHundredEihgty;
            }            
        }
       
    }
}
