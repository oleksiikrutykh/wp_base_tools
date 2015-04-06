#region File and License Information
/*
<File>
	<Copyright>Copyright © 2010, Daniel Vaughan. All rights reserved.</Copyright>
	<License>
		Redistribution and use in source and binary forms, with or without
		modification, are permitted provided that the following conditions are met:
			* Redistributions of source code must retain the above copyright
			  notice, this list of conditions and the following disclaimer.
			* Redistributions in binary form must reproduce the above copyright
			  notice, this list of conditions and the following disclaimer in the
			  documentation and/or other materials provided with the distribution.
			* Neither the name of the <organization> nor the
			  names of its contributors may be used to endorse or promote products
			  derived from this software without specific prior written permission.

		THIS SOFTWARE IS PROVIDED BY <copyright holder> ''AS IS'' AND ANY
		EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED
		WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE ARE
		DISCLAIMED. IN NO EVENT SHALL <copyright holder> BE LIABLE FOR ANY
		DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL DAMAGES
		(INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES;
		LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND
		ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT
		(INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
		SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
	</License>
	<Owner Name="Daniel Vaughan" Email="dbvaughan@gmail.com"/>
	<CreationDate>2011-01-24 20:41:43Z</CreationDate>
</File>
*/
#endregion

namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Media;
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Media;
#endif

    using System;
    using System.Collections.Generic;
    using System.Windows;
    using System.Windows.Input;
    using BaseTools.Core.Utility;

    public static class ScrollViewerMonitor
    {
        public const int EmptyListBoxScrollHeight = 0;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Part of dependency property pattern")]
        public static DependencyProperty AtEndCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtEndCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                    new PropertyMetadata(null, OnCommandChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Part of dependency property pattern")]
        public static DependencyProperty AtHorizontalEndCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtHorizontalEndCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                new PropertyMetadata(null, OnCommandChanged));


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Part of dependency property pattern")]
        public static DependencyProperty AtStartCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtStartCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                new PropertyMetadata(null, OnCommandChanged));

         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible", Justification = "Part of dependency property pattern")]
        public static DependencyProperty AtHorizontalStartCommandProperty
            = DependencyProperty.RegisterAttached(
                "AtHorizontalStartCommand", typeof(ICommand),
                typeof(ScrollViewerMonitor),
                new PropertyMetadata(null, OnCommandChanged));

         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Attached property pattern")]
         public static readonly DependencyProperty VerticalOffsetMirrorProperty = DependencyProperty.RegisterAttached("VerticalOffsetMirror", typeof(double), typeof(ScrollViewerMonitor), new PropertyMetadata(0.0, OnScrollChanged));

         [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "Attached property pattern")]
         public static readonly DependencyProperty HorizontalOffsetMirrorProperty = DependencyProperty.RegisterAttached("HorizontalOffsetMirror", typeof(double), typeof(ScrollViewerMonitor), new PropertyMetadata(0.0, OnScrollChanged));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static ICommand GetAtEndCommand(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (ICommand)obj.GetValue(AtEndCommandProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetAtEndCommand(DependencyObject obj, ICommand value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(AtEndCommandProperty, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static ICommand GetAtHorizontalEndCommand(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (ICommand)obj.GetValue(AtHorizontalEndCommandProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetAtHorizontalEndCommand(DependencyObject obj, ICommand value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(AtHorizontalEndCommandProperty, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static ICommand GetAtStartCommand(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (ICommand)obj.GetValue(AtStartCommandProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetAtStartCommand(DependencyObject obj, ICommand value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(AtStartCommandProperty, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static ICommand GetAtHorizontalStartCommand(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (ICommand)obj.GetValue(AtHorizontalStartCommandProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static void SetAtHorizontalStartCommand(DependencyObject obj, ICommand value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(AtHorizontalStartCommandProperty, value);
        }

        public static void OnCommandChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var element = sender as ItemsControl;
            if (element == null)
            {
                throw new ArgumentException("Command must be attached to ItemsControl");
            }

            element.Loaded -= OnItemsControlLoaded;
            element.ApplyTemplate();
            bool attachWasSuccess = TryAttachToScrollEvent(element);
            if (!attachWasSuccess)
            {
                element.Loaded += OnItemsControlLoaded;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA2204:Literals should be spelled correctly", MessageId = "ScrollViewer", Justification = "This is control name.")]
        static void OnItemsControlLoaded(object sender, RoutedEventArgs e)
        {
            ItemsControl element = (ItemsControl)sender;
            element.Loaded -= OnItemsControlLoaded;
            TryAttachToScrollEvent(element);
        }

        private static bool TryAttachToScrollEvent(ItemsControl element)
        {
            Guard.CheckIsNotNull(element, "element");
            var scrollViewer = FindChildOfType<ScrollViewer>(element);
            bool scrollViewerFounded = scrollViewer != null;
            if (scrollViewerFounded)
            {
                var canScrollVertical = scrollViewer.VerticalScrollBarVisibility != ScrollBarVisibility.Disabled;
                if (canScrollVertical)
                {
                    var startCommand = GetAtStartCommand(element);
                    var endCommand = GetAtEndCommand(element);
                    if (startCommand != null || endCommand != null)
                    {
                        AttachToScrollProperty(element, scrollViewer, false);
                    }
                }

                var canScrollHorizontal = scrollViewer.HorizontalScrollBarVisibility != ScrollBarVisibility.Disabled;
                if (canScrollHorizontal)
                {
                    var startCommand = GetAtHorizontalEndCommand(element);
                    var endCommand = GetAtHorizontalEndCommand(element);
                    if (startCommand != null || endCommand != null)
                    {
                        AttachToScrollProperty(element, scrollViewer, true);
                    }
                }
            }

            return scrollViewerFounded;
        }

        private static void AttachToScrollProperty(ItemsControl control, ScrollViewer scrollViewer, bool isHorizontal)
        {
            string propertyName = "VerticalOffset";
            DependencyProperty scrollListenerProperty = VerticalOffsetMirrorProperty;
            if (isHorizontal)
            {
                propertyName = "HorizontalOffset";
                scrollListenerProperty = HorizontalOffsetMirrorProperty;
            }

            Binding binding = new Binding()
            {
                Path = new PropertyPath(propertyName),
                Source = scrollViewer
            };

            control.SetBinding(scrollListenerProperty, binding);
        }

        private static void OnScrollChanged(DependencyObject sender, DependencyPropertyChangedEventArgs e)
        {
            var itemsControl = (ItemsControl)sender;
            var scrollViewer = FindChildOfType<ScrollViewer>(itemsControl);

            bool isVerticalChange = e.Property == VerticalOffsetMirrorProperty;
            var totalSrollableSize = scrollViewer.ScrollableHeight;
            var currentPosition = scrollViewer.VerticalOffset;
            ICommand atStartCommand = GetAtStartCommand(itemsControl);
            var atEndCommand = GetAtEndCommand(itemsControl);
            if (!isVerticalChange)
            {
                totalSrollableSize = scrollViewer.ScrollableWidth;
                currentPosition = scrollViewer.HorizontalOffset;
                atStartCommand = GetAtHorizontalStartCommand(itemsControl);
                atEndCommand = GetAtHorizontalEndCommand(itemsControl);
            }

            var startOffsetLimit = 0.1;
            bool atBottom = currentPosition > totalSrollableSize - 2; 
            if (atBottom)
            {
                if (totalSrollableSize > EmptyListBoxScrollHeight)
                {
                    if (atEndCommand != null && atEndCommand.CanExecute(null))
                    {
                        atEndCommand.Execute(null);
                    }
                }
            }
            else if (currentPosition < startOffsetLimit)
            {
                if (atStartCommand != null && atStartCommand.CanExecute(null))
                {
                    atStartCommand.Execute(null);
                }
            }
        }

        static T FindChildOfType<T>(DependencyObject root) where T : class
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);

            while (queue.Count > 0)
            {
                DependencyObject current = queue.Dequeue();
                for (int i = VisualTreeHelper.GetChildrenCount(current) - 1; 0 <= i; i--)
                {
                    var child = VisualTreeHelper.GetChild(current, i);
                    var typedChild = child as T;
                    if (typedChild != null)
                    {
                        return typedChild;
                    }
                    queue.Enqueue(child);
                }
            }
            return null;
        }
    }
}
