namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.Geolocation;
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class AnalyticsParameter
    {
        public string Key { get; set; }

        public object Value { get; set; }
    }

    public class AnalyticsParametersCollection : List<AnalyticsParameter>
    {
        public void Add(string key, object value)
        {
            var newParameter = new AnalyticsParameter
            {
                Key = key,
                Value = value
            };

            this.Add(newParameter);
        }
    }

    public class AnalyticsProvider
    {
        private static AnalyticsProvider instance;

        public string Key { get; set; }

        public static AnalyticsProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Factory.Common.GetInstance<AnalyticsProvider>();
                }

                return instance;
            }
        }

        public virtual void StartSession()
        { 
        }

        public virtual void EndSession()
        { 
        }

        public virtual void NewPageView(string navigatedSource)
        { 
        }

        public void SendEvent(string eventName)
        {
            this.SendEvent(eventName, new AnalyticsParametersCollection());
        }

        public void SendEvent(string eventName, object value)
        {
            if (value != null)
            {
                var args = new AnalyticsParametersCollection();
                args.Add(eventName, value);
                this.SendEvent(eventName, args);
            }
            else
            {
                this.SendEvent(eventName, new AnalyticsParametersCollection());
            }
        }

        public virtual void SendEvent(string eventName, AnalyticsParametersCollection parameters)
        { 
        }

        public virtual void SendError(string message, Exception ex)
        { 
        }

        public virtual void SetCurrentLocation(GeoCoordinate location)
        {
            if (location != null)
            {
                var args = new AnalyticsParametersCollection();
                args.Add("Latitude", location.Latitude);
                args.Add("Longitude", location.Longitude);
                this.SendEvent("CurrentLocation", args);
            }
        }

        public virtual void SetUserName(string userName)
        { 
        }
    }
}
