namespace BaseTools.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;

    public class LightItemsControl : UserControl
    {
        // Using a DependencyProperty as the backing store for ItemsSource.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register("ItemsSource", typeof(object), typeof(LightItemsControl), new PropertyMetadata(null, OnItemsSourcePropertyChanged));

        // Using a DependencyProperty as the backing store for ItemTemplate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemTemplateProperty =
            DependencyProperty.Register("ItemTemplate", typeof(DataTemplate), typeof(LightItemsControl), new PropertyMetadata(null, OnItemTemplatePropertyChanged));

        // Using a DependencyProperty as the backing store for ItemsPanel.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ItemsPanelProperty =
            DependencyProperty.Register("ItemsPanel", typeof(Panel), typeof(LightItemsControl), new PropertyMetadata(null, OnItemsPanelPropertyChanged));

        public object ItemsSource
        {
            get 
            { 
                return (object)GetValue(ItemsSourceProperty);
            }

            set 
            {
                SetValue(ItemsSourceProperty, value); 
            }
        }

        public DataTemplate ItemTemplate
        {
            get 
            { 
                return (DataTemplate)GetValue(ItemTemplateProperty); 
            }

            set 
            { 
                SetValue(ItemTemplateProperty, value);
            }
        }

        public Panel ItemsPanel
        {
            get 
            { 
                return (Panel)GetValue(ItemsPanelProperty);
            }

            set 
            { 
                SetValue(ItemsPanelProperty, value);
            }
        }

        private static void OnItemsSourcePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (LightItemsControl)sender;
            control.OnItemsSourceChanged(args.OldValue, args.NewValue);
        }

        private void OnItemsSourceChanged(object oldItemsSource, object newItemsSource)
        {
            var oldItemSourceEnumerable = oldItemsSource as IEnumerable;
            if (oldItemSourceEnumerable != null)
            {
                this.ClearPanel(this.ItemsPanel);
            }

            this.Render(this.ItemsPanel, newItemsSource);
        }

        private static void OnItemTemplatePropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
        }

        private static void OnItemsPanelPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var control = (LightItemsControl)sender;
            control.OnItemsPanelChanged((Panel)args.OldValue, (Panel)args.NewValue);
        }

        private void OnItemsPanelChanged(Panel previousPanel, Panel newPanel)
        {
            ClearPanel(previousPanel);
            this.Content = newPanel;
            Render(newPanel, this.ItemsSource);
        }

        private void ClearPanel(Panel panel)
        {
            if (panel != null)
            {
                panel.Children.Clear();
            }
        }

        private void Render(Panel panel, object items)
        {
            var itemsEnumerable = items as IEnumerable;
            if (panel != null && itemsEnumerable != null)
            {
                foreach (var item in itemsEnumerable)
                {
                    var control = this.GetControl(item);
                    panel.Children.Add(control);
                }
            }
        }

        private FrameworkElement GetControl(object dataItem)
        {
            FrameworkElement element = null;
            var template = this.ItemTemplate;
            if (template != null)
            {
                element = (FrameworkElement)template.LoadContent();
            }

            if (element == null)
            {
                element = new TextBlock();
                element.SetBinding(TextBlock.TextProperty, new Binding());
            }

            element.DataContext = dataItem;
            return element;
        }
        
    }
}
