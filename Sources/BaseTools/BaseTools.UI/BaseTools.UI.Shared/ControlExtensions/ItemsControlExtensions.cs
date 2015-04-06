namespace BaseTools.UI.ControlExtensions
{
    using BaseTools.Core.Models;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;

    public static class ItemsControlExtensions
    {
        public static readonly DependencyProperty ItemsSourceProperty = DependencyProperty.RegisterAttached("ItemsSource", typeof (IList), typeof (ItemsControlExtensions), new PropertyMetadata(null, new PropertyChangedCallback(OnDataChanged)));
        
        public static readonly DependencyProperty ViewCountProperty = DependencyProperty.RegisterAttached("ViewCount", typeof (int), typeof (ItemsControlExtensions), new PropertyMetadata(-1, new PropertyChangedCallback(OnDataChanged)));

        public static IList GetItemsSource(ItemsControl sender)
        {
            return (IList) sender.GetValue(ItemsSourceProperty);
        }

        public static void SetItemsSource(ItemsControl sender, IList value)
        {
            sender.SetValue(ItemsSourceProperty, (object) value);
        }

        public static int GetViewCount(ItemsControl sender)
        {
            return (int)sender.GetValue(ViewCountProperty);
        }

        public static void SetViewCount(ItemsControl sender, int value)
        {
            sender.SetValue(ViewCountProperty, (object) value);
        }

        private static void OnDataChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            ItemsControl sender1 = (ItemsControl) sender;
            IList wrappedCollection = GetItemsSource(sender1);
            int viewCount = GetViewCount(sender1);
            if (viewCount >= 0)
            {
                wrappedCollection = new Subcollection(wrappedCollection, 0, viewCount);
            }

            sender1.ItemsSource = wrappedCollection;
        }
    }
}
