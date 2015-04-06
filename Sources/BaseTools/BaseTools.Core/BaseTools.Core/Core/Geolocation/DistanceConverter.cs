namespace BaseTools.Core.Geolocation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Converts a distance to different measure systems.
    /// </summary>
    public static class DistanceConverter
    {
        private const double MilesInMeter = 0.000621371192237;

        private const double KilometersInMeter = 0.001;

        public static double Convert(double value, DistanceMeasureUnit originalUnit, DistanceMeasureUnit resultUnit)
        {
            var meterValue = ConvertToMeters(value, originalUnit);
            var result = ConvertFromMeters(meterValue, resultUnit);
            return result;
        }

        public static double ConvertFromMeters(double value, DistanceMeasureUnit resultUnit)
        {
            double result = value;
            switch (resultUnit)
            {
                case DistanceMeasureUnit.Miles:
                    result = value * MilesInMeter;
                    break;

                case DistanceMeasureUnit.Kilometers:
                    result = value * KilometersInMeter;
                    break;
            }

            return result;
        }

        public static double ConvertToMeters(double value, DistanceMeasureUnit originalUnit)
        {
            double result = value;
            switch (originalUnit)
            {
                case DistanceMeasureUnit.Miles:
                    result = value / MilesInMeter;
                    break;

                case DistanceMeasureUnit.Kilometers:
                    result = value / KilometersInMeter;
                    break;
            }

            return result;
        }
    }
}
