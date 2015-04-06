namespace BaseTools.UI.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    public class LongBackgroundPanel : Panel
    {
        private LongRectangle backgroundElement = new LongRectangle();

        public LongBackgroundPanel()
        {
            this.Children.Add(backgroundElement);
            this.backgroundElement.SetBinding(LongRectangle.FillProperty, new System.Windows.Data.Binding { Source = this, Path = new PropertyPath("LongBackground") });
        }



        public Brush LongBackground
        {
            get { return (Brush)GetValue(LongBackgroundProperty); }
            set { SetValue(LongBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for FixedBackground.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty LongBackgroundProperty =
            DependencyProperty.Register("LongBackground", typeof(Brush), typeof(LongBackgroundPanel), new PropertyMetadata(null));


        

        

        

        protected override System.Windows.Size MeasureOverride(System.Windows.Size availableSize)
        {
            Debug.WriteLine("Measure" + availableSize);
            var resultSize = new Size();
            foreach (var child in this.Children)
            {
                if (child != backgroundElement)
                {
                    child.Measure(availableSize);
                    var desiredSize = child.DesiredSize;
                    resultSize = CombineSizes(resultSize, desiredSize);
                }
            }

            this.backgroundElement.Measure(availableSize);
            return resultSize;
        }

        protected override System.Windows.Size ArrangeOverride(System.Windows.Size finalSize)
        {
            Debug.WriteLine("Arrange" + finalSize);
            var resultSize = new Size();
            var arrangeRect = new Rect(0, 0, finalSize.Width, finalSize.Height);
            foreach (FrameworkElement item in this.Children)
            {
                item.Arrange(arrangeRect);
                resultSize = CombineSizes(resultSize, item.RenderSize);
            }

            return resultSize;
        }

        private static Size CombineSizes(Size first, Size second)
        {
            var newWidth = Math.Max(first.Width, second.Width);
            var newHeight = Math.Max(first.Height, second.Height);
            return new Size(newWidth, newHeight);
        }
    }
}
