namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;

    public class NavigationMapper
    {
        private Dictionary<object, NavigationMapItem> sourceToRealMapping = new Dictionary<object, NavigationMapItem>();

#if SILVERLIGHT
        private Dictionary<string, NavigationMapItem> realToSourceMapping = new Dictionary<string, NavigationMapItem>();
#endif

#if WINRT
        private Dictionary<Type, NavigationMapItem> realToSourceMapping = new Dictionary<Type, NavigationMapItem>();
#endif

        public NavigationMapper()
        {
            this.Mappings = new List<NavigationMapItem>();
        }

        public IList<NavigationMapItem> Mappings { get; private set; }

#if SILVERLIGHT
        public void AddMapping(string pageRealPath, object mappedSource)
        {
            this.AddMapping(pageRealPath, mappedSource, null);
        }

        public void AddMapping(string pageRealPath, object mappedSource, Type parameterType)
        {
            var mapItem = new NavigationMapItem(pageRealPath, mappedSource, parameterType);
            this.Mappings.Add(mapItem);
            this.sourceToRealMapping.Add(mapItem.Source, mapItem);
            this.realToSourceMapping.Add(mapItem.PagePath, mapItem);
        }

        public NavigationMapItem GetMapping(string pagePath)
        {
            NavigationMapItem mappingItem = null;
            this.realToSourceMapping.TryGetValue(pagePath, out mappingItem);
            return mappingItem;
        }
#endif
#if WINRT
        public void AddMapping(Type pageType, object mappedSource)
        {
            this.AddMapping(pageType, mappedSource, null);
        }

        private void AddMapping(Type pageType, object mappedSource, Type parameterType)
        {
            var mapItem = new NavigationMapItem(pageType, mappedSource, parameterType);
            this.Mappings.Add(mapItem);
            this.sourceToRealMapping.Add(mapItem.Source, mapItem);
            this.realToSourceMapping.Add(mapItem.PageType, mapItem);
        }

        public NavigationMapItem GetMapping(Type pageType)
        {
            NavigationMapItem mappingItem = null;
            this.realToSourceMapping.TryGetValue(pageType, out mappingItem);
            return mappingItem;
        }
#endif

        public NavigationMapItem GetMapping(object source)
        {
            NavigationMapItem mappingItem = null;
            this.sourceToRealMapping.TryGetValue(source, out mappingItem);
            return mappingItem;
        }
    }
}
