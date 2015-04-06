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
using System.Diagnostics;
using System.Threading.Tasks;
using BaseTools.Core.Info;

namespace BaseTools.Sample.WpSilverlight.Views
{

    public partial class BindableAppBarSample : PhoneApplicationPage
    {
        private Color transparent;

        private Color untransparent;

        public BindableAppBarSample()
        {
            
            InitializeComponent();
            

            this.transparent = (Color)this.Resources["TransparentColor"];
            this.untransparent = (Color)this.Resources["UntransparentColor"];
            this.SizeChanged += BindableAppBarSample_SizeChanged;
        }

        void BindableAppBarSample_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Debug.WriteLine(e.NewSize);
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.appBar.BackgroundColor == this.transparent)
            {
                this.appBar.BackgroundColor = this.untransparent;
            }
            else
            {
                this.appBar.BackgroundColor = this.transparent;
            }
            
        }

        private void Button_Click1(object sender, RoutedEventArgs e)
        {
            var appBar1 = this.ApplicationBar;
            this.ApplicationBar = null;
            //this.ApplicationBar = appBar1;
        }
    }
}