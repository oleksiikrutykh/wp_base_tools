namespace BaseTools.UI.ControlExtensions
{
    using System;
    using System.ComponentModel;
    using System.Net;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Ink;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    using System.Windows.Shapes;

    public class ImageLoaderSettings : DependencyObject
    {
        private static readonly DependencyProperty DefaultImageStubProperty = DependencyProperty.Register("DefaultImageStub", typeof(string), typeof(ImageLoaderSettings), new PropertyMetadata(null, OnDefaultImageStubChanged));

        public string DefaultImageStub
        {
            get 
            {
                return (string)this.GetValue(DefaultImageStubProperty);
            }

            set 
            {
                this.SetValue(DefaultImageStubProperty, value);
            }
        }


        private static void OnDefaultImageStubChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var isDesingMode = DesignerProperties.IsInDesignTool;
            if (!isDesingMode)
            {
                var newImageStub = (string)e.NewValue;
                var uri = new Uri(newImageStub, UriKind.RelativeOrAbsolute);
                ImageLoader.ImageStubPath = uri;
            }
        }
    }
}
