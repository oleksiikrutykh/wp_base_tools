namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System.Windows.Controls;
#endif
#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif

    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
    using BaseTools.Core.Utility;
    using BaseTools.Core.Info;
    using System;

    public static class SelectionListener
    {
        /// <summary>
        /// Attached property which attached to ListViewBase element. Represents a value indicating whether on selected items collection.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.RegisterAttached("Items", typeof(IList), typeof(SelectionListener), new PropertyMetadata(null, OnSelectedItemsCollectionChanged));

        /// <summary>
        /// Attached property which attached to ListViewBase element. Represents a value indicating whether on is current ListViewBase element tracking or not.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty IsTrackProperty =
            DependencyProperty.RegisterAttached("IsTrack", typeof(bool), typeof(SelectionListener), new PropertyMetadata(false, IsTrackPropertyChanged));


        /// <summary>
        /// Attached property which attached to ListViewBase element. Represents a value indicating on AppBar which need to open when selected element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty СoupleItemsHelperProperty = DependencyProperty.RegisterAttached("СoupleItemsHelper", typeof(СoupleItemsHelper), typeof(SelectionListener), new PropertyMetadata(null));

#if WINRT
        /// <summary>
        /// Attached property which attached to ListViewBase element. Represents a value indicating on AppBar which need to open when selected element.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes")]
        public static readonly DependencyProperty AssociatedAppBarProperty = DependencyProperty.RegisterAttached("AssociatedAppBar", typeof(AppBar), typeof(SelectionListener), new PropertyMetadata(null));
#endif

        /// <summary>
        /// Gets a value indicating whether on selected items collection.
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Command that accociated through Command property with FrameworkElement</returns>
        private static СoupleItemsHelper GetCoupleItemsHelper(DependencyObject obj)
        {
            return (СoupleItemsHelper)obj.GetValue(СoupleItemsHelperProperty);
        }

        /// <summary>
        /// Sets a value indicating whether on selected items collection.
        /// </summary>
        /// <param name="obj">Associated object.</param>
        /// <param name="value">New value property.</param>
        private static void SetCoupleItemsHelper(DependencyObject obj, СoupleItemsHelper value)
        {
            obj.SetValue(СoupleItemsHelperProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether on selected items collection.
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Command that accociated through Command property with FrameworkElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Validate by Guard")]
        public static IList GetItems(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (IList)obj.GetValue(ItemsProperty);
        }

        /// <summary>
        /// Sets a value indicating whether on selected items collection.
        /// </summary>
        /// <param name="obj">Associated object.</param>
        /// <param name="value">New value property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public static void SetItems(DependencyObject obj, IList value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(ItemsProperty, value);
        }

        /// <summary>
        /// Gets a value indicating whether on on tracking state.
        /// </summary>
        /// <param name="obj">Associated object</param>
        /// <returns>Command that accociated through Command property with FrameworkElement</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public static bool GetIsTrack(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (bool)obj.GetValue(IsTrackProperty);
        }

        /// <summary>
        /// Sets a value indicating whether on tracking state.
        /// </summary>
        /// <param name="obj">Associated object.</param>
        /// <param name="value">New value property.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public static void SetIsTrack(DependencyObject obj, bool value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(IsTrackProperty, value);
        }

#if WINRT
        /// <summary>
        /// Sets a value indicating on AppBar control.
        /// </summary>
        /// <param name="obj">Associated object.</param>
        /// <param name="value">New value property.</param>
        public static AppBar GetAssociatedAppBar(DependencyObject obj)
        {
            return (AppBar)obj.GetValue(AssociatedAppBarProperty);
        }

        /// <summary>
        /// Invoked when AppBar control property changes.
        /// </summary>
        /// <param name="sender">Associated object.</param>
        /// <param name="e">Property contains information about changes.</param>
        public static void SetAssociatedAppBar(DependencyObject obj, AppBar value)
        {
            obj.SetValue(AssociatedAppBarProperty, value);
        }
#endif

        private static List<SelectorWrapperMapping> Mappings = new List<SelectorWrapperMapping>();

#if WINRT
        static SelectionListener()
        {
            AddMapping<ListViewBase, ListViewBaseWrapper>();
        }
#endif

        public static void AddMapping<TControl, TWrapper>()
            where TWrapper : SelectorWrapper, new()
            where TControl : DependencyObject
        {
            var mapping = new SelectorWrapperMapping<TControl, TWrapper>();
            Mappings.Add(mapping);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Validate by Guard")]
        public static SelectorWrapper CreateSelector(DependencyObject control)
        {
            Guard.CheckIsNotNull(control, "control");
            SelectorWrapper wrapper = null;
            var controlType = control.GetType();
            foreach (var mapping in Mappings)
            {
                if (mapping.CanCreateWrapper(control))
                {
                    wrapper = mapping.CreateWrapper(control);
                }
            }

            if (wrapper == null)
            {
                if (EnvironmentInfo.Current.IsInDesignMode)
                {
                    wrapper = new EmptyWrapper();
                }
                else
                {
                    var errorMessage = String.Format("Control of type {0} isn't supported by SelectionListener. Register selection wrapper for this control.", control.GetType());
                    throw new ArgumentException(errorMessage);
                }
            }

            return wrapper;
        }

        public static void IsTrackPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = CreateSelector(sender);

            if ((bool)e.NewValue)
            {
                selector.SelectionChanged += OnSelectorSelectionChanged;
            }
            else
            {
                selector.SelectionChanged -= OnSelectorSelectionChanged;
            }
        }



        /// <summary>
        /// Invoked when Items property changes.
        /// </summary>
        /// <param name="sender">Associated object.</param>
        /// <param name="e">Property contains information about changes.</param>
        private static void OnSelectedItemsCollectionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var selector = CreateSelector(sender);

            if (e.OldValue != null)
            {
                var coupleItemsHelper = GetCoupleItemsHelper(selector.InternalControl);
                coupleItemsHelper.Clear();
                SetCoupleItemsHelper(selector.InternalControl, null);
            }

            if (e.NewValue != null)
            {
                var collection = SelectionListener.GetItems(selector.InternalControl);
                if (collection != null)
                {
                    selector.SelectedItems.Clear();
                    foreach (var item in collection)
                    {
                        selector.SelectedItems.Add(item);
                    }
                }

                СoupleItemsHelper couple = new СoupleItemsHelper(selector, collection);
                SetCoupleItemsHelper(selector.InternalControl, couple);
            }

        }


        static void OnSelectorSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var selector = (SelectorWrapper)sender;

            if (selector != null)
            {
                var items = SelectionListener.GetItems(selector.InternalControl);

                if (items != null)
                {
                    foreach (var item in e.AddedItems)
                    {
                        if (!items.Contains(item))
                        {
                            items.Add(item);
                        }
                    }

                    foreach (var item in e.RemovedItems)
                    {
                        if (items.Contains(item))
                        {
                            items.Remove(item);
                        }
                    }
                }
            }

#if WINRT
            AppBar appBar = SelectionListener.GetAssociatedAppBar(selector.InternalControl);

            if (appBar != null)
            {
                if (selector.SelectedItems.Count > 0)
                {
                    appBar.IsSticky = true;
                    appBar.IsOpen = true;
                }
                else
                {
                    appBar.IsSticky = false;
                    appBar.IsOpen = false;
                }
            }
#endif

        }
    }
}
