namespace BaseTools.UI.Navigation
{
#if WINRT
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Navigation;
#endif

#if SILVERLIGHT
    using Microsoft.Phone.Shell;
    using System.Windows.Controls;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
#endif

    using System;
    using System.Linq;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Text;
    using System.Reflection;
    using BaseTools.Core.Utility;
    using BaseTools.Core.Threading;

    public class ApplicationNavigationProvider : IApplicationNavigationProvider
    {
        private const string UniqueParameterName = "navigation_provider_page_index";

        private Frame frame;

        private NavigationEntry currentEntry;

        private NavigationEntry targetEntry;

        private PathNavigationMapper mapper;

        private NavigationMode targetNavigatingMode;

        private List<NavigationEntry> navigationStack = new List<NavigationEntry>();

        private bool isNavigationExecutingNow;

        private bool alreadyInitialized;

        private ExternalNavigationListener externalNavigationListener = new ExternalNavigationListener();

#if SILVERLIGHT
        private bool isInternalGoBackExecuted;
#endif

        public ApplicationNavigationProvider()
        {
#if SILVERLIGHT
            // Add root entity. 
            this.currentEntry = new NavigationEntry
            {
                IsUnknown = true
            };

            navigationStack.Add(this.currentEntry);
#endif
            this.targetEntry = currentEntry;
            this.NavigationStackSerializer = new NavigationHistoryStorageProvider();
        }

        public INavigationHistoryStorageProvider NavigationStackSerializer { get; set; }

        public event EventHandler<NavigationProviderEventArgs> Navigated;

        public event EventHandler<NavigationProviderCancellableEventArgs> Navigating;

        public event NavigationProviderEventHandler NavigationCanceled;

        public event EventHandler<NavigationEntryRemovedEventArgs> BackEntryRemoved;

        public object RootUIElement
        {
            get
            {
                return this.frame;
            }
        }

        public NavigationEntry TargetEntry
        {
            get
            {
                return this.targetEntry;
            }
        }

        public NavigationEntry CurrentEntry
        {
            get
            {
                return this.currentEntry;
            }
        }

        public IReadOnlyList<NavigationEntry> NavigationStack
        {
            get
            {
                return this.navigationStack;
            }
        }

        public int BackStackDepth
        {
            get
            {
                var depth = this.navigationStack.Count;
                //if (this.isNavigationExecutingNow)
                //{
                //    depth++;
                //}

                return depth;
            }
        }

        public Frame AssociatedFrame
        {
            get
            {
                return this.frame;
            }
        }

#if WINRT
        public bool IsStackRestoringNow { get; set; }
#endif

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "1", Justification = "Validated by Guard.")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "2", Justification = "Validated by Guard.")]
        public void Initialize(Frame associatedFrame, PathNavigationMapper newMapper)
        {
            Guard.CheckIsNotNull(associatedFrame, "associatedFrame");
            Guard.CheckIsNotNull(newMapper, "newMapper");
            if (!this.alreadyInitialized)
            {
                this.mapper = newMapper;
                this.frame = associatedFrame;
                this.frame.Navigating += this.OnFrameNavigating;
                this.frame.Navigated += this.OnFrameNavigated;
                this.frame.NavigationStopped += this.OnFrameNavigationStopped;
                this.frame.NavigationFailed += this.OnFrameNavigationFailed;

#if SILVERLIGHT
                var phoneFrame = (Microsoft.Phone.Controls.PhoneApplicationFrame)frame;
                phoneFrame.JournalEntryRemoved += OnJournalEntryRemoved;
#endif

                this.alreadyInitialized = true;
            }
        }

        public void Navigate(object navigationSource)
        {
            this.Navigate(navigationSource, null);
        }

        public void RemoveBackEntry()
        {
#if SILVERLIGHT
            var phoneFrame = (PhoneApplicationFrame)frame;
            if (phoneFrame.BackStack.Count() > 0)
            {
                phoneFrame.RemoveBackEntry();
            }
#endif

#if WINRT
            if (this.frame.BackStackDepth > 0)
            {
                var typeInfo = typeof(Frame).GetTypeInfo();
                var backStackProperty = typeInfo.GetDeclaredProperty("BackStack");
                var backStack = backStackProperty.GetValue(this.frame);
                var backStackType = backStackProperty.PropertyType;
                var removeAtMethod = backStackType.GetTypeInfo().GetDeclaredMethod("RemoveAt");
                var parameters = new object[1] { this.frame.BackStackDepth - 1 };
                removeAtMethod.Invoke(backStack, parameters);

                // Check indexes in WIn81
                var latestItemIndex = this.navigationStack.Count - 2;
                var removedEntity = this.navigationStack[latestItemIndex];
                this.navigationStack.RemoveAt(latestItemIndex);

                var handler = this.BackEntryRemoved;
                if (handler != null)
                {
                    var args = new NavigationEntryRemovedEventArgs(removedEntity);
                    handler.Invoke(this, args);
                }
            }
#endif
        }

        public void Navigate(object navigationSource, object parameters)
        {
            if (!this.isNavigationExecutingNow)
            {
                var navigationEntry = new NavigationEntry();
                navigationEntry.Source = navigationSource;
                navigationEntry.Parameter = parameters;
                this.targetEntry = navigationEntry;
#if SILVERLIGHT
                var uri = this.BuildUri(navigationSource);
                this.frame.Navigate(uri);
#endif

#if WINRT
                var pageType = this.ConvertToPageType(navigationSource);
                this.frame.Navigate(pageType, parameters);
#endif
            }
        }

        private void OnFrameNavigating(object sender, NavigatingCancelEventArgs e)
        {
#if SILVERLIGHT
            var phoneFrame = (PhoneApplicationFrame)frame;
            var currentBackStackCount = phoneFrame.BackStack.Count();
#endif
            this.isNavigationExecutingNow = true;
            var navigationMode = NavigationModeConverter.ConvertFromSystem(e.NavigationMode);
            this.targetNavigatingMode = navigationMode;
            NavigationEntry fromEntry = null;
            if (this.navigationStack.Count > 0)
            {
                fromEntry = this.navigationStack[this.navigationStack.Count - 1];
            }

            if (navigationMode == NavigationMode.New || navigationMode == NavigationMode.Forward)
            {
                bool isProviderNavigation = this.targetEntry != this.currentEntry;
                if (!isProviderNavigation)
                {
#if SILVERLIGHT
                    this.targetEntry = this.CreateNavigationEntry(e.Uri);
#endif

#if WINRT
                    // Can't load page parameter here. Can use parameters from navigated event only. 
                this.targetEntry = this.CreateNavigationEntry(e.SourcePageType, null);
#endif
                }

                this.navigationStack.Add(this.targetEntry);
            }
            else
            {
                this.targetEntry = null;
                if (this.navigationStack.Count > 0)
                {
                    this.navigationStack.RemoveAt(this.navigationStack.Count - 1);
                    this.targetEntry = this.navigationStack[this.navigationStack.Count - 1];
                }
            }

            this.externalNavigationListener.HandleNavigatingEvent(e);

            var handler = this.Navigating;
            if (handler != null)
            {
                var navigatingEventArgs = new NavigationProviderCancellableEventArgs(fromEntry, this.targetEntry, navigationMode);
                handler.Invoke(this, navigatingEventArgs);
                bool isCancelled = navigatingEventArgs.IsCanceled;
                if (isCancelled)
                {
#if SILVERLIGHT
                    if (!ExternalNavigationListener.IsExternalUri(e.Uri))
#endif
                    {
                        e.Cancel = true;
                    }
                }
            }
#if SILVERLIGHT
            SynchronizationContextProvider.PostAsync(() =>
            {
                if (navigationMode == NavigationMode.Back && e.IsCancelable)
                {
                    var afterNavigationBackstackCount = phoneFrame.BackStack.Count();

                    var difference = currentBackStackCount - afterNavigationBackstackCount;
                    if (difference == 0)
                    {
                        NavigationEventArgs handleNavigationEventArgs = new NavigationEventArgs(null, e.Uri, e.NavigationMode, false);
                        this.OnFrameNavigationStopped(sender, handleNavigationEventArgs);
                    }
                }
            });

#endif
        }

        private void OnFrameNavigationStopped(object sender, NavigationEventArgs e)
        {
            this.isNavigationExecutingNow = false;
            var navigationMode = NavigationModeConverter.ConvertFromSystem(e.NavigationMode);
            this.RollbackNavigation(navigationMode);
            this.externalNavigationListener.HandleNavigationNotSuccess();
            var handler = this.NavigationCanceled;
            var toEntry = this.targetEntry;
            this.targetEntry = this.currentEntry;
            if (handler != null)
            {
                var args = new NavigationProviderEventArgs(this.currentEntry, toEntry, navigationMode);
                handler.Invoke(this, args);
            }   
        }

        private void RollbackNavigation(NavigationMode navigationMode)
        {
            if (navigationMode == NavigationMode.New || navigationMode == NavigationMode.Reset)
            {
                this.navigationStack.RemoveAt(this.navigationStack.Count - 1);
            }
            else
            {
                this.navigationStack.Add(this.currentEntry);
            }
        }


        private void OnFrameNavigationFailed(object sender, NavigationFailedEventArgs e)
        {
            this.isNavigationExecutingNow = false;
            this.RollbackNavigation(this.targetNavigatingMode);
            this.targetEntry = this.currentEntry;
            this.externalNavigationListener.HandleNavigationNotSuccess();
        }

        private void OnFrameNavigated(object sender, NavigationEventArgs e)
        {
            this.isNavigationExecutingNow = false;
            var previousEntity = this.currentEntry;
            this.currentEntry = this.targetEntry;
            var navigationMode = NavigationModeConverter.ConvertFromSystem(e.NavigationMode);
            
            

//            var content = e.Content; 
//#if WINRT
//            var storableFrame = frame as NavigationStorableFrame;
//            if (storableFrame != null)
//            {
//                content = storableFrame.RealContent;
//            }
            //#endif
#if SILVERLIGHT
            if (!ExternalNavigationListener.IsExternalUri(e.Uri))
#endif
            {
                this.currentEntry.Content = frame.Content;
            }


#if WINRT
            this.currentEntry.Parameter = (NavigationParameters)e.Parameter;
#endif

#if SILVERLIGHT
            if (navigationMode == NavigationMode.Back && this.externalNavigationListener.State != ExternalNavigationState.Returning)
            {
                this.isInternalGoBackExecuted = true;
            }
#endif

            this.externalNavigationListener.HandleNavigatedEvent();

            var handler = this.Navigated;
            if (handler != null)
            {
                var args = new NavigationProviderEventArgs(previousEntity, this.currentEntry, navigationMode);
                handler.Invoke(this, args);
            }
        }


#if SILVERLIGHT

        private Uri BuildUri(object source)
        {
            PathNavigationMapItem mappingItem = this.mapper.GetMapping(source);
            if (mappingItem == null)
            {
                var exceptionMessage = String.Format(CultureInfo.CurrentCulture, "Source {0} not mapped to any page", source.ToString());
                throw new ArgumentException(exceptionMessage, "source");
            }

            string pagePath = mappingItem.PagePath;
            var uriParameters = new Dictionary<string, string>();

            uriParameters[UniqueParameterName] = Guid.NewGuid().ToString();

            var parametersAdditionalSymbol = "?";
            if (pagePath.Contains("?"))
            {
                parametersAdditionalSymbol = "&";
            }

            var uriBuilder = new StringBuilder(pagePath + parametersAdditionalSymbol);

            foreach (var item in uriParameters)
            {
                uriBuilder.Append(item.Key);
                uriBuilder.Append("=");
                var escapedValue = Uri.EscapeDataString(item.Value);
                uriBuilder.Append(escapedValue);
                uriBuilder.Append("&");
            }

            var uri = new Uri(uriBuilder.ToString(), UriKind.Relative);
            return uri;
        }

        private NavigationEntry CreateNavigationEntry(Uri uri)
        {
            var navigationEntry = new NavigationEntry();
            navigationEntry.IsUnknown = ExternalNavigationListener.IsExternalUri(uri);
            if (!navigationEntry.IsUnknown)
            {
                var uriSource = uri.OriginalString;
                string pagePath = uriSource;
                string parametersPart = String.Empty;
                var indexOfParameterSeparator = uriSource.IndexOf('?');
                if (indexOfParameterSeparator != -1)
                {
                    pagePath = uriSource.Substring(0, indexOfParameterSeparator);
                    var parametersStringStartIndex = indexOfParameterSeparator + 1;
                    if (parametersStringStartIndex < uriSource.Length)
                    {
                        parametersPart = uriSource.Substring(parametersStringStartIndex, uriSource.Length - parametersStringStartIndex);
                    }
                }

                pagePath = pagePath.Replace('\\', '/');
                PathNavigationMapItem mapping = this.mapper.GetMapping(pagePath);
                if (mapping != null)
                {
                    navigationEntry.Source = mapping.Source;
                    if (mapping.ParameterType != null)
                    {
                        var parameter = (NavigationParameters)Activator.CreateInstance(mapping.ParameterType);
                        var parameters = new Dictionary<string, string>();
                        var parametersKeyValuesString = parametersPart.Split('&');

                        foreach (var keyValueString in parametersKeyValuesString)
                        {
                            var keyValuePair = keyValueString.Split('=');
                            string key = keyValuePair[0];
                            string value = String.Empty;
                            if (keyValuePair.Length > 1)
                            {
                                value = keyValuePair[1];
                            }

                            parameters[key] = value;
                        }

                        parameter.InitializeFromUriParameters(parameters);

                        navigationEntry.Parameter = parameter;
                    }
                }
                else
                {
                    // Frame can open pages that was not mapped to any source. This can be third-party controls. 
                    navigationEntry.IsUnknown = true;
                }
            }

            return navigationEntry;
        }

        private void OnJournalEntryRemoved(object sender, JournalEntryRemovedEventArgs e)
        {
            // Event can occured when user simply go back or call RemoveBackEntry();
            if (!this.isInternalGoBackExecuted)
            {
                // RemoveBackEntry() called on frame.
                var removedIndex = this.navigationStack.Count - 2;

                var removedEnrty = this.navigationStack[removedIndex];
                this.navigationStack.RemoveAt(removedIndex);

                var handler = this.BackEntryRemoved;
                if (handler != null)
                {
                    var args = new NavigationEntryRemovedEventArgs(removedEnrty);
                    handler.Invoke(this, args);
                }
            }
            else
            {
                //var handler = this.BackEntryRemoved;
                //if (handler != null)
                //{
                //    var args = new NavigationEntryRemovedEventArgs(this.navigationStack.);
                //    handler.Invoke(this, args);
                //}

                // GoBack() called on frame.
                this.isInternalGoBackExecuted = false;
            }
        }

#endif

#if WINRT
        private Type ConvertToPageType(object source)
        {
            var mapping = mapper.GetMapping(source);
            if (mapping == null)
            {
                var exceptionMessage = String.Format(CultureInfo.CurrentCulture, "Source {0} not mapped to any page", source.ToString());
                throw new ArgumentException(exceptionMessage, "source");
            }

            return mapping.PageType;
        }

        private NavigationEntry CreateNavigationEntry(Type pageType, NavigationParameters parameter)
        {
            var entry = new WinRTNavigationEntry();
            var mapping = this.mapper.GetMapping(pageType);
            if (mapping != null)
            {
                entry.Source = mapping.Source;
            }
            else
            {
                // Frame can open pages that was not mapped to any source. This can be third-party controls. 
                entry.IsUnknown = true;
            }

            entry.PageType = pageType.AssemblyQualifiedName;
            entry.Parameter = parameter;
            return entry;
        }
#endif


        private void Restore() 
        {
            var storedNavigationStack = this.NavigationStackSerializer.RestoreHistory();
            bool isSuccessfullyRestored = storedNavigationStack.Count > 0;
            if (isSuccessfullyRestored)
            {
#if SILVERLIGHT
                this.navigationStack = storedNavigationStack;
                this.currentEntry = this.navigationStack[this.navigationStack.Count - 1];
                this.targetEntry = this.currentEntry;
                this.externalNavigationListener.HandleRestoring();
#endif
#if WINRT
                if (this.alreadyInitialized)
                {
                    this.IsStackRestoringNow = true;
                    try
                    {
                        var lastItemIndex = storedNavigationStack.Count - 1;
                        for (int i = 0; i < storedNavigationStack.Count; i++)
                        {
                            if (i == lastItemIndex)
                            {
                                this.IsStackRestoringNow = false;
                            }

                            var entry = storedNavigationStack[i];
                            if (!entry.IsUnknown)
                            {
                                this.Navigate(entry.Source, entry.Parameter);
                            }
                            else
                            {
                                var winrtEntry = (WinRTNavigationEntry)entry;
                                var pageType = Type.GetType(winrtEntry.PageType);
                                this.frame.Navigate(pageType, entry.Parameter);
                            }
                        }
                    }
                    finally
                    {
                        this.IsStackRestoringNow = false;
                    }
                }
#endif

            }
            else
            {
                //TODO: navigate to default page, if restoring failed.
            }
        }

        public bool CanGoBack
        {
            get
            {
                return this.frame.CanGoBack;
            }
        }

        public void GoBack()
        {
            if (this.CanGoBack)
            {
                this.frame.GoBack();
            }
        }

        public object Parameters
        {
            get
            {
                object parameters = null;
                if (this.currentEntry != null)
                {
                    parameters = this.currentEntry.Parameter;
                }

                return parameters;
            }
        }

        public object CurrentSource
        {
            get
            {
                object source = null;
                if (this.currentEntry != null)
                {
                    source = this.currentEntry.Source;
                }

                return source;
            }
        }

        public void RestoreHistory()
        {
            this.Restore();
        }

        public void SaveHistory()
        {
            this.NavigationStackSerializer.StoreHistory(this.navigationStack);
        }
    }
}
