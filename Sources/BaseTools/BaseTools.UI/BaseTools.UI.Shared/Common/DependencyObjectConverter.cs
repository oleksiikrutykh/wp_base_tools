namespace BaseTools.UI.Common
{
#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Data;
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Data;
#endif

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Globalization;
    

    /// <summary>
    /// Provides method implementation of <see cref="IValueConverter"/>
    /// </summary>
    public class DependencyObjectConverter : DependencyObject, IValueConverter
    {
#if SILVERLIGHT

        object IValueConverter.Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.Convert(value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return this.ConvertBack(value, targetType, parameter, culture);
        }

#endif


#if WINRT

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            var culture = new CultureInfo(language);
            return this.Convert(value, targetType, parameter, culture);
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            var culture = new CultureInfo(language);
            return this.ConvertBack(value, targetType, parameter, culture);
        }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "OneWay", Justification = "Name of enum")]
        protected virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converter not support OneWay bindings");
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "TwoWay", Justification = "Name of enum")]
        protected virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Converter not support TwoWay bindings");
        }
    }
}
