namespace BaseTools.UI.ControlExtensions
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Windows;
#if SILVERLIGHT
    using System.Windows.Controls;
#endif
#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif

    public abstract class SelectorWrapper
    {
        private DependencyObject internalControl;

        public abstract IList<object> SelectedItems { get; }

        public DependencyObject InternalControl
        {
            get
            {
                return internalControl;
            }

            set
            {
                this.internalControl = value;
                OnInitialize(value);
            }
        }

        public event SelectionChangedEventHandler SelectionChanged;

        protected virtual void OnInitialize(DependencyObject control)
        {
        }

        protected void OnRaiseSelectionChangedEvent(SelectionChangedEventArgs args)
        {
            var handler = this.SelectionChanged;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }
    }
}
