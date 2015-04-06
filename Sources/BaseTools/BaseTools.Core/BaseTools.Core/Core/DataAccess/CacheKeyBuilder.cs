namespace BaseTools.Core.DataAccess
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class CacheKeyBuilder
    {
        private StringBuilder builder = new StringBuilder();

        public CacheKeyBuilder()
        {
 
        }

        public CacheKeyBuilder(string data)
        {
            this.builder.Append(data);
        }

        public string Value
        {
            get
            {
                return this.builder.ToString();
            }
        }

        /// <summary>
        /// Add to cache key new parameter.
        /// </summary>
        /// <param name="parameterName">The name of added parameter</param>
        /// <param name="parameterValue">The value of added parameter</param>
        public void AddParameterToCacheKey(string parameterName, object parameterValue)
        {
            // convert value to string.
            string stringValue = null;
            if (parameterValue != null)
            {
                stringValue = parameterValue.ToString();
            }

            // Escape value.
            if (!String.IsNullOrEmpty(stringValue))
            {
                stringValue = Uri.EscapeDataString(stringValue);
            }

            // Append prepeared data to cache key.
            this.builder.Append("_");
            this.builder.Append(parameterName);
            this.builder.Append("_");
            this.builder.Append(stringValue);
        }

        ///// <summary>
        ///// Convert location data to cache string
        ///// </summary>
        ///// <param name="degree">Location coordinate value.</param>
        ///// <returns>Cache key text.</returns>
        //private string DegreeToString(double? degree)
        //{
        //    string result = string.Empty;
        //    if (degree.HasValue)
        //    {
        //        var roundedDegree = Math.Round(degree.Value, 2);
        //        result = roundedDegree.ToString(CultureInfo.InvariantCulture);
        //    }
        //}

          
    }
}
