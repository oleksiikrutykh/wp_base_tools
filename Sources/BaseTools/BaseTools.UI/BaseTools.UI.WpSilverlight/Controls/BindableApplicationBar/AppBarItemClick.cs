namespace BaseTools.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.Phone.Shell;
    using Microsoft.Phone.Tasks;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Static Class that holds all Dependency Properties and Static methods to allow 
    /// the Click event of the IApplicationBarMenuItem interface to be attached to a Command. 
    /// </summary>
    /// <remarks>
    /// This class is required, because Silverlight for WinPhone doesn't have native support for Commands. 
    /// </remarks>
    public static class AppBarItemClick
    {
        /// <summary>
        /// Command to execute on click event.
        /// </summary>
        public static readonly DependencyProperty NavigationUriProperty = DependencyProperty.RegisterAttached(
                "NavigationUri",
                typeof(Uri),
                typeof(AppBarItemClick),
                new PropertyMetadata(OnSetNavigationUriCallback));


        /// <summary>
        /// Command to execute on click event.
        /// </summary>
        public static readonly DependencyProperty CommandProperty = DependencyProperty.RegisterAttached(
                "Command",
                typeof(ICommand),
                typeof(AppBarItemClick),
                new PropertyMetadata(OnSetCommandCallback));

        /// <summary>
        /// Command parameter to supply on command execution.
        /// </summary>
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.RegisterAttached(
                "CommandParameter",
                typeof(object),
                typeof(AppBarItemClick),
                new PropertyMetadata(OnSetCommandParameterCallback));

        /// <summary>
        /// Command behavior that is raise by click.
        /// </summary>
        private static readonly DependencyProperty ClickCommandBehaviorProperty = DependencyProperty.RegisterAttached(
                "ClickCommandBehavior",
                typeof(AppBarItemCommandBehavior<IApplicationBarMenuItem>),
                typeof(AppBarItemClick),
                null);

        /// <summary>
        /// Sets the <see cref="Uri"/> to execute on the click event.
        /// </summary>
        /// <param name="item">AppBarItem dependency object to attach navigation uri</param>
        /// <param name="command">Navigation uri to attach</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static void SetNavigationUri(IApplicationBarMenuItem item, Uri value)
        {
            (item as FrameworkElement).SetValue(NavigationUriProperty, value);
        }

        /// <summary>
        /// Retrieves the <see cref="ICommand"/> attached to the <see cref="IApplicationBarMenuItem"/>.
        /// </summary>
        /// <param name="item">IApplicationBarMenuItem containing the Command dependency property</param>
        /// <returns>The value of the command attached</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static Uri GetNavigationUri(IApplicationBarMenuItem item)
        {
            var frameworkElement = item as FrameworkElement;
            var uri = (Uri)frameworkElement.GetValue(NavigationUriProperty);
            return uri;
        }


        /// <summary>
        /// Sets the <see cref="ICommand"/> to execute on the click event.
        /// </summary>
        /// <param name="item">AppBarItem dependency object to attach command</param>
        /// <param name="command">Command to attach</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static void SetCommand(IApplicationBarMenuItem item, ICommand command)
        {
            (item as FrameworkElement).SetValue(CommandProperty, command);
        }

        /// <summary>
        /// Retrieves the <see cref="ICommand"/> attached to the <see cref="IApplicationBarMenuItem"/>.
        /// </summary>
        /// <param name="item">IApplicationBarMenuItem containing the Command dependency property</param>
        /// <returns>The value of the command attached</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static ICommand GetCommand(IApplicationBarMenuItem item)
        {
            return (item as FrameworkElement).GetValue(CommandProperty) as ICommand;
        }

        /// <summary>
        /// Sets the value for the CommandParameter attached property on the provided <see cref="IApplicationBarMenuItem"/>.
        /// </summary>
        /// <param name="item">IApplicationBarMenuItem to attach CommandParameter</param>
        /// <param name="parameter">Parameter value to attach</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static void SetCommandParameter(IApplicationBarMenuItem item, object parameter)
        {
            (item as FrameworkElement).SetValue(CommandParameterProperty, parameter);
        }

        /// <summary>
        /// Gets the value in CommandParameter attached property on the provided <see cref="IApplicationBarMenuItem"/>
        /// </summary>
        /// <param name="item">IApplicationBarMenuItem that has the CommandParameter</param>
        /// <returns>The value of the property</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "Only works for IApplicationBarMenuItem")]
        public static object GetCommandParameter(IApplicationBarMenuItem item)
        {
            return (item as FrameworkElement).GetValue(CommandParameterProperty);
        }

        /// <summary>
        /// Raise when callback is setted to command.
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void OnSetCommandCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            IApplicationBarMenuItem item = dependencyObject as IApplicationBarMenuItem;
            if (item != null)
            {
                AppBarItemCommandBehavior<IApplicationBarMenuItem> behavior = GetOrCreateBehavior(item);
                behavior.Command = e.NewValue as ICommand;
            }
        }

        /// <summary>
        /// Raise when navigation uri changed.
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void OnSetNavigationUriCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            var associatedMenuItem = (IApplicationBarMenuItem)dependencyObject;
            if (e.OldValue != null)
            {
                associatedMenuItem.Click -= new EventHandler(NavigateToUriOnMenuItemClicked);
            }
            
            if (e.NewValue != null)
            {
                associatedMenuItem.Click += new EventHandler(NavigateToUriOnMenuItemClicked);
            }
        }

        /// <summary>
        /// Occured when menu item was clicked. Perform navigation to Uri here.
        /// </summary>
        /// <param name="sender">Menu item that was clicked.</param>
        /// <param name="e">Info about click event</param>
        private static void NavigateToUriOnMenuItemClicked(object sender, EventArgs e)
        {
            var associatedMenuItem = (IApplicationBarMenuItem)sender;
            var navigationUri = GetNavigationUri(associatedMenuItem);
            if (navigationUri != null)
            {
                if (navigationUri.IsAbsoluteUri)
                {
                    var webBrowserTask = new WebBrowserTask();
                    webBrowserTask.Uri = navigationUri;
                    webBrowserTask.Show();
                }
                else
                {
                    var rootFrame = (PhoneApplicationFrame)Application.Current.RootVisual;
                    rootFrame.Navigate(navigationUri);
                }
            }
        }

        /// <summary>
        /// Raise when set command parameters.
        /// </summary>
        /// <param name="dependencyObject">Dependency object</param>
        /// <param name="e">Event arguments</param>
        private static void OnSetCommandParameterCallback(DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            IApplicationBarMenuItem item = dependencyObject as IApplicationBarMenuItem;
            if (item != null)
            {
                AppBarItemCommandBehavior<IApplicationBarMenuItem> behavior = GetOrCreateBehavior(item);
                behavior.CommandParameter = e.NewValue;
            }
        }

        /// <summary>
        /// Gets or creats clickCommandBehavior.
        /// </summary>
        /// <param name="item">App bar menu item.</param>
        /// <returns>Created or geted behavior.</returns>
        private static AppBarItemCommandBehavior<IApplicationBarMenuItem> GetOrCreateBehavior(IApplicationBarMenuItem item)
        {
            var frameworkElement = item as FrameworkElement;
            AppBarItemCommandBehavior<IApplicationBarMenuItem> behavior = frameworkElement.GetValue(ClickCommandBehaviorProperty) as AppBarItemCommandBehavior<IApplicationBarMenuItem>;
            if (behavior == null)
            {
                behavior = new AppBarItemCommandBehavior<IApplicationBarMenuItem>(item);
                frameworkElement.SetValue(ClickCommandBehaviorProperty, behavior);
            }

            return behavior;
        }
    }
}