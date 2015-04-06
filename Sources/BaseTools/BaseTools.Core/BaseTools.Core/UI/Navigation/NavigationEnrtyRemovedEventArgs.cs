namespace BaseTools.UI.Navigation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class NavigationEntryRemovedEventArgs : EventArgs
    {
        public NavigationEntryRemovedEventArgs(NavigationEntry removedEntry)
        {
            this.RemovedEntry = removedEntry;
        }

        public NavigationEntry RemovedEntry { get; private set; }
    }
}
