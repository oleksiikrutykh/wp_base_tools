namespace BaseTools.UI.Input
{
#if WINRT
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI.Xaml.Input;
#endif

#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Input;
#endif

    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Provide info about <see cref="ManipulationListener.ManipulationCompleted"/> event. Unified API between platforms.
    /// </summary>
    internal class ManipulationCompletedSharedEventArgs : EventArgs
    {
        /// <summary>
        /// Real event args.
        /// </summary>
#if WINRT
        private PointerRoutedEventArgs internalArgs;
#endif
#if SILVERLIGHT
        private ManipulationCompletedEventArgs internalArgs;
#endif

        /// <summary>
        /// Initialize a new instance of <see cref="ManipulationCompletedSharedEventArgs"/>
        /// </summary>
        /// <param name="args">real event args</param>
#if WINRT
        public ManipulationCompletedSharedEventArgs(PointerRoutedEventArgs args)
#endif

#if SILVERLIGHT
        public ManipulationCompletedSharedEventArgs(ManipulationCompletedEventArgs args)
#endif
        {
            this.internalArgs = args;
        }
    }
}
