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
    ///  Provide info about <see cref="ManipulationListener.ManipulationStarted"/> event. Unified API between platforms.
    /// </summary>
    internal class ManipulationStartedSharedEventArgs : EventArgs
    {
        /// <summary>
        /// Real event args.
        /// </summary>
#if WINRT
        private PointerRoutedEventArgs internalArgs;
#endif
#if SILVERLIGHT
        private ManipulationStartedEventArgs internalArgs;
#endif

        /// <summary>
        /// Initialzie a new instance of <see cref="ManipulationStartedSharedEventArgs"/>
        /// </summary>
        /// <param name="args">Real event args.</param>
#if WINRT
        public ManipulationStartedSharedEventArgs(PointerRoutedEventArgs args)
#endif

#if SILVERLIGHT
        public ManipulationStartedSharedEventArgs(ManipulationStartedEventArgs args)
#endif
        {
            this.internalArgs = args;
        }

        /// <summary>
        /// Gets a reference to the object that raised the event.
        /// </summary>
        public object OriginalSource
        {
            get
            {
                return this.internalArgs.OriginalSource;
            }
        }

        /// <summary>
        /// Gets the container that defines the coordinates for the manipulation.
        /// </summary>
        public UIElement ManipulationContainer
        {
            get
            {
#if WINRT
                return this.internalArgs.OriginalSource as UIElement;
                //return this.internalArgs.Container;
#endif

#if SILVERLIGHT
                return this.internalArgs.ManipulationContainer;
#endif
            }
        }

        /// <summary>
        /// Gets the point from which the manipulation originated.
        /// </summary>
        public Point ManipulationOrigin
        {
            get
            {
#if WINRT
                return new Point();
#endif

#if SILVERLIGHT
                return this.internalArgs.ManipulationOrigin;
#endif
            }
        }

    }
}
