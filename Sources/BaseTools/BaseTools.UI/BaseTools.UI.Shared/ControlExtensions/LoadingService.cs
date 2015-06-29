namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System.Windows;
#endif
#if WINRT
    using Windows.UI.Xaml;
#endif
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
   
    /// <summary>
    /// Class for tracking load state of any control.
    /// </summary>
    public static class LoadingService
    {
        /// <summary>
        /// Identifies the <see href="ControlLoadService.LoadingListener"> attached property.
        /// </summary>
        private static readonly DependencyProperty LoadingListenerProperty =
            DependencyProperty.RegisterAttached("LoadingListener", typeof(ControlLoadingListener), typeof(LoadingService), new PropertyMetadata(null));

        /// <summary>
        /// Gets loading listener for control.
        /// </summary>
        /// <param name="obj">An associated <see cref="FrameworkElement"/>.</param>
        /// <returns>Associated loaded listener.</returns>
        internal static ControlLoadingListener GetLoadingListener(DependencyObject obj)
        {
            return (ControlLoadingListener)obj.GetValue(LoadingListenerProperty);
        }

        /// <summary>
        /// Sets loading listener for control.
        /// </summary>
        /// <param name="obj">An associated <see cref="FrameworkElement"/>.</param>
        /// <returns>Loaded listener fro tracking load state of <see cref="FrameworkElement"/>.</returns>
        internal static void SetLoadingListener(DependencyObject obj, ControlLoadingListener value)
        {
            obj.SetValue(LoadingListenerProperty, value);
        }

        /// <summary>
        /// Check whether element is loaded in visual tree.
        /// </summary>
        /// <param name="element">A checked instance of <see cref="FrameworkElement"/>.</param>
        /// <returns>True, if <see cref="FrameworkElement"/> is loaded in visual tree. Otherwise false.</returns>
        public static bool IsLoaded(FrameworkElement element)
        {
            var listener = GetLoadingListener(element);
            if (listener == null)
            {
                listener = new ControlLoadingListener(element);
                SetLoadingListener(element, listener);
            }

            return listener.IsLoaded;
        }

        /// <summary>
        /// Wait while element was loaded to visual tree.
        /// </summary>
        /// <param name="element">A checked instance of <see cref="FrameworkElement"/>.</param>
        /// <returns>When this method completes, control is loaded to visual tree.</returns>
        public static Task WhenLoaded(FrameworkElement element)
        {
            var task = new TaskCompletionSource<bool>();
            RoutedEventHandler loadedHandler = null;
            loadedHandler = (s, e) =>
            {
                element.Loaded -= loadedHandler;
                task.SetResult(true);
            };
            element.Loaded += loadedHandler;
            return task.Task;

        }

        /// <summary>
        /// Wait while element was unloaded from visual tree.
        /// </summary>
        /// <param name="element">A checked instance of <see cref="FrameworkElement"/>.</param>
        /// <returns>When this method completes, control is loaded to visual tree.</returns>
        public static Task WhenUnloaded(FrameworkElement element)
        {
            var task = new TaskCompletionSource<bool>();
            RoutedEventHandler unloadedHandler = null;
            unloadedHandler = (s, e) =>
            {
                element.Unloaded -= unloadedHandler;
                task.SetResult(true);
            };

            element.Unloaded += unloadedHandler;
            return task.Task;
        }
    }
}
