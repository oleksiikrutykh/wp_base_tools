namespace BaseTools.Core.Geolocation
{
    using BaseTools.Core.Geolocation;
    using System;
    using System.Threading.Tasks;
    using WP = System.Device.Location;

    /// <summary>
    /// A provider class exposing the location service.
    /// </summary>
    internal sealed class GeoCoordinateWatcher : IGeoCoordinateWatcher, IDisposable
    {
        private GeoPosition position;

        private const double MovementThreshold = 100;

        private WP.GeoCoordinateWatcher coordinateWatcher;

        private readonly object startingLock = new object();

        //private IStorageProvider storageProvider = Factory.CommonFactory.GetInstance<IStorageProvider>();

        //private IMessageBoxProvider messageBoxProvider = Factory.CommonFactory.GetInstance<IMessageBoxProvider>();

        private const string LocationAllowedStorageKey = "IsAccessToLocationAllowed";

        //private WatcherState watcherState = WatcherState.Initialized;

        private Task<WatcherState> runningTask;

        private TaskCompletionSource<GeoCoordinate> initializationCoordinatesTask;

        private int startsCount;

        /// <summary>
        /// Initializes a new instance of the <see cref="GeoCoordinateWatcher" /> class.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public GeoCoordinateWatcher()
        {
            
        }

        public static string LocationAccessMessage { get; set; }

        public static string LocationAccessTitle { get; set; }

        /// <summary>
        /// The most recent position obtained from the location service.
        /// </summary>
        /// <value>The position.</value>
        public GeoPosition Position
        {
            get 
            {
                return this.position;
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule"), System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2140:TransparentMethodsMustNotReferenceCriticalCodeFxCopRule")]
        public bool? IsAccessAllowed
        {
            get
            {
                return true;
            }

            set
            {
            }

            //get
            //{
            //    return this.storageProvider.ReadFromSettings<bool?>(LocationAllowedStorageKey);
            //}

            //set
            //{
            //    this.storageProvider.WriteToSettings<bool?>(LocationAllowedStorageKey, value);
            //}
        }

        /// <summary>
        /// Occurs when the location service detects a change in position.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1009:DeclareEventHandlersCorrectly", Justification = "This is normal declaration of EventHandler.")]
        public event EventHandler<GeoPositionChangedEventArgs> PositionChanged;

        public Task<GeoCoordinate> TakeGeoCoordinateAsync()
        {
            var taskSource = new TaskCompletionSource<GeoCoordinate>();
            if (this.position == null)
            {
                initializationCoordinatesTask = new TaskCompletionSource<GeoCoordinate>();
                taskSource = initializationCoordinatesTask;
            }
            else
            {
                taskSource.TrySetResult(this.position.Location);
            }

            return taskSource.Task;
        }

        public Task<WatcherState> StartAsync()
        {
            Task<WatcherState> initializationTask = null;
            lock (startingLock)
            {
                this.startsCount++;
                if (this.runningTask == null)
                {
                    this.runningTask = this.Initialize();
                }

                initializationTask = this.runningTask;
            }

            return initializationTask;
        }

        public async Task<WatcherState> Initialize()
        {
            
            bool? isLocationAllowed = true;
            // Hide question about access to location. 
            //var isLocationAllowed = this.storageProvider.ReadFromSettings<bool?>(LocationAllowedStorageKey);
            //if (!isLocationAllowed.HasValue)
            //{
            //    var message = LocationAccessMessage;
            //    if (String.IsNullOrEmpty(message))
            //    {
            //        message = CommonResources.LocationAccessMessage;
            //    }

            //    var title = LocationAccessTitle;
            //    if (String.IsNullOrEmpty(title))
            //    {
            //        title = CommonResources.LocationAccessTitle;
            //    }

            //    var messageBoxResult = await messageBoxProvider.ShowAsync(message, title, MessageBoxProviderButton.OkCancel);
            //    isLocationAllowed = messageBoxResult == MessageBoxProviderResult.Ok;
            //    this.storageProvider.WriteToSettings<bool?>(LocationAllowedStorageKey, isLocationAllowed);
            //}

            WatcherState result = WatcherState.Disabled;
            if (isLocationAllowed.Value)
            {
                this.coordinateWatcher = new WP.GeoCoordinateWatcher();
                this.coordinateWatcher.MovementThreshold = MovementThreshold;
                this.coordinateWatcher.PositionChanged += this.OnPositionChanged;
                // Run initialization in background thread.
                result = await Task.Run(() =>
                {
                    var isSuccessInitialize = this.coordinateWatcher.TryStart(true, TimeSpan.FromSeconds(6));
                    var state = WatcherState.Active;
                    if (!isSuccessInitialize)
                    {
                        state = WatcherState.Error;
                    }

                    if (this.coordinateWatcher.Permission == WP.GeoPositionPermission.Denied)
                    {
                        state = WatcherState.Disabled;
                    }

                    return state;
                });
            }

            //this.watcherState = result;
            return result;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>        

        public void Dispose()
        {
            this.Stop();
            this.coordinateWatcher.Dispose();
        }

        private void OnPositionChanged(object sender, WP.GeoPositionChangedEventArgs<WP.GeoCoordinate> args)
        {
            var location = args.Position.Location;
            this.UpdatePosition(location, args.Position.Timestamp);
        }

        private void UpdatePosition(WP.GeoCoordinate systemCoordinates, DateTimeOffset timestamp)
        {
            double latitude = systemCoordinates.Latitude;
            double longitude = systemCoordinates.Longitude;
            var isLatitudeCorrect = !double.IsNaN(latitude);
            var isLongitudeCorrect = !double.IsNaN(longitude);
            if (isLatitudeCorrect && isLongitudeCorrect)
            {
                var coordinates = new GeoCoordinate
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = systemCoordinates.Altitude,
                    Accuracy = systemCoordinates.HorizontalAccuracy,
                    Speed = systemCoordinates.Speed,
                    Course = systemCoordinates.Course
                };

                this.position = new GeoPosition
                {
                    Timestamp = timestamp,
                    Location = coordinates
                };

                // Notify all waiting thread what
                if (initializationCoordinatesTask != null)
                {
                    initializationCoordinatesTask.TrySetResult(this.position.Location);
                }

                var handler = this.PositionChanged;
                if (handler != null)
                {
                    handler(this, new GeoPositionChangedEventArgs() { Position = this.position });
                }
            }
        }

        public async void Stop()
        {
            var initializationTask = this.runningTask;
            if (initializationTask != null)
            {
                await initializationTask;
            }

            lock (startingLock)
            {
                if (this.startsCount > 0)
                {
                    this.startsCount--;
                    if (this.startsCount == 0)
                    {
                        if (this.coordinateWatcher != null)
                        {
                            this.coordinateWatcher.Stop();
                            this.coordinateWatcher.PositionChanged -= this.OnPositionChanged;
                            this.coordinateWatcher.Dispose();
                            this.coordinateWatcher = null;
                        }

                        this.runningTask = null;
                    }
                }
            }
        }
    }
}
