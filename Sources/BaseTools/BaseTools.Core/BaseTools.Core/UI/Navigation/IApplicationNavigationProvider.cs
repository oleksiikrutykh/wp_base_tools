namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;

    public interface IApplicationNavigationProvider
    {
        object CurrentSource { get; }

        object Parameters { get; }

        NavigationEntry CurrentEntry { get;  }

        NavigationEntry TargetEntry { get; }

        IReadOnlyList<NavigationEntry> NavigationStack { get; }

        int BackStackDepth { get; }

        bool CanGoBack { get; }

        event EventHandler<NavigationProviderEventArgs> Navigated;

        event EventHandler<NavigationProviderCancellableEventArgs> Navigating;

        event NavigationProviderEventHandler NavigationCanceled;

        void GoBack();

        void Navigate(object navigationSource, object parameters);

        void Navigate(object navigationSource);

        void RemoveBackEntry();

        void RestoreHistory();

        void SaveHistory();

        event EventHandler<NavigationEntryRemovedEventArgs> BackEntryRemoved;
    }
}