namespace BaseTools.UI.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Markup;
    using System.Windows.Media;
    using Microsoft.Phone.Controls;
    using Microsoft.Phone.Shell;
    using BaseTools.Core.Utility;
    using BaseTools.UI.Common;
    using System.Threading.Tasks;
    
    /// <summary>
    /// Bindable app bar
    /// </summary>
    public class BindableApplicationBar : ItemsControl, IApplicationBar
    {
        public static readonly DependencyProperty IsVisibleProperty = DependencyProperty.Register("IsVisible", typeof(bool), typeof(BindableApplicationBar), new PropertyMetadata(true, OnVisibleChanged));

        public static readonly DependencyProperty IsMenuEnabledProperty = DependencyProperty.Register("IsMenuEnabled", typeof(bool), typeof(BindableApplicationBar), new PropertyMetadata(true, OnMenuEnabledChanged));

        public static readonly DependencyProperty BackgroundColorProperty = DependencyProperty.Register("BackgroundColor", typeof(Color), typeof(BindableApplicationBar), new PropertyMetadata(OnBackgroundColorChanged));

        public static readonly DependencyProperty ModeProperty = DependencyProperty.Register("Mode", typeof(ApplicationBarMode), typeof(BindableApplicationBar), new PropertyMetadata(OnModeChanged));

        public static readonly DependencyProperty BarOpacityProperty = DependencyProperty.Register("BarOpacity", typeof(double), typeof(BindableApplicationBar), new PropertyMetadata(1d, OnBarOpacityChanged));

        public static readonly DependencyProperty ForegroundColorProperty = DependencyProperty.Register("ForegroundColor", typeof(Color), typeof(BindableApplicationBar), new PropertyMetadata(OnForegroundColorChanged));

        public static readonly DependencyProperty MenuItemVisibilityProperty = DependencyProperty.RegisterAttached("MenuItemVisibility", typeof(bool), typeof(BindableApplicationBar), new PropertyMetadata(true, OnMenuItemVisibilityChanged));

        /// <summary>
        /// Max buttons count
        /// </summary>
        private const int MaxButtonsCount = 4;

        private ApplicationBar storeStateAppBar;

        private IApplicationBar displayedApplicationBar;

        private Queue<BindableApplicationBarIconButton> additionalButtonsQueue;

        public BindableApplicationBar()
        {
            this.MenuItems = new List<object>();
            this.Buttons = new List<object>();
            //this.storeStateAppBar = new ApplicationBar();
            this.additionalButtonsQueue = new Queue<BindableApplicationBarIconButton>();
            this.Loaded += new RoutedEventHandler(BindableApplicationBar_Loaded);
            this.Unloaded += new RoutedEventHandler(BindableApplicationBar_Unloaded);
        }

        #region Properties

        public bool IsVisible
        {
            get
            {
                return (bool)GetValue(IsVisibleProperty);
            }
            set
            {
                SetValue(IsVisibleProperty, value);
            }
        }

        public double BarOpacity
        {
            get
            {
                return (double)GetValue(BarOpacityProperty);
            }
            set
            {
                SetValue(BarOpacityProperty, value);
            }
        }

        public bool IsMenuEnabled
        {
            get
            {
                return (bool)GetValue(IsMenuEnabledProperty);
            }
            set
            {
                SetValue(IsMenuEnabledProperty, value);
            }
        }

        public Color BackgroundColor
        {
            get
            {
                return (Color)GetValue(BackgroundColorProperty);
            }
            set
            {
                SetValue(BackgroundColorProperty, value);
            }
        }

        public Color ForegroundColor
        {
            get
            {
                return (Color)GetValue(ForegroundColorProperty);
            }
            set
            {
                SetValue(ForegroundColorProperty, value);
            }
        }

        public IList Buttons { get; private set; }

        public IList MenuItems { get; private set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static bool GetMenuItemVisibility(DependencyObject obj)
        {
            Guard.CheckIsNotNull(obj, "obj");
            return (bool)obj.GetValue(MenuItemVisibilityProperty);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by guard")]
        public static void SetMenuItemVisibility(DependencyObject obj, bool value)
        {
            Guard.CheckIsNotNull(obj, "obj");
            obj.SetValue(MenuItemVisibilityProperty, value);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "В разработке.")]
        public double DefaultSize
        {
            get { throw new NotImplementedException(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "В разработке.")]
        public double MiniSize
        {
            get { throw new NotImplementedException(); }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Justification = "В разработке.")]
        public ApplicationBarMode Mode
        {
            get
            {
                return (ApplicationBarMode)GetValue(ModeProperty);
            }
            set
            {
                SetValue(ModeProperty, value);
            }
        }

#endregion

        public event EventHandler<ApplicationBarStateChangedEventArgs> StateChanged;

        private void BindableApplicationBar_Unloaded(object sender, RoutedEventArgs e)
        {
            this.displayedApplicationBar = null;
            this.additionalButtonsQueue.Clear();
            var page = this.GetVisualAncestors().Where(c => c is PhoneApplicationPage).LastOrDefault() as PhoneApplicationPage;
            foreach (BindableApplicationBarIconButton button in this.Buttons)
            {
                page.ApplicationBar.Buttons.Remove(button.Button);
            }

            foreach (BindableApplicationBarMenuItem menuItem in this.MenuItems)
            {
                page.ApplicationBar.MenuItems.Remove(menuItem.Item);
            }
            page.ApplicationBar.StateChanged -= new EventHandler<ApplicationBarStateChangedEventArgs>(OnInternalApplicationBarStateChanged);

            if (this.storeStateAppBar != null)
            {
                
                page.ApplicationBar.ForegroundColor = this.storeStateAppBar.ForegroundColor;
                page.ApplicationBar.IsMenuEnabled = this.storeStateAppBar.IsMenuEnabled;
                page.ApplicationBar.IsVisible = this.storeStateAppBar.IsVisible;
                page.ApplicationBar.Mode = this.storeStateAppBar.Mode;
                page.ApplicationBar.Opacity = this.storeStateAppBar.Opacity;
                ChangeRealBackgroudColor(page, this.storeStateAppBar.BackgroundColor);
            }
        }

        private void SetApplicationBar(BindableApplicationBar bindableAppBar)
        {
            Guard.CheckIsNotNull(bindableAppBar, "bindableAppBar");
            var page = this.GetVisualAncestors().Where(c => c is PhoneApplicationPage).LastOrDefault() as PhoneApplicationPage;
            if (page != null)
            {
                if (page.ApplicationBar != null)
                {
                    this.storeStateAppBar = new ApplicationBar();
                    this.storeStateAppBar.BackgroundColor = page.ApplicationBar.BackgroundColor;
                    this.storeStateAppBar.ForegroundColor = page.ApplicationBar.ForegroundColor;
                    this.storeStateAppBar.IsMenuEnabled = page.ApplicationBar.IsMenuEnabled;
                    this.storeStateAppBar.IsVisible = page.ApplicationBar.IsVisible;
                    this.storeStateAppBar.Mode = page.ApplicationBar.Mode;
                    this.storeStateAppBar.Opacity = page.ApplicationBar.Opacity;
                }
                else
                {
                    page.ApplicationBar = new ApplicationBar();
                }

                this.displayedApplicationBar = page.ApplicationBar;

                //page.ApplicationBar.BackgroundColor = this.BackgroundColor;
                page.ApplicationBar.ForegroundColor = this.ForegroundColor;
                page.ApplicationBar.IsMenuEnabled = this.IsMenuEnabled;
                page.ApplicationBar.IsVisible = this.IsVisible;
                page.ApplicationBar.Mode = this.Mode;
                page.ApplicationBar.Opacity = this.BarOpacity;
                page.ApplicationBar.StateChanged += new EventHandler<ApplicationBarStateChangedEventArgs>(OnInternalApplicationBarStateChanged);

                foreach (var button in this.Buttons)
                {
                    var iconButton = (BindableApplicationBarIconButton)button;
                    bool isButtonVisible = GetMenuItemVisibility(iconButton);
                    if (isButtonVisible)
                    {
                        this.AddButton(page.ApplicationBar, iconButton);
                    }
                }

                foreach (BindableApplicationBarMenuItem menuItem in this.MenuItems)
                {
                    bool isMenuItemVisible = GetMenuItemVisibility(menuItem);
                    if (isMenuItemVisible)
                    {
                        this.AddMenuItem(page.ApplicationBar, menuItem);
                    }
                }

                ChangeRealBackgroudColor(page, this.BackgroundColor);
            }
        }

        private void BindableApplicationBar_Loaded(object sender, RoutedEventArgs e)
        {
            SetApplicationBar(sender as BindableApplicationBar);
        }

        private static void OnVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                bindableApplicationBar.displayedApplicationBar.IsVisible = (bool)e.NewValue;
            }
        }

        private static void OnBackgroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                var page = bindableApplicationBar.GetVisualAncestors().Where(c => c is PhoneApplicationPage).LastOrDefault() as PhoneApplicationPage;
                ChangeRealBackgroudColor(page, (Color)e.NewValue);
            }
        }

        private static async void ChangeRealBackgroudColor(PhoneApplicationPage page, Color newColor)
        {
            var appBar = page.ApplicationBar;
            bool isOldTransparent = IsColorTransparent(appBar.BackgroundColor);
            bool isNewTransparent = IsColorTransparent(newColor);
            if (!isOldTransparent && isNewTransparent)
            {
                page.ApplicationBar = null;
                await Task.Delay(1);
                appBar.BackgroundColor = newColor;
                page.ApplicationBar = appBar;
            }
            else
            {
                appBar.BackgroundColor = newColor;
            }
        }

        private static bool IsColorTransparent(Color color)
        {
            return color.A != 255 && color.A != 0;
        }

        private static void OnModeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                bindableApplicationBar.displayedApplicationBar.Mode = (ApplicationBarMode)e.NewValue;
            }
        }

        private static void OnBarOpacityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                bindableApplicationBar.displayedApplicationBar.Opacity = (double)e.NewValue;
            }
        }

        private static void OnForegroundColorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                bindableApplicationBar.displayedApplicationBar.ForegroundColor = (Color)e.NewValue;
            }
        }

        private static void OnMenuEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var bindableApplicationBar = (BindableApplicationBar)d;
            if (bindableApplicationBar.displayedApplicationBar != null)
            {
                bindableApplicationBar.displayedApplicationBar.IsMenuEnabled = (bool)e.NewValue;
            }
        }

        private static void OnMenuItemVisibilityChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var menuItem = (BindableApplicationBarMenuItem)d;
            var appBar = menuItem.Parent;
            if ((appBar != null) && (appBar.Items.Contains(menuItem)))
            {
                var page = appBar.GetVisualAncestors().Where(c => c is PhoneApplicationPage).LastOrDefault() as PhoneApplicationPage;
                if ((page != null) && (page.ApplicationBar != null))
                {
                    bool isVisible = (bool)e.NewValue;
                    var button = menuItem as BindableApplicationBarIconButton;
                    if (button != null)
                    {
                        if (isVisible)
                        {
                            appBar.AddButton(page.ApplicationBar, button);
                        }
                        else
                        {
                            appBar.RemoveButton(page.ApplicationBar, button);
                        }
                    }
                    else
                    {
                        if (isVisible)
                        {
                            appBar.AddMenuItem(page.ApplicationBar, menuItem);
                        }
                        else
                        {
                            page.ApplicationBar.MenuItems.Remove(menuItem.Item);
                        }
                    }
                }
            }
        }

        private void ClearAdditionalButtonsQueue()
        {
            this.additionalButtonsQueue.Clear();
        }

        private void EnqueueButton(BindableApplicationBarIconButton button)
        {
            if (!this.additionalButtonsQueue.Contains(button))
            {
                this.additionalButtonsQueue.Enqueue(button);
            }
        }

        private void FillAppBarWithQueuedButtons(IApplicationBar appBar)
        {
            // If we have buttons queued for displaying in app bar, try to add them.
            if (this.additionalButtonsQueue.Count > 0)
            {
                while (appBar.Buttons.Count < MaxButtonsCount && this.additionalButtonsQueue.Count > 0)
                {
                    var button = this.additionalButtonsQueue.Dequeue();
                    bool isVisible = GetMenuItemVisibility(button);
                    if (isVisible)
                    {
                        this.AddButton(appBar, button);
                    }
                }
            }
        }

        private void AddButton(IApplicationBar appBar, BindableApplicationBarIconButton button)
        {
            bool alreadyContains = appBar.Buttons.Contains(button.Button);
            if (!alreadyContains)
            {
                if (appBar.Buttons.Count < MaxButtonsCount)
                {
                    this.AddMenuItem(appBar, button);
                }
                else
                {
                    this.EnqueueButton(button);
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Can't be static")]
        private void AddMenuItem(IApplicationBar appBar, BindableApplicationBarMenuItem menuItem)
        {
            var bindableAppBar = menuItem.Parent;
            IList realItems = appBar.MenuItems;
            IList bindableItems = bindableAppBar.MenuItems;
            if (menuItem is BindableApplicationBarIconButton)
            {
                realItems = appBar.Buttons;
                bindableItems = bindableAppBar.Buttons;
            }

            bool alreadyContains = realItems.Contains(menuItem.Item);
            if (!alreadyContains)
            {
                int originalPosition = bindableItems.IndexOf(menuItem);

                // Find index of next button in app bar.
                int insertPosition = -1;

                // Check all buttons that was next in app bar.
                for (int i = originalPosition + 1; i < bindableItems.Count; i++)
                {
                    var nextItem = (BindableApplicationBarMenuItem)bindableItems[i];
                    bool isVisible = GetMenuItemVisibility(nextItem);
                    if (isVisible)
                    {
                        var index = realItems.IndexOf(nextItem.Item);
                        // if next button displayed in app bar, put current button before it.
                        if (index != -1)
                        {
                            insertPosition = index;
                            break;
                        }
                    }
                }

                try
                {
                    if (insertPosition == -1)
                    {
                        realItems.Add(menuItem.Item);
                    }
                    else
                    {
                        // Put button into app bar before first button, that already displayed.
                        realItems.Insert(insertPosition, menuItem.Item);
                    }
                }
                catch (Exception ex)
                {
                    // Error occured when application start closing. 
                    // We need handle this error.
                    if (!ex.Message.Contains("Item could not be inserted"))
                    {
                        var currentSource = ((PhoneApplicationFrame)Application.Current.RootVisual).CurrentSource;
                        var message = ex.Message + " On \"" + currentSource + "\"";
                        throw new BindableApplicationBarException(message);
                    }
                }
            }
        }

        private void RemoveButton(IApplicationBar appBar, BindableApplicationBarIconButton button)
        {
            // Delete button from displayed app bar.
            appBar.Buttons.Remove(button.Button);

            // If we have buttons queued for displaying in app bar, try to add them
            this.FillAppBarWithQueuedButtons(appBar);
        }

        protected override void OnItemsChanged(System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            this.Buttons = new List<object>();
            this.MenuItems =  new List<object>();
            foreach (BindableApplicationBarMenuItem item in this.Items)
            {
                item.Parent = this;
                if (item is BindableApplicationBarIconButton)
                {
                    this.Buttons.Add(item);
                }
                else if (item is BindableApplicationBarMenuItem)
                {
                    this.MenuItems.Add(item);
                }
            }
        }

        void OnInternalApplicationBarStateChanged(object sender, ApplicationBarStateChangedEventArgs e)
        {
            var eventHandler = this.StateChanged;
            if (eventHandler != null)
            {
                eventHandler(this, e);
            }
        }
    }
}