namespace BaseTools.UI.Navigation
{
    using System;

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "Delegate.")]
    public delegate void NavigationProviderEventHandler(object sender, NavigationProviderEventArgs e);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class NavigationProviderEventArgs : EventArgs
    {
        public NavigationProviderEventArgs(NavigationEntry fromEntry, NavigationEntry toEntry, NavigationMode mode)
        {
            this.FromEntry = fromEntry;
            this.ToEntry = toEntry;
            this.NavigationMode = mode;
        }

        public NavigationEntry FromEntry { get; private set; }

        public NavigationEntry ToEntry { get; private set; }

        public NavigationMode NavigationMode { get; private set; }
    }
}
