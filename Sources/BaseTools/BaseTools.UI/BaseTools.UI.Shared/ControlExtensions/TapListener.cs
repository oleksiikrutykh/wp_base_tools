﻿namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT

    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using Microsoft.Phone.Controls;
    using System.Windows.Media;
    using Microsoft.Phone.Tasks;

#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls.Primitives;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;
#endif

    using System;
    using System.Windows;
    using System.Windows.Input;
    using BaseTools.Core.Utility;

    /// <summary>
    /// Execte command when tap event is raised.
    /// </summary>
    public static class TapListener
    {
        /// <summary>
        /// Attached property which attached to any UIElement. Represents command that executed when UIElement tapped.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable")]
        public static readonly DependencyProperty TapCommandProperty =
            DependencyProperty.RegisterAttached("TapCommand", typeof(ICommand), typeof(TapListener), new PropertyMetadata(null, OnAttachedPropertyChanged));

        /// <summary>
        /// Attached property which attached to any UIElement. Represents parameter of command executed when UIElement tapped.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable")]
        public static readonly DependencyProperty TapCommandParameterProperty =
            DependencyProperty.RegisterAttached("TapCommandParameter", typeof(object), typeof(TapListener), new PropertyMetadata(null));

        /// <summary>
        /// Attached property which attached to any UIElement. Represents navigation uri that performs when UIElement tapped.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable")]
        public static readonly DependencyProperty NavigationUriProperty =
            DependencyProperty.RegisterAttached("NavigationUri", typeof(Uri), typeof(TapListener), new PropertyMetadata(null, OnAttachedPropertyChanged));

        /// <summary>
        /// Attached property which attached to any UIElement. If true, control handle Tap routed event, and it not raised
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable")]
        public static readonly DependencyProperty NeedHandleTapProperty =
            DependencyProperty.RegisterAttached("NeedHandleTap", typeof(bool), typeof(TapListener), new PropertyMetadata(false, OnNeedHandleTapChanged));

        /// <summary>
        /// Gets value of that indicates whether control need handle tap event.
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>value of that indicates whether control need handle tap event.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static bool GetNeedHandleTap(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (bool)obj.GetValue(NeedHandleTapProperty);
        }

        /// <summary>
        /// Sets value that indicates whether control need handle tap event. 
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <param name="value">New value of NeedHandleTap attached property</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetNeedHandleTap(DependencyObject obj, bool value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(NeedHandleTapProperty, value);
        }

        /// <summary>
        /// Gets value of NavigationUri attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Uri that accociated through NavigationUri property with UIElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static Uri GetNavigationUri(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (Uri)obj.GetValue(NavigationUriProperty);
        }

        /// <summary>
        /// Sets value of NavigationUri attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <param name="value">New value of NavigationUri attached property</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static void SetNavigationUri(DependencyObject obj, Uri value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(NavigationUriProperty, value);
        }

        /// <summary>
        /// Gets value of TapCommand attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Command that accociated through TapCommand property with UIElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static ICommand GetTapCommand(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (ICommand)obj.GetValue(TapCommandProperty);
        }

        /// <summary>
        /// Sets value of TapCommand attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <param name="value">New value of TapCommand attached property</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static void SetTapCommand(DependencyObject obj, ICommand value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(TapCommandProperty, value);
        }

        /// <summary>
        /// Gets value of TapCommandParameter attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Command that accociated through TapCommandParameter property with UIElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static object GetTapCommandParameter(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return obj.GetValue(TapCommandParameterProperty);
        }

        /// <summary>
        /// Sets value of TapCommandParameter attached property
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <param name="value">New value of TapCommandParameter attached property</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static void SetTapCommandParameter(DependencyObject obj, object value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(TapCommandParameterProperty, value);
        }

        /// <summary>
        /// Handle change of TapCommand or NavigateUri attached properties
        /// </summary>
        /// <param name="sender">UIElement which TapCommand or NavigateUri property was chaned</param>
        /// <param name="e">Info about property changes</param>
        private static void OnAttachedPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var inspectedUIElement = (UIElement)sender;
#if SILVERLIGHT
            if (e.OldValue != null)
            {
                inspectedUIElement.Tap -= OnUIElementTapped;
            }

            if (e.NewValue != null)
            {
                inspectedUIElement.Tap += OnUIElementTapped;
            }
#endif

#if WINRT
            if (e.OldValue != null)
            {
                inspectedUIElement.Tapped -= OnUIElementTapped;
            }

            if (e.NewValue != null)
            {
                inspectedUIElement.Tapped += OnUIElementTapped;
            }
#endif
        }

        /// <summary>
        /// Handle tap event of current item
        /// </summary>
        /// <param name="sender">UIElement which raise Tap event</param>
        /// <param name="e">Info about tap event.</param>
#if SILVERLIGHT
        private static void OnUIElementTapped(object sender, GestureEventArgs e)
#endif

#if WINRT
        private static void OnUIElementTapped(object sender, TappedRoutedEventArgs e)
#endif
        {
            var ansector = (DependencyObject)e.OriginalSource;
            var tappedObject = (DependencyObject)sender;

            // Stop tap event for controls that have tap command or navigation uri
            var needStopTapEvent = GetNeedHandleTap(tappedObject);
            if (needStopTapEvent)
            {
                e.Handled = true;
            }

            CheckTappedCommandsCalling(tappedObject, ansector);
        }

        /// <summary>
        /// Checks the tapped commands calling.
        /// </summary>
        /// <param name="tappedObject">The tapped object.</param>
        /// <param name="ansector">The ansector.</param>
        private static void CheckTappedCommandsCalling(DependencyObject tappedObject, DependencyObject ansector)
        {
            bool needExit = false;
            do
            {
                var tapCommand = GetTapCommand(ansector);

                // Check is element has TapCommand.
                if (tapCommand != null)
                {
                    var parameter = GetTapCommandParameter(ansector);
                    if (tapCommand.CanExecute(parameter))
                    {
                        if (ansector == tappedObject)
                        {
                            tapCommand.Execute(parameter);
                        }
                        else
                        {
                            needExit = true;
                        }
                    }
                }
                else
                {

                    // Check is element has NavigationUri property.
                    var navigateUri = GetNavigationUri(ansector);
                    if (navigateUri != null)
                    {
                        if (ansector == tappedObject)
                        {
                            OpenHyperlink(navigateUri);
                        }
                        else
                        {
                            needExit = true;
                        }
                    }
                }

                if (CheckForTapHandlingByAnotherControl(ansector))
                {
                    needExit = true;
                }

                if (ansector == tappedObject)
                {
                    needExit = true;
                }
                else
                {
                    ansector = VisualTreeHelper.GetParent(ansector);
                }
            }
            while (!needExit);
        }

#if WINRT
        /// <summary>
        /// Open specified uri;
        /// </summary>
        /// <param name="uri"></param>
        private async static void OpenHyperlink(Uri uri)
        {
            await Windows.System.Launcher.LaunchUriAsync(uri);
        }
#endif

#if SILVERLIGHT

        /// <summary>
        /// Open specified uri;
        /// </summary>
        /// <param name="uri"></param>
        private static void OpenHyperlink(Uri uri)
        {
            if (!uri.IsAbsoluteUri)
            {
                var frame = (PhoneApplicationFrame)Application.Current.RootVisual;
                frame.Navigate(uri);
            }
            else
            {
                var webBrowserTask = new WebBrowserTask();
                webBrowserTask.Uri = uri;
                webBrowserTask.Show();
            }
        }
#endif



        /// <summary>
        /// Check if control handle tap itself
        /// </summary>
        /// <param name="control">Control for inspection</param>
        /// <returns>Value indicates wheter control handle tap itself</returns>
        private static bool CheckForTapHandlingByAnotherControl(DependencyObject control)
        {
            bool tapHandled = false; 
            var button = control as ButtonBase;
            if (button != null && button.IsEnabled)
            {
                tapHandled = true;
            }

            var slider = control as Slider;
            if (slider != null && slider.IsEnabled)
            {
                tapHandled = true;
            }

            return tapHandled;
        }

        private static void OnNeedHandleTapChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var frameworkElement = (FrameworkElement)sender;
            var needHandleTap = (bool)e.NewValue;

#if SILVERLIGHT
            if (needHandleTap)
            {
                frameworkElement.Tap += StopTapEventRaising;
            }
            else
            {
                frameworkElement.Tap-= StopTapEventRaising;
            }
#endif

#if WINRT
            if (needHandleTap)
            {
                frameworkElement.Tapped += StopTapEventRaising;
            }
            else
            {
                frameworkElement.Tapped -= StopTapEventRaising;
            }
#endif
        }

#if SILVERLIGHT
        private static void StopTapEventRaising(object sender, GestureEventArgs e)
#endif
#if WINRT
        private static void StopTapEventRaising(object sender, TappedRoutedEventArgs e)
#endif
        {
            var tappedElement = (FrameworkElement)sender;
            var command = GetTapCommand(tappedElement);
            var uri = GetNavigationUri(tappedElement);
            if (command == null && uri == null)
            {
                // Stop tap event for controls that have no tap command or navigation uri.
                e.Handled = true;
            }
        }
    }
}
