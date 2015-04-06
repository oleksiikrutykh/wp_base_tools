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
    using System.Diagnostics;

    /// <summary>
    /// Provide unified API for using manipulation events.
    /// </summary>
    internal class ManipulationListener
    {
        /// <summary>
        /// Occurs  when an input device begins a manipulation on the <see cref="UIElement"/>.
        /// For information on using manipulations, see How to handle manipulation events
        /// for Windows Phone.
        /// </summary>
        public event EventHandler<ManipulationStartedSharedEventArgs> ManipulationStarted;

        /// <summary>
        /// Occurs when the input device changes position during a manipulation. For
        /// information on using manipulations, see How to handle manipulation events
        /// for Windows Phone.
        /// </summary>
        public event EventHandler<ManipulationDeltaSharedEventArgs> ManipulationDelta;

        /// <summary>
        /// Occurs when a manipulation and inertia on the <see cref="UIElement"/> is
        /// complete. For information on using manipulations, see How to handle manipulation
        /// events for Windows Phone.
        /// </summary>
        public event EventHandler<ManipulationCompletedSharedEventArgs> ManipulationCompleted;

        /// <summary>
        /// Initialize a new instance of <see cref="ManipulationListener"/>.
        /// </summary>
        public ManipulationListener()
        {
        }

        /// <summary>
        /// Start listen <see cref="ManipulationListener.ManipulationStarted"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that will be tracked.</param>
        public void AttachToManipulationStarted(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationStarted += OnManipulationStarted;
#endif
#if WINRT
            element.PointerPressed += OnPointerPressed;
#endif
        }

        /// <summary>
        /// Cancel tracking of <see cref="ManipulationListener.ManipulationStarted"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that was tracked.</param>
        public void DetachFromManipulationStarted(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationStarted -= this.OnManipulationStarted;
#endif
#if WINRT
            element.PointerPressed -= this.OnPointerPressed;
#endif
        }

        /// <summary>
        /// Start listen <see cref="ManipulationListener.ManipulationDelta"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that will be tracked.</param>
        public void AttachToManipulationDelta(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationDelta += this.OnManipulationDelta;
#endif
#if WINRT
            element.PointerMoved += this.OnPointerMoved;
#endif
        }

        /// <summary>
        /// Cancel tracking of <see cref="ManipulationListener.ManipulationDelta"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that was tracked.</param>
        public void DetachFromManipulationDelta(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationDelta -= this.OnManipulationDelta;
#endif
#if WINRT
            element.PointerMoved -= this.OnPointerMoved;
#endif
        }

        /// <summary>
        /// Start listen <see cref="ManipulationListener.ManipulationCompleted"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that will be tracked.</param>
        public void AttachToManipulationCompleted(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationCompleted += this.OnManipulationCompleted;
#endif
#if WINRT
            element.PointerExited += this.OnPointerExited;
            element.PointerReleased += this.OnPointerReleased;
            element.PointerCanceled += this.OnPointerCanceled;
            element.PointerCaptureLost += this.OnPointerCaptureLost;
#endif
        }

        /// <summary>
        /// Cancel tracking of <see cref="ManipulationListener.ManipulationCompleted"/> event for <see cref="FrameworkElement"/>.
        /// </summary>
        /// <param name="element">An instance of <see cref="FrameworkElement"/>, that was tracked.</param>
        public void DetachFromManipulationCompleted(FrameworkElement element)
        {
#if SILVERLIGHT
            element.ManipulationCompleted -= this.OnManipulationCompleted;
#endif
#if WINRT
            element.PointerExited -= this.OnPointerExited;
            element.PointerReleased -= this.OnPointerReleased;
            element.PointerCanceled -= this.OnPointerCanceled;
            element.PointerCaptureLost -= this.OnPointerCaptureLost;
#endif
        }

#if WINRT

        /// <summary>
        /// Handle pointer pressed event (manipulation started).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerPressed(object sender, PointerRoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            element.CapturePointer(e.Pointer);
            var args = new ManipulationStartedSharedEventArgs(e);
            this.ManipulationStarted.CallEvent(sender, args);
        }

        /// <summary>
        /// Handle pointer moved event (manipulation delta).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerMoved(object sender, PointerRoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            this.ManipulationDelta.CallEvent(element, new ManipulationDeltaSharedEventArgs(e));
        }

        /// <summary>
        /// Handle PointerCaptureLost event (manipulation completed for element placed on <see cref="ScrollViewer"/>).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerCaptureLost(object sender, PointerRoutedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            this.ReleaseElement(sender, e);
        }

        /// <summary>
        ///  Handle PointerCanceled event (manipulation completed).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerCanceled(object sender, PointerRoutedEventArgs e)
        {
            this.ReleaseElement(sender, e);
        }

        /// <summary>
        /// Handle PointerReleased event (manipulation completed).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerReleased(object sender, PointerRoutedEventArgs e)
        {
            this.ReleaseElement(sender, e);
        }

        /// <summary>
        /// Handle PointerExited event (manipulation completed).
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void OnPointerExited(object sender, PointerRoutedEventArgs e)
        {
            this.ReleaseElement(sender, e);
        }

        /// <summary>
        /// Call manipulation completed event for WinRT.
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Info about pointer event.</param>
        private void ReleaseElement(object elementObject, PointerRoutedEventArgs e)
        {
            var element = (FrameworkElement)elementObject; 
            
            //element.PointerMoved -= this.OnPointerMoved;
            //element.PointerExited -= this.OnPointerExited;
            //element.PointerReleased -= this.OnPointerReleased;
            //element.PointerCanceled -= this.OnPointerCanceled;
            //element.PointerCaptureLost -= this.OnPointerCaptureLost;
            this.ManipulationCompleted.CallEvent(element, new ManipulationCompletedSharedEventArgs(e));
        }
#endif

#if SILVERLIGHT
        /// <summary>
        /// Handle real ManipulationStarted event
        /// </summary>
        /// <param name="sender">associated element</param>
        /// <param name="e">real event args</param>
        private void OnManipulationStarted(object sender, ManipulationStartedEventArgs e)
        {
            var element = (FrameworkElement)sender;
            var handler = ManipulationStarted;
            if (handler != null)
            { 
                var args = new ManipulationStartedSharedEventArgs(e);
                handler.Invoke(sender, args);
            }
        }

        /// <summary>
        /// Handle real ManipulationDelta event
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Real event args</param>
        private void OnManipulationDelta(object sender, ManipulationDeltaEventArgs e)
        {
            var handler = ManipulationDelta;
            if (handler != null)
            {
                var args = new ManipulationDeltaSharedEventArgs(e);
                handler.Invoke(sender, args);
            }
        }

        /// <summary>
        /// Handle real ManipulationCompleted event
        /// </summary>
        /// <param name="sender">Associated element</param>
        /// <param name="e">Real event args</param>
        private void OnManipulationCompleted(object sender, ManipulationCompletedEventArgs e)
        {
            var element = (FrameworkElement)sender;

            var handler = ManipulationCompleted;
            if (handler != null)
            {
                var args = new ManipulationCompletedSharedEventArgs(e);
                handler.Invoke(sender, args);
            }
        }
#endif

    }
}
