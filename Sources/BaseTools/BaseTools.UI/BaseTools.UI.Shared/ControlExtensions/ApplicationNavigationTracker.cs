namespace BaseTools.UI.ControlExtensions
{
#if SILVERLIGHT
    using Microsoft.Phone.Controls;
    using System.Windows.Controls;
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
#endif

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using BaseTools.Core.Utility;
    using BaseTools.UI.Navigation;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Info;

    public static class ApplicationNavigationTracker 
    {
        private static IApplicationNavigationProvider navigationProvider;

#if WINRT
        private static ApplicationNavigationProvider winrtNavigationProvider;
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification="Initialize fields by function in contructor")]
        static ApplicationNavigationTracker()
        {
            Initialize();
        }

        /// <summary>
        /// Command executed when OnNavigatedTo page method called.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable")]
        public static readonly DependencyProperty NavigatedToPageCommandProperty = DependencyProperty.RegisterAttached("NavigatedToPageCommand", typeof(ICommand), typeof(ApplicationNavigationTracker), new PropertyMetadata(null));

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Validate by Guard")]
        public static ICommand GetNavigatedToPageCommand(DependencyObject control)
        {
            Guard.CheckIsNotNull(control, "control");
            return (ICommand)control.GetValue(NavigatedToPageCommandProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public static void SetNavigatedToPageCommand(DependencyObject control, ICommand command)
        {
            Guard.CheckIsNotNull(control, "control");
            control.SetValue(NavigatedToPageCommandProperty, command);
        }

        private static void Initialize()
        {
            if (!EnvironmentInfo.Current.IsInDesignMode)
            {
                navigationProvider = Factory.Common.GetInstance<IApplicationNavigationProvider>();
                navigationProvider.Navigated -= OnNavigated;
                navigationProvider.Navigated += OnNavigated;

#if WINRT
                winrtNavigationProvider = navigationProvider as ApplicationNavigationProvider;
#endif
            }
        }

        private static void OnNavigated(object sender, NavigationProviderEventArgs e)
        {
            bool canHandle = true;
#if WINRT
            //if navigationStack is restoring now - not call navigation commands.
            canHandle = !winrtNavigationProvider.IsStackRestoringNow;
#endif
            if (canHandle)
            {
                var page = e.ToEntry.Content as Page;
                if (page != null)
                {
                    var command = GetNavigatedToPageCommand(page);
                    if (command != null)
                    {
                        var canExecute = command.CanExecute(e);
                        if (canExecute)
                        {
                            command.Execute(e);
                        }
                    }
                }
            }
        }
    }
}
