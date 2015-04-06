namespace BaseTools.UI.Navigation
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances", Justification = "Delegate.")]
    public delegate void NavigationProviderCancellableEventHandler(object sender, NavigationProviderCancellableEventArgs e);

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix")]
    public class NavigationProviderCancellableEventArgs : NavigationProviderEventArgs
    {
        public NavigationProviderCancellableEventArgs(NavigationEntry fromEntry, NavigationEntry toEntry, NavigationMode mode)
            : base(fromEntry, toEntry, mode)
        {          
        }

        public bool IsCanceled { get; set; }
    }
}
