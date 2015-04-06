
namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using System;
    using System.Windows;
#endif
#if WINRT
    using Windows.UI.Xaml;
#endif

    public abstract class SelectorWrapperMapping
    {
        public abstract SelectorWrapper CreateWrapper(DependencyObject control);

        public abstract bool CanCreateWrapper(DependencyObject control);
    }

    public sealed class SelectorWrapperMapping<TControl, TWrapper> : SelectorWrapperMapping where TWrapper : SelectorWrapper, new()
    {

        public override SelectorWrapper CreateWrapper(DependencyObject control)
        {
            var wrapper = new TWrapper();
            wrapper.InternalControl = control;
            return wrapper;
        }

        public override bool CanCreateWrapper(DependencyObject control)
        {
            return control is TControl;
        }
    }
}
