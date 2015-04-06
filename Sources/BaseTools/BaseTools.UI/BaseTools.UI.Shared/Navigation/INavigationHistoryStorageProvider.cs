namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;

    public interface INavigationHistoryStorageProvider
    {
        IList<Type> KnownTypes { get; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        List<NavigationEntry> RestoreHistory();

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1002:DoNotExposeGenericLists")]
        void StoreHistory(List<NavigationEntry> navigationStack);
    }
}
