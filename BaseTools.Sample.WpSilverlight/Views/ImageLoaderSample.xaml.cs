using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using BaseTools.UI.ControlExtensions;

namespace BaseTools.Sample.WpSilverlight.Views
{
    public partial class ImageLoaderSample : PhoneApplicationPage
    {
        public ImageLoaderSample()
        {
            InitializeComponent();
            WaitOnLoading();
        }

        private async void WaitOnLoading()
        {
            var task = ImageLoader.WaitOnLoading(imageSuccess);
            var task2 = ImageLoader.WaitOnLoading(imageWebFail);
            await task;
            await task2;
        }
    }
}