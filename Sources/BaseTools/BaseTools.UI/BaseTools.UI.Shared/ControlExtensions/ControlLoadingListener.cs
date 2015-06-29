namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Controls.Primitives;
#endif
#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls.Primitives;
#endif
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BaseTools.Core.Utility;
    using BaseTools.UI.Common;

    /// <summary>
    /// Class for tracking load state of single <see cref="FrameworkElement"/>.
    /// </summary>
    internal class ControlLoadingListener
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:ControlLoadingListener"/> class 
        /// and attach it to <see cref="FrameworkElement"/>
        /// </summary>
        /// <param name="element">An isntance of <see cref="FrameworkElement"/> for load state tracking.</param>
        public ControlLoadingListener(FrameworkElement element)
        {
            Guard.CheckIsNotNull(element, "element");

            // Find parents tree for control.
            var tree = VisualTreeHelperExtensions.GetVisualAncestors(element).ToList();
            tree.Insert(0, element);
            if (tree.Count > 0)
            {
                // Check last item.
                var lastItem = tree[tree.Count - 1];
                var rootPopup = lastItem.Parent as Popup;

                // If tree is placed in popup - check it IsOpen property.
                if (rootPopup != null)
                {
                    this.IsLoaded = rootPopup.IsOpen;
                }
            }


            foreach (var visualParent in tree)
            {
                // If element in tree is root element of application - control is in tree.
#if SILVERLIGHT
                if (Application.Current.RootVisual == visualParent)
                {
                   
                    this.IsLoaded = true;
                    break;
                }
#endif

#if WINRT
                if (Window.Current.Content == visualParent)
                {
                    this.IsLoaded = true;
                    break;
                }
#endif
            }

            // Track Loaded and Unloaded events.
            element.Loaded += OnLoaded;
            element.Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:ControlLoadingListener"/> class.
        /// </summary>
        /// <param name="element">An isntance of <see cref="FrameworkElement"/> for load state tracking.</param>
        /// <param name="isLoadedInitial">Initial value for IsLoaded property.</param>
        public ControlLoadingListener(FrameworkElement element, bool isLoadedInitial)
        {
            Guard.CheckIsNotNull(element, "element");
            this.IsLoaded = isLoadedInitial;

            // Track Loaded and Unloaded events.
            element.Loaded += OnLoaded;
            element.Unloaded += OnUnloaded;
        }

        /// <summary>
        /// Gets a value indicating whether control is laoded to visual tree.
        /// </summary>
        public bool IsLoaded { get; private set; }

        /// <summary>
        /// Occurs when control has been loaded to visual tree.
        /// </summary>
        public event EventHandler Loaded;

        /// <summary>
        /// Occurs when control has been unloaded to visual tree.
        /// </summary>
        public event EventHandler Unloaded;

        /// <summary>
        /// Handle unloading of associated control.
        /// </summary>
        /// <param name="sender">Associated control.</param>
        /// <param name="e">Info about unload event.</param>
        private void OnUnloaded(object sender, RoutedEventArgs e)
        {
            this.IsLoaded = false;
            this.Unloaded.CallEvent(this, EventArgs.Empty);
        }

        /// <summary>
        /// Handle loading of associated control.
        /// </summary>
        /// <param name="sender">Associated control.</param>
        /// <param name="e">Info about load event.</param>
        private void OnLoaded(object sender, RoutedEventArgs e)
        {

            this.IsLoaded = true;
            this.Loaded.CallEvent(this, EventArgs.Empty);
        }
    }
}
