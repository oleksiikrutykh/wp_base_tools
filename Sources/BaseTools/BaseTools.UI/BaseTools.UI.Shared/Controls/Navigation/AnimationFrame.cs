namespace BaseTools.UI.Controls
{
    using BaseTools.UI.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows.Controls;
    using BaseTools.Core.Utility;
    using BaseTools.Core.Threading;

    public class AnimationFrame : ContentControl
    {
        private const string CurrentContainerName = "currentContainer";

        private const string NextContainerName = "nextContainer";

        private List<PageEntry> navigationStack = new List<PageEntry>();

        private ContentPresenter currentContainer;

        private ContentPresenter nextContainer;

        private TaskCompletionSource<bool> controlReadyWaiter = new TaskCompletionSource<bool>();
        private bool isNavigationExecuted;

        private PageEntry currentEntry;


        private PageEntry nextEntry;

        public event EventHandler Navigating;

        public event EventHandler NavigationFailed;

        public event EventHandler Navigated;

        public event EventHandler NavigationCancelled;


        public bool CanGoBack
        {
            get
            {
                return this.navigationStack.Count > 0;
            }
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this.currentContainer = (ContentPresenter)this.GetTemplateChild(CurrentContainerName);
            this.nextContainer = (ContentPresenter)this.GetTemplateChild(NextContainerName);
            Canvas.SetZIndex(this.currentContainer, 1);
            Canvas.SetZIndex(this.nextContainer, 0);
            this.controlReadyWaiter.TrySetResult(true);
        }

        public void Navigate(Type pageType)
        {
            this.Navigate(pageType, null);
        }

        public async void Navigate(Type pageType, object parameter)
        {
            if (!this.isNavigationExecuted)
            {
                this.isNavigationExecuted = true;
                await this.controlReadyWaiter.Task;
                try
                {
                    this.nextEntry = new PageEntry
                    {
                        PageType = pageType,
                        Parameter = parameter,
                    };

                    this.navigationStack.Add(this.nextEntry);
                    // TODO: handle cancel event.
                    this.Navigating.CallEvent(this, EventArgs.Empty);
                    var newPage = Activator.CreateInstance(pageType);
                    this.nextContainer.Visibility = System.Windows.Visibility.Visible;
                    this.nextContainer.Content = newPage;
                    this.nextEntry.CachedContent = newPage;

                    await SynchronizationContextProvider.PostAsync(() =>
                    {
                        this.Navigated.CallEvent(this, EventArgs.Empty);
                    });
                }
                catch (Exception ex)
                {
                    this.NavigationFailed.CallEvent(this, EventArgs.Empty);
                }

                await this.PlayTransition();
            }
        }

        private 

        private async Task PlayTransition()
        {
            if (this.currentContainer.Content != null)
            {
                // play transition.
            }

            var oldContainer = this.currentContainer;
            this.currentContainer = this.nextContainer;
            this.nextContainer = oldContainer;
            oldContainer.Content = null;
            oldContainer.Visibility = System.Windows.Visibility.Collapsed;
            Canvas.SetZIndex(this.currentContainer, 1);
            Canvas.SetZIndex(this.nextContainer, 0);
        }

    }

    public class PageEntry
    {
        public Type PageType { get; set; }

        public object Parameter { get; set; }

        public object CachedContent { get; set; }
    }



}
