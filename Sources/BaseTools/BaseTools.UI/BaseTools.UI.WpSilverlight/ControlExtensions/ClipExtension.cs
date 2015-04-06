namespace Marketplace.Sdk.UI.Extesions
{
#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Media;
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.Foundation;
    using Windows.UI.Xaml.Media;
#endif

    using System;
    using System.Collections.Generic;
    using System.Text;
    
    /// <summary>
    /// Applies a clipping for controls. 
    /// </summary>
    public static class ClipExtension
    {
        /// <summary>
        /// Identifies the <see href="ClipExtension">.AllowClipToBounds attached property.
        /// </summary>
        public static readonly DependencyProperty AllowClipToBoundsProperty =
            DependencyProperty.RegisterAttached("AllowClipToBounds", typeof(bool),
            typeof(ClipExtension), new PropertyMetadata(false, OnToBoundsPropertyChanged));

        /// <summary>
        /// Gets a value indicates whether <see cref="FrameworkElement"/> must be clipped by its bounds.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> for checking.</param>
        /// <returns>True if this clipping is applied.</returns>
        public static bool GetAllowClipToBounds(FrameworkElement element)
        {
            return (bool)element.GetValue(AllowClipToBoundsProperty);
        }

        /// <summary>
        /// Sets a value indicates whether <see cref="FrameworkElement"/> must be clipped by its bounds.
        /// </summary>
        /// <param name="element">The <see cref="FrameworkElement"/> for clipping.</param>
        /// <param name="clipToBounds">The new value of ClipToBounds property</param>
        public static void SetAllowClipToBounds(FrameworkElement element, bool clipToBounds)
        {
            element.SetValue(AllowClipToBoundsProperty, clipToBounds);
        }

        private static void OnToBoundsPropertyChanged(DependencyObject d,
            DependencyPropertyChangedEventArgs e)
        {
            FrameworkElement fe = d as FrameworkElement;
            if (fe != null)
            {
                ClipToBounds(fe);

                // whenever the element which this property is attached to is loaded
                // or re-sizes, we need to update its clipping geometry
                fe.SizeChanged -= OnSizeChanged;
                fe.SizeChanged += OnSizeChanged;
            }
        }

        /// <summary>
        /// Creates a rectangular clipping geometry which matches the geometry of the
        /// passed element
        /// </summary>
        private static void ClipToBounds(FrameworkElement fe)
        {
            if (GetAllowClipToBounds(fe))
            {
                fe.Clip = new RectangleGeometry()
                {
                    Rect = new Rect(0, 0, fe.ActualWidth, fe.ActualHeight)
                };
            }
            else
            {
                fe.Clip = null;
            }
        }

        /// <summary>
        /// Handle the size change of clipped element. 
        /// </summary>
        /// <param name="sender">clipped element</param>
        /// <param name="e">Info about size changes</param>
        private static void OnSizeChanged(object sender, SizeChangedEventArgs e)
        {
            ClipToBounds(sender as FrameworkElement);
        }
    }
}
