namespace BaseTools.UI.Navigation
{
    using System;

    public class NavigationMapItem
    {
#if SILVERLIGHT
        public NavigationMapItem(string pagePath, object source, Type parameterType)
        {
            this.Source = source;
            this.PagePath = pagePath;
            this.ParameterType = parameterType;
        }

        public object Source { get; private set; }

        public string PagePath { get; private set; }

        public Type ParameterType { get; private set; }
#endif

#if WINRT
        public NavigationMapItem(Type pageType, object source, Type parameterType)
        {
            this.Source = source;
            this.PageType = pageType;
            this.ParameterType = parameterType;
        }

        public object Source { get; private set; }

        public Type PageType { get; private set; }

        public Type ParameterType { get; private set; }
#endif

    }
}
