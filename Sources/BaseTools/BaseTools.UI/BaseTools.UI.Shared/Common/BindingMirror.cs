namespace BaseTools.UI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using BaseTools.Core.Utility;

    public class BindingMirror : DependencyObject
    {
        // Using a DependencyProperty as the backing store for Mirror.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty DataProperty =
            DependencyProperty.Register("Data", typeof(object), typeof(BindingMirror), new PropertyMetadata(null, OnChanged));

        public object Data
        {
            get
            {
                return (object)GetValue(DataProperty);
            }
            set
            {
                SetValue(DataProperty, value);
            }
        }

        private static void OnChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (BindingMirror)sender;
            control.DataChanged.CallEvent(control, EventArgs.Empty);
        }

        public event EventHandler DataChanged;
    }
}
