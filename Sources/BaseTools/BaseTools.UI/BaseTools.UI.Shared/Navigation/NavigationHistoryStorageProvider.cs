namespace BaseTools.UI.Navigation
{
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Serialization;
    using BaseTools.Core.Storage;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class NavigationHistoryStorageProvider : INavigationHistoryStorageProvider
    {
        private const string NavigationStackStorageKey = "NavigationProviderStack";

        private const string KnownTypesStorageKey = "NavigationProviderStackKnownTypes";

        private static readonly IStorageProvider StorageProvider = Factory.Common.GetInstance<IStorageProvider>();

        private List<Type> knownTypes = new List<Type>();

        public IList<Type> KnownTypes 
        {
            get
            {
                return this.knownTypes;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Not reusable library.")]
        public List<NavigationEntry> RestoreHistory()
        {
            var task = RestoreHistoryAsync();
            task.Wait();
            return task.Result;
        }

        private Task<List<NavigationEntry>> RestoreHistoryAsync()
        {
            return Task.Run(async () =>
            {
                var result = new List<NavigationEntry>();
                //var storedHistory = await StorageProvider.ReadFromFileAsync<List<NavigationEntry>>(NavigationStackStorageKey);
                //if (storedHistory != null)
                //{
                //    result = storedHistory;
                //}

                var typesSerializer = new DataContractXmlSerializer();
                var knownTypeNames = await StorageProvider.ReadFromFileAsync<List<string>>(KnownTypesStorageKey, typesSerializer);

                if (knownTypeNames != null)
                {
                    var serializer = new DataContractXmlSerializer();
                    foreach (var typeName in knownTypeNames)
                    {
                        var type = Type.GetType(typeName);
                        serializer.KnownTypes.Add(type);
                    }

                    this.MergeWithExternalKnownTypes(serializer);
                    var storedHistory = await StorageProvider.ReadFromFileAsync<List<NavigationEntry>>(NavigationStackStorageKey, serializer);
                    if (storedHistory != null)
                    {
                        result = storedHistory;
                    }
                }

                return result;
            });
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists", Justification = "Not reusable library.")]
        public void StoreHistory(List<NavigationEntry> navigationStack)
        {
            var task = StoreHistoryAsync(navigationStack);
            task.Wait();
        }

        private Task StoreHistoryAsync(List<NavigationEntry> navigationStack)
        {
            return Task.Run(async () =>
            {
                //await StorageProvider.WriteToFileAsync(NavigationStackStorageKey, navigationStack);
                var serializer = new DataContractXmlSerializer();
                foreach (var item in navigationStack)
                {
                    AddToSerialzierKnownTypes(serializer.KnownTypes, item);
                    AddToSerialzierKnownTypes(serializer.KnownTypes, item.Source);
                    AddToSerialzierKnownTypes(serializer.KnownTypes, item.Parameter);
                }

                var storedTypes = serializer.KnownTypes.Select(t => t.AssemblyQualifiedName).ToArray();

                this.MergeWithExternalKnownTypes(serializer);
                await StorageProvider.WriteToFileAsync(NavigationStackStorageKey, navigationStack, serializer);

                var typesSerializer = new DataContractXmlSerializer();
                await StorageProvider.WriteToFileAsync(KnownTypesStorageKey, storedTypes, typesSerializer);
            });
        }

        private static void AddToSerialzierKnownTypes(List<Type> knownTypes, object checkedValue)
        {
            if (checkedValue != null)
            {
                var type = checkedValue.GetType();
                var typeAlreadyExisted = knownTypes.Contains(type);
                if (!typeAlreadyExisted)
                {
                    knownTypes.Add(type);
                }
            }
        }

        private void MergeWithExternalKnownTypes(DataContractXmlSerializer serialzier)
        {
            foreach (var type in this.knownTypes)
            {
                AddToSerialzierKnownTypes(serialzier.KnownTypes, type);
            }
        }
    }
}
