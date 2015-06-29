namespace BaseTools.UI.ControlExtensions
{
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Models.WeakReferences;
    using BaseTools.UI.Navigation;
    using BaseTools.UI.Common;
    using Microsoft.Phone.Controls;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    public enum ControlLifetimeState
    {
        Unknown,
        Active,
        Unactive
    }

    public class ControlLifetimeListener
    {
        private static ControlLifetimeListener instance = new ControlLifetimeListener();

        private WeakKeyDictionary<Page, List<WeakReference<FrameworkElement>>> pageDictionary = new WeakKeyDictionary<Page, List<WeakReference<FrameworkElement>>>();

        private WeakDictionary<FrameworkElement, Page> controlsDictionary = new WeakDictionary<FrameworkElement, Page>();

        private WeakKeyDictionary<FrameworkElement, ControlLifetimeState> controlStates = new WeakKeyDictionary<FrameworkElement, ControlLifetimeState>();

        private IApplicationNavigationProvider navigationProvider;

        public ControlLifetimeListener()
        {
            this.navigationProvider = Factory.Common.GetInstance<IApplicationNavigationProvider>();
            navigationProvider.Navigated += OnNavigated;
            navigationProvider.BackEntryRemoved += OnPageRemoved;
            this.IsTrackAllowed = true;
        }

        public static ControlLifetimeListener Instance
        {
            get
            {
                return instance;
            }
        }
        

        /// <summary>
        /// Gets or sets value indicates whether ControlLifetimeListener works. If False - ControlLifetimeListener not works. 
        /// Used for backwork compatibility for applications that use IApplicationNavigationProvider.
        /// </summary>
        public bool IsTrackAllowed { get; set; }

        public event EventHandler<ControlRemovedEventArgs> ControlDeactivate;

        public event EventHandler<ControlRestoredEventArgs> ControlRestored;

        public void TrackControl(FrameworkElement element)
        {
            if (this.IsTrackAllowed)
            {
                Page info = null;
                var founded = this.controlsDictionary.TryGetValue(element, out info);
                if (!founded)
                {
                    var page = DetermineContainerPage(element);
                    this.controlsDictionary[element] = page;
                    if (page != null)
                    {
                        List<WeakReference<FrameworkElement>> pageChilds = null;
                        var pageFoudned = this.pageDictionary.TryGetValue(page, out pageChilds);
                        if (!pageFoudned)
                        {
                            pageChilds = new List<WeakReference<FrameworkElement>>();
                            this.pageDictionary[page] = pageChilds;
                        }

                        var controlReference = new WeakReference<FrameworkElement>(element);
                        pageChilds.Add(controlReference);
                    }


                    element.Loaded -= OnFrameworkElementLoaded;
                    element.Loaded += OnFrameworkElementLoaded;
                    this.controlStates[element] = ControlLifetimeState.Active;
                }
            }
        }

        public ControlLifetimeState DetermineControlState(FrameworkElement element)
        {
            ControlLifetimeState state = ControlLifetimeState.Unknown;
            this.controlStates.TryGetValue(element, out state);
            return state;
        }

        private void OnNavigated(object sender, NavigationProviderEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Back)
            {
                var removedPage = e.FromEntry.Content as Page;
                if (removedPage != null)
                {
                    this.OnPageUnactive(removedPage);
                }
            }
        }

        private void OnPageRemoved(object sender, NavigationEntryRemovedEventArgs e)
        {
            var page = e.RemovedEntry.Content as Page;
            if (page != null)
            {
                this.OnPageUnactive(page);
            }
        }

        private void OnPageUnactive(Page removedPage)
        {
            List<WeakReference<FrameworkElement>> childsCollection = null;
            var associationExist = this.pageDictionary.TryGetValue(removedPage, out childsCollection);
            if (associationExist)
            {
                foreach (var childReference in childsCollection)
                {
                    FrameworkElement child = null;
                    var isAlive = childReference.TryGetTarget(out child);
                    if (isAlive)
                    {
                        var handler = this.ControlDeactivate;
                        if (handler != null)
                        {
                            var args = new ControlRemovedEventArgs(child);
                            handler.Invoke(this, args);
                        }

                        this.controlsDictionary.Remove(child);
                        this.controlStates[child] = ControlLifetimeState.Unactive;
                    }
                }

                this.pageDictionary.Remove(removedPage);
            }

            this.controlsDictionary.RemoveCollectedEntries();
            this.controlStates.RemoveCollectedEntries();
            this.pageDictionary.RemoveCollectedEntries();
        }

        private Page DetermineContainerPage(FrameworkElement element)
        {
            Page rootPage = null;
            var visualAncestors = element.GetVisualAncestors();
            foreach (var ancestor in visualAncestors)
            {
                if (this.navigationProvider.RootUIElement == ancestor)
                {
                    break;
                }

                var page = ancestor as Page;
                if (page != null)
                {
                    rootPage = page;
                }
            }

            return rootPage;
        }

        private void OnFrameworkElementLoaded(object sender, RoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            Page previousPage = null;
            var founded = this.controlsDictionary.TryGetValue(element, out previousPage);
            if (!founded)
            {
                var handler = this.ControlRestored;
                if (handler != null)
                {
                    var args = new ControlRestoredEventArgs(element);
                    handler.Invoke(this, args);
                }
            }

            // Update page reference.
            var currentPage = DetermineContainerPage(element);
            if (currentPage != previousPage)
            {
                if (previousPage != null)
                {
                    List<WeakReference<FrameworkElement>> previousPageChilds = null;


                    var previousPageFounded = this.pageDictionary.TryGetValue(previousPage, out previousPageChilds);
                    if (previousPageFounded)
                    {
                        for (int i = 0; i < previousPageChilds.Count; i++)
                        {
                            var childReference = previousPageChilds[i];
                            FrameworkElement child = null;
                            childReference.TryGetTarget(out child);
                            if (element != child)
                            {
                                previousPageChilds.RemoveAt(i);
                                break;
                            }
                        }
                    }
                }

                this.controlsDictionary[element] = currentPage;

                if (currentPage != null)
                {
                    List<WeakReference<FrameworkElement>> pageChilds = null;
                    var pageFoudned = this.pageDictionary.TryGetValue(currentPage, out pageChilds);
                    if (!pageFoudned)
                    {
                        pageChilds = new List<WeakReference<FrameworkElement>>();
                        this.pageDictionary[currentPage] = pageChilds;
                    }

                    var controlReference = new WeakReference<FrameworkElement>(element);
                    pageChilds.Add(controlReference);
                    this.controlStates[element] = ControlLifetimeState.Active;
                }
            }
        }
    }

    public class ControlRemovedEventArgs : EventArgs
    {
        internal ControlRemovedEventArgs(FrameworkElement element)
        {
            this.Element = element;
        }

        public FrameworkElement Element { get; private set; }
    }

    public class ControlRestoredEventArgs : EventArgs
    {
        internal ControlRestoredEventArgs(FrameworkElement element)
        {
            this.Element = element;
        }

        public FrameworkElement Element { get; private set; }
    }
}
