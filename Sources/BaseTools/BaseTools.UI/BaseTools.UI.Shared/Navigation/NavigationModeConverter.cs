namespace BaseTools.UI.Navigation
{
#if SILVERLIGHT
    using Platform = System.Windows.Navigation;
#endif

#if WINRT
    using Platform = Windows.UI.Xaml.Navigation;
#endif

    internal static class NavigationModeConverter
    {
        internal static NavigationMode ConvertFromSystem(Platform.NavigationMode source)
        {
            var result = (NavigationMode)source;
            return result;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Justification = "Converter have to implement both methods.")]
        internal static Platform.NavigationMode ConvertToSystem(NavigationMode source)
        {
            var result = (Platform.NavigationMode)source;
            return result;
        }
    }
}
