namespace BaseTools.UI.Navigation
{
    using System;

    public class PathNavigationMapItem
    {
        public PathNavigationMapItem(string pagePath, object source, Type parameterType)
        {
            this.Source = source;
            this.PagePath = pagePath;
            this.ParameterType = parameterType;
        }

        public object Source { get; private set; }

        public string PagePath { get; private set; }

        public Type ParameterType { get; private set; }
    }

    public class TypeNavigationMapItem
    {
        public TypeNavigationMapItem(Type pageType, object source, Type parameterType)
        {
            this.Source = source;
            this.PageType = pageType;
            this.ParameterType = parameterType;
        }

        public object Source { get; private set; }

        public Type PageType { get; private set; }

        public Type ParameterType { get; private set; }
    }
}
