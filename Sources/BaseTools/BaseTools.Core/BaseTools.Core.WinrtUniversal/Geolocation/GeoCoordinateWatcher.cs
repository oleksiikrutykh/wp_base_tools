namespace BaseTools.Core.Geolocation
{
    using BaseTools.Core.Geolocation;
    using BaseTools.Core.Threading;
    using System;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;

    /// <summary>
    /// Provides access to device location. Can be used in WinRT apps
    /// </summary>
    internal sealed class GeoCoordinateWatcher : IGeoCoordinateWatcher, IDisposable
    {
        private GeoPosition position;

        private const double MovementThreshold = 100;

        private Geolocator coordinateWatcher;

        private readonly object startingLock = new object();

        private Task<WatcherState> runningTask;

        private TaskCompletionSource<GeoCoordinate> initializationCoordinatesTask;

        private int startsCount;

        public bool? IsAccessAllowed
        {
            get
            {
                return true;
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public GeoCoordinateWatcher()
        {
            this.coordinateWatcher = new Geolocator();
            this.coordinateWatcher.MovementThreshold = MovementThreshold;            
        }

        public GeoPosition Position
        {
            get
            {
                return this.position;
            }
        }

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
            lock (startingLock)
            {
                this.startsCount++;
                if (this.runningTask == null)
                {
                    this.runningTask = this.Initialize();
                    this.coordinateWatcher.PositionChanged += this.OnPositionChanged;
                }
            }

            return this.runningTask;
        }

        public async Task<WatcherState> Initialize()
        {
            var state = WatcherState.Active;
            try
            {
                await SynchronizationContextProvider.PostAsync(async () =>
                {
                    await this.coordinateWatcher.GetGeopositionAsync();
                });
            }
            catch (Exception)
            {
                state = WatcherState.Error;
                if (this.coordinateWatcher.LocationStatus == PositionStatus.Disabled)
                {
                    state = WatcherState.Disabled;
                }
            }

            return state;
        }     

        public void Dispose()
        {
            this.Stop();
            GC.SuppressFinalize(this);
        }

        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            var location = args.Position.Coordinate;
            this.UpdatePosition(location, args.Position.Coordinate.Timestamp);
        }

        private void UpdatePosition(Geocoordinate systemCoordinates, DateTimeOffset timestamp)
        {
            var latitude = systemCoordinates.Point.Position.Latitude;
            var longitude = systemCoordinates.Point.Position.Longitude;
            var isLatitudeCorrect = !double.IsNaN(latitude);
            var isLongitudeCorrect = !double.IsNaN(longitude);
            if (isLatitudeCorrect && isLongitudeCorrect)
            {
                var coordinates = new GeoCoordinate
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Accuracy = systemCoordinates.Accuracy,
                    Altitude = systemCoordinates.Point.Position.Altitude,
                    Course = systemCoordinates.Heading ?? 0,
                    Speed = systemCoordinates.Speed ?? 0,
                };

                this.position = new GeoPosition
                {
                    Timestamp = timestamp,
                    Location = coordinates
                };

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
                        this.coordinateWatcher.PositionChanged -= this.OnPositionChanged;
                        this.runningTask = null;
                    }
                }
            }
        }
    }
}
