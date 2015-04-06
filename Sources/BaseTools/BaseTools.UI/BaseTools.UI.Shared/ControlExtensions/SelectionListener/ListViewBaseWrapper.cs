namespace BaseTools.UI.ControlExtensions
{
#if WINRT
    using System.Collections.Generic;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    public class ListViewBaseWrapper : SelectorWrapper
    {
        private ListViewBase internalControl;

        protected override void OnInitialize(DependencyObject control)
        {
            base.OnInitialize(control);
            this.internalControl = (ListViewBase)control;
            this.internalControl.SelectionChanged += OnSelectionChanged;
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this.OnRaiseSelectionChangedEvent(e);
        }

        public override IList<object> SelectedItems
        {
            get
            {
                return this.internalControl.SelectedItems;
            }
        }
    }
#endif

}
