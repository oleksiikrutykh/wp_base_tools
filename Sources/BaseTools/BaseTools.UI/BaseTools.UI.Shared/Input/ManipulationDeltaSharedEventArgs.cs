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
    ///  Provides data for the <see cref="ManipulationListener.ManipulationDelta"/> event. Unified API between different platforms.
    /// </summary>
    internal class ManipulationDeltaSharedEventArgs : EventArgs
    {
        /// <summary>
        /// Real event args.
        /// </summary>
#if WINRT
        private PointerRoutedEventArgs internalArgs;
#endif

#if SILVERLIGHT
        private ManipulationDeltaEventArgs internalArgs;
#endif

        /// <summary>
        /// Initialize a new instance of <see cref="ManipulationDeltaSharedEventArgs"/>
        /// </summary>
        /// <param name="args">Real event args.</param>
#if WINRT
        public ManipulationDeltaSharedEventArgs(PointerRoutedEventArgs args)
#endif

#if SILVERLIGHT
        public ManipulationDeltaSharedEventArgs(ManipulationDeltaEventArgs args)
#endif
        {
            this.internalArgs = args;
        }

        /// <summary>
        /// Gets the container that defines the coordinates for the manipulation
        /// </summary>
        public object ManipulationContainer
        {
            get
            {
#if SILVERLIGHT
                return this.internalArgs.ManipulationContainer;
#endif
#if WINRT
                return this.internalArgs.OriginalSource; // Container;
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
#if SILVERLIGHT
                return this.internalArgs.ManipulationOrigin;
#endif

#if WINRT
                var uiElement = (FrameworkElement)this.internalArgs.OriginalSource;
                var pointerPoint = this.internalArgs.GetCurrentPoint(uiElement);
                return pointerPoint.Position;
                //return  Position;
#endif
            }
        }
    }
}
