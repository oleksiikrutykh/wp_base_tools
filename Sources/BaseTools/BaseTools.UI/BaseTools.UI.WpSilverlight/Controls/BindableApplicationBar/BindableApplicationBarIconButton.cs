using System;

using Microsoft.Phone.Shell;
using System.Windows;

namespace BaseTools.UI.Controls
{
    public class BindableApplicationBarIconButton : BindableApplicationBarMenuItem, IApplicationBarIconButton
    {
        public BindableApplicationBarIconButton() 
        {
            InitializeItem(new ApplicationBarIconButton());
        }


        public static readonly DependencyProperty IconUriProperty = DependencyProperty.Register("IconUri", typeof(Uri), typeof(BindableApplicationBarIconButton), new PropertyMetadata(OnIconUriPropertyChanged));

        public ApplicationBarIconButton Button
        {
            get
            {
                return (ApplicationBarIconButton)Item;
            }
        }

        public Uri IconUri
        {
            get
            {
                return (Uri)GetValue(IconUriProperty);
            }
            set
            {
                SetValue(IconUriProperty, value);
            }
        }

        private static void OnIconUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((BindableApplicationBarIconButton)d).Button.IconUri = (Uri)e.NewValue;
        }

    }
}