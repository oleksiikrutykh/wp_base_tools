using System;
using System.ComponentModel;
using System.Windows;

using Microsoft.Phone.Shell;

namespace BaseTools.UI.Controls
{
    public class BindableApplicationBarMenuItem : FrameworkElement, IApplicationBarMenuItem
    {
        public static readonly DependencyProperty IsEnabledProperty = DependencyProperty.RegisterAttached("IsEnabled", typeof(bool), typeof(BindableApplicationBarMenuItem), new PropertyMetadata(true, OnEnabledChanged));

        public static readonly DependencyProperty TextProperty = DependencyProperty.RegisterAttached("Text", typeof(string), typeof(BindableApplicationBarMenuItem), new PropertyMetadata(OnTextChanged));

        private static void OnEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                ((BindableApplicationBarMenuItem)d).Item.IsEnabled = (bool)e.NewValue;
        }

        private static void OnTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (e.NewValue != e.OldValue)
                ((BindableApplicationBarMenuItem)d).Item.Text = e.NewValue.ToString();
        }

        public IApplicationBarMenuItem Item
        {
            get;
            set;
        }

        public BindableApplicationBarMenuItem()
        {
            InitializeItem(new ApplicationBarMenuItem());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters", MessageId = "Microsoft.Phone.Shell.IApplicationBarMenuItem.set_Text(System.String)", Justification = "This fix control works in design time")]
        protected void InitializeItem(IApplicationBarMenuItem item)
        {
            Item = item;
            if (DesignerProperties.IsInDesignTool)
                Item.Text = "Text";

            Item.Click += (s, e) =>
            {
                if (Click != null)
                    Click(this, e);
            };
        }

        public bool IsEnabled
        {
            get
            {
                return (bool)GetValue(IsEnabledProperty);
            }
            set
            {
                SetValue(IsEnabledProperty, value);
            }
        }

        public string Text
        {
            get
            {
                return (string)GetValue(TextProperty);
            }
            set
            {
                SetValue(TextProperty, value);
            }
        }

        public BindableApplicationBar Parent { get; set; }

        public event EventHandler Click;

    }
}