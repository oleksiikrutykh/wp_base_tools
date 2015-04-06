namespace BaseTools.UI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows;

    public class BindingChangedEventArgs : EventArgs
    {
        internal BindingChangedEventArgs(DependencyPropertyChangedEventArgs changedArgs)
        {
            this.OldValue = changedArgs.OldValue;
            this.NewValue = changedArgs.NewValue;
        }

        public object OldValue { get; private set; }

        public object NewValue { get; private set; }
    }
}
