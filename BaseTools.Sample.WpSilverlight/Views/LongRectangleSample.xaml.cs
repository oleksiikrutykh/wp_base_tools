using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.Windows.Media;

namespace BaseTools.Sample.WpSilverlight.Views
{
    public partial class LongRectangleSample : PhoneApplicationPage
    {
        public LongRectangleSample()
        {
            InitializeComponent();
            this.stackPanel.Children.Add(new Border { Height = 100, Width = 100, Margin = new Thickness(5), Background = new SolidColorBrush(Colors.Green) });
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.stackPanel.Children.Add(new Border { Height = 100, Width = 100, Margin = new Thickness(5), Background = new SolidColorBrush(Colors.Green) });
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            if (this.stackPanel.Children.Count > 0)
            {
                this.stackPanel.Children.RemoveAt(this.stackPanel.Children.Count - 1);
            }
        }

        private void Button_Click2(object sender, RoutedEventArgs e)
        {
            this.horizontalStackPanel.Children.Add(new Border { Height = 100, Width = 100, Margin = new Thickness(5), Background = new SolidColorBrush(Colors.Green) });
        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            if (this.horizontalStackPanel.Children.Count > 0)
            {
                this.horizontalStackPanel.Children.RemoveAt(this.horizontalStackPanel.Children.Count - 1);
            }
        }

    }
}