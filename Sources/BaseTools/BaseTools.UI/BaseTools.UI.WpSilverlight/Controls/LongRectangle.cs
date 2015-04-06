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
    using System.Windows.Shapes;

    public class LongRectangle : Control
    {
        private const string RootPanelName = "rootPanel";

        private const double ElementMaxSize = 2048;

        private Grid rootPanel;

        private Size currentSize = new Size();

        private int previousRowsCount;

        private int previousColumsCount;

        public LongRectangle()
        {
            this.DefaultStyleKey = typeof(LongRectangle);
            this.SizeChanged += OnSizeChanged;
        }



        public Brush Fill
        {
            get 
            {
                return (Brush)GetValue(FillProperty);
            }

            set
            { 
                SetValue(FillProperty, value);
            }
        }

        // Using a DependencyProperty as the backing store for Fill.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty FillProperty =
            DependencyProperty.Register("Fill", typeof(Brush), typeof(LongRectangle), new PropertyMetadata(null, OnFillChangedStatic));

        private static void OnFillChangedStatic(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (LongRectangle)sender;
            control.OnFillChanged();
        }

        private void OnFillChanged()
        {
            if (this.rootPanel != null)
            {
                foreach (Rectangle rectangle in this.rootPanel.Children)
                {
                    rectangle.Fill = this.Fill;
                }
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.rootPanel = (Grid)this.GetTemplateChild(RootPanelName);
            this.RecreateContent();
        }

        private void OnSizeChanged(object sender, System.Windows.SizeChangedEventArgs e)
        {
            if (e.NewSize.Height != 0 &&
                e.NewSize.Width != 0 &&
                (this.currentSize.Height != e.NewSize.Height || this.currentSize.Width != e.NewSize.Width))
            {
                this.currentSize = e.NewSize;
                this.RecreateContent();
            }
        }

        private void RecreateContent()
        {
            if (this.rootPanel != null)
            {
                var rowsCount = (int)Math.Ceiling(this.currentSize.Height / ElementMaxSize);
                var columnsCount = (int)Math.Ceiling(this.currentSize.Width / ElementMaxSize);
                while (this.rootPanel.RowDefinitions.Count > rowsCount)
                {
                     this.rootPanel.RowDefinitions.RemoveAt(this.rootPanel.RowDefinitions.Count - 1);
                }

                while (this.rootPanel.RowDefinitions.Count < rowsCount)
                {
                    this.rootPanel.RowDefinitions.Add(new RowDefinition { Height = new GridLength(0, GridUnitType.Auto), });
                }

                while (this.rootPanel.ColumnDefinitions.Count > columnsCount)
                {
                     this.rootPanel.ColumnDefinitions.RemoveAt(this.rootPanel.ColumnDefinitions.Count - 1);
                }

                while (this.rootPanel.ColumnDefinitions.Count < columnsCount)
                {
                    this.rootPanel.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(0, GridUnitType.Auto) });
                }

                double lastColumnWidth = 0;
                if (columnsCount > 0)
                {
                    lastColumnWidth = this.currentSize.Width - (columnsCount - 1) * ElementMaxSize;
                }

                double lastRowHeight = 0;
                if (rowsCount > 0)
                {
                    lastRowHeight = this.currentSize.Height - (rowsCount - 1) * ElementMaxSize;
                }


                var rowLimit = Math.Max(this.previousRowsCount, rowsCount);
                var columnLimit = Math.Max(this.previousColumsCount, columnsCount);

                var childIndex = 0;
                for (int rowIndex = 0; rowIndex < rowLimit; rowIndex++)
                {
                    bool isRowAdded = rowIndex >= this.previousRowsCount;
                    bool isRowRemoved = rowIndex >= rowsCount;

                    for (int columnIndex = 0; columnIndex < columnLimit; columnIndex++)
                    {
                        var isColumnAdded = columnIndex >= this.previousColumsCount;
                        var isColumnRemoved = columnIndex >= columnsCount;

                        if (isRowRemoved)
                        {
                            if (!isColumnAdded)
                            {
                                this.RemoveControl(childIndex);
                            }
                        }
                        else if (isRowAdded)
                        {
                            if (!isColumnRemoved)
                            {
                                var child = this.AddControl(childIndex, rowIndex, columnIndex);

                                var height = ElementMaxSize;
                                if (rowIndex == rowsCount - 1)
                                {
                                    height = lastRowHeight;
                                }

                                var width = ElementMaxSize;
                                if (columnIndex == columnsCount - 1)
                                {
                                    width = lastColumnWidth;
                                }

                                child.Height = height + 2;
                                child.Width = width + 2;


                                // TODO: update control sizes.
                                childIndex++;
                            }
                        }
                        else
                        {
                            if (isColumnRemoved)
                            {
                                this.RemoveControl(childIndex);
                            }
                            else if (isColumnAdded)
                            {
                                var child = this.AddControl(childIndex, rowIndex, columnIndex);
                                // TODO: update control sizes.

                                var height = ElementMaxSize;
                                if (rowIndex == rowsCount - 1)
                                {
                                    height = lastRowHeight;
                                }

                                var width = ElementMaxSize;
                                if (columnIndex == columnsCount - 1)
                                {
                                    width = lastColumnWidth;
                                }

                                child.Height = height + 2;
                                child.Width = width + 2;

                                childIndex++;
                            }
                            else
                            {
                                var child = (Rectangle)this.rootPanel.Children[childIndex];
                                // TODO: update control sizes.

                                var height = ElementMaxSize;
                                if (rowIndex == rowsCount - 1)
                                {
                                    height = lastRowHeight;
                                }

                                var width = ElementMaxSize;
                                if (columnIndex == columnsCount - 1)
                                {
                                    width = lastColumnWidth;
                                }

                                child.Height = height + 2;
                                child.Width = width + 2;

                                childIndex++;
                            }
                        }
                    }
                }


                this.previousColumsCount = columnsCount;
                this.previousRowsCount = rowsCount;
            }
        }

        private Rectangle AddControl(int index, int rowIndex, int columnIndex)
        {
            var rectangle = new Rectangle();
            rectangle.Fill = this.Fill;
            rectangle.Margin = new Thickness(-1);
            rectangle.HorizontalAlignment = System.Windows.HorizontalAlignment.Stretch;
            rectangle.VerticalAlignment = System.Windows.VerticalAlignment.Stretch;
            Grid.SetRow(rectangle, rowIndex);
            Grid.SetColumn(rectangle, columnIndex);
            this.rootPanel.Children.Insert(index, rectangle);

            return rectangle;
        }

        private void RemoveControl(int index)
        {
            this.rootPanel.Children.RemoveAt(index);
        }
    }
}
