namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;

    public class PathNavigationMapper
    {
        private Dictionary<object, PathNavigationMapItem> sourceToRealMapping = new Dictionary<object, PathNavigationMapItem>();

        private Dictionary<string, PathNavigationMapItem> realToSourceMapping = new Dictionary<string, PathNavigationMapItem>();

        public PathNavigationMapper()
        {
            this.Mappings = new List<PathNavigationMapItem>();
        }

        public IList<PathNavigationMapItem> Mappings { get; private set; }

        public void AddMapping(string pageRealPath, object mappedSource)
        {
            this.AddMapping(pageRealPath, mappedSource, null);
        }

        public void AddMapping(string pageRealPath, object mappedSource, Type parameterType)
        {
            var mapItem = new PathNavigationMapItem(pageRealPath, mappedSource, parameterType);
            this.Mappings.Add(mapItem);
            this.sourceToRealMapping.Add(mapItem.Source, mapItem);
            this.realToSourceMapping.Add(mapItem.PagePath, mapItem);
        }

        public PathNavigationMapItem GetMapping(string pagePath)
        {
            PathNavigationMapItem mappingItem = null;
            this.realToSourceMapping.TryGetValue(pagePath, out mappingItem);
            return mappingItem;
        }

        public PathNavigationMapItem GetMapping(object source)
        {
            PathNavigationMapItem mappingItem = null;
            this.sourceToRealMapping.TryGetValue(source, out mappingItem);
            return mappingItem;
        }
    }

    public class TypeNavigationMapper
    {
        private Dictionary<object, TypeNavigationMapItem> sourceToRealMapping = new Dictionary<object, TypeNavigationMapItem>();

        private Dictionary<Type, TypeNavigationMapItem> realToSourceMapping = new Dictionary<Type, TypeNavigationMapItem>();

        public TypeNavigationMapper()
        {
            this.Mappings = new List<TypeNavigationMapItem>();
        }

        public IList<TypeNavigationMapItem> Mappings { get; private set; }

        public void AddMapping(Type pageType, object mappedSource)
        {
            this.AddMapping(pageType, mappedSource, null);
        }

        private void AddMapping(Type pageType, object mappedSource, Type parameterType)
        {
            var mapItem = new TypeNavigationMapItem(pageType, mappedSource, parameterType);
            this.Mappings.Add(mapItem);
            this.sourceToRealMapping.Add(mapItem.Source, mapItem);
            this.realToSourceMapping.Add(mapItem.PageType, mapItem);
        }

        public TypeNavigationMapItem GetMapping(Type pageType)
        {
            TypeNavigationMapItem mappingItem = null;
            this.realToSourceMapping.TryGetValue(pageType, out mappingItem);
            return mappingItem;
        }

        public TypeNavigationMapItem GetMapping(object source)
        {
            TypeNavigationMapItem mappingItem = null;
            this.sourceToRealMapping.TryGetValue(source, out mappingItem);
            return mappingItem;
        }
    }
}
