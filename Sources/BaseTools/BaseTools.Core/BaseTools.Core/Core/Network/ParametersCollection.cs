namespace BaseTools.Core.Network
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using System.Globalization;

    public class ParametersCollection : List<RequestParameter>
    {
        public void AddNotDefaultParameter<T>(string key, T value)
        {
            bool isParameterEmpty = true;
            var stringValue = value as string;
            if (stringValue != null)
            {
                isParameterEmpty = stringValue == String.Empty;
            }
            else
            {
                isParameterEmpty = Object.Equals(value, default(T));
            }

            if (!isParameterEmpty)
            {
                this.AddParameter(key, value);
            }
        }

        public void AddNotDefaultParameter(string key, object value)
        {
            bool isParameterEmpty = true;
            if (value != null)
            {
                var stringValue = value as string;
                if (stringValue != null)
                {
                    isParameterEmpty = stringValue == String.Empty;
                }
                else
                {
                    var type = value.GetType();
                    var typeInfo = type.GetTypeInfo();
                    bool isValueType = typeInfo.IsValueType;
                    if (isValueType)
                    {
                        var defaultValue = Activator.CreateInstance(type);
                        isParameterEmpty = Object.Equals(value, defaultValue);
                    }
                    else
                    {
                        isParameterEmpty = false;
                    }
                }
            }

            if (!isParameterEmpty)
            {
                this.AddParameter(key, value);
            }
        }

        public void AddParameter(string key, object value)
        {
            this.AddParameter(key, value, true);
        }

        public void AddParameter(string key, object value, bool allowsUriEscape)
        {
            var stringValue = Convert.ToString(value, CultureInfo.InvariantCulture);
            var parameter = new RequestParameter
            {
                Key = key,
                Value = stringValue,
                NeedEscapeValue = allowsUriEscape
            };

            this.Add(parameter);
        }
    }
}
