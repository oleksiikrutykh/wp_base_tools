// Copyright (C) Microsoft Corporation. All Rights Reserved.
// This code released under the terms of the Microsoft Public License
// (Ms-PL, http://opensource.org/licenses/ms-pl.html).



namespace BaseTools.UI.ControlExtensions
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics.CodeAnalysis;
    using System.IO;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media.Imaging;
    using System.Windows.Media;
    using System.IO.IsolatedStorage;
    using System.Diagnostics;
    using System.Windows.Shapes;
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using System.Threading.Tasks;

    /// <summary>
    /// Provides access to the Image.UriSource attached property which allows
    /// Images to be loaded by Windows Phone with less impact to the UI thread.
    /// </summary>
    public static class ImageLoader
    {
        private static Dictionary<DependencyObject, PendingRequest> notSendedRequestsDictionary = new Dictionary<DependencyObject, PendingRequest>();

        private static int activeRequestCount = 0;

        private const int WorkItemQuantum = 5;
        private static readonly Thread _thread = new Thread(WorkerThreadProc);
        private static Stack<PendingRequest> _pendingRequests = new Stack<PendingRequest>();
        private static readonly Queue<IAsyncResult> _pendingResponses = new Queue<IAsyncResult>();
        private static readonly object _syncBlock = new object();
        private static bool _exiting;

        private static readonly IFileSystemProvider fileSystemProvider;

        private static Stream emptyStream;

        private static Uri imageStubPath = new Uri("/BaseTools.UI.WindowsPhone;component/Assets/loading.png", UriKind.Relative);

        public static Uri ImageStubPath 
        {
            get
            {
                return imageStubPath;
            }

            set
            {
                if (value != imageStubPath && value != null)
                {
                    imageStubPath = value;
                    LoadingImageSource.SetSource(Application.GetResourceStream(imageStubPath).Stream);
                }
            }
        }

    	private static BitmapImage imageStub;
    	private static BitmapImage LoadingImageSource
		{
			get
			{
				if (imageStub == null)
				{
					imageStub = new BitmapImage();

                    var path = Application.GetResourceStream(imageStubPath);
                    if (path != null)
                    {
                        imageStub.SetSource(path.Stream);
                    }
				}

				return imageStub;
			}
		}

        /// <summary>
        /// Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static string GetSource(DependencyObject obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (string)obj.GetValue(SourceProperty);
        }

        /// <summary>
        /// Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static void SetSource(DependencyObject obj, string value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            obj.SetValue(SourceProperty, value);
        }

        /// <summary>
        /// Identifies the UriSource attached DependencyProperty.
        /// </summary>
        public static readonly DependencyProperty SourceProperty = DependencyProperty.RegisterAttached(
            "Source", typeof(string), typeof(ImageLoader), new PropertyMetadata(OnUriSourceChanged));

        public static readonly DependencyProperty ImageSettingsProperty = DependencyProperty.RegisterAttached(
            "ImageSettings", typeof(ImageSettings), typeof(ImageLoader), new PropertyMetadata(null));

        /// <summary>
        /// Gets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <returns>Uri to use for providing the contents of the Source property.</returns>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static ImageSettings GetImageSettings(DependencyObject obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            return (ImageSettings)obj.GetValue(ImageSettingsProperty);
        }

        /// <summary>
        /// Sets the value of the Uri to use for providing the contents of the Image's Source property.
        /// </summary>
        /// <param name="obj">Image needing its Source property set.</param>
        /// <param name="value">Uri to use for providing the contents of the Source property.</param>
        [SuppressMessage("Microsoft.Design", "CA1011:ConsiderPassingBaseTypesAsParameters", Justification = "UriSource is applicable only to Image elements.")]
        public static void SetImageSettings(DependencyObject obj, ImageSettings value)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }

            obj.SetValue(ImageSettingsProperty, value);
        }


        /// <summary>
        /// Gets or sets a value indicating whether low-profile image loading is enabled.
        /// </summary>
        public static bool IsEnabled { get; set; }

        [SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Static constructor performs additional tasks.")]
        static ImageLoader()
        {
            // Start worker thread
            _thread.Start();
            CreateEmptyStream();
            Application.Current.Exit += new EventHandler(HandleApplicationExit);
            IsEnabled = true;
            if (!DesignerProperties.IsInDesignTool)
            {
                fileSystemProvider = Factory.Common.GetInstance<IFileSystemProvider>();
                //ControlLifetimeListener.Instance.ControlDeactivate += OnControlRemoved;
                //ControlLifetimeListener.Instance.ControlRestored += OnControlRestored;
            }
        }

        //private static void OnControlRestored(object sender, ControlRestoredEventArgs e)
        //{
        //    var image = (Image)e.Element;
        //    StartLoading(image);
        //    // Write restore logic.
        //    //OnUriSourceChanged(image, )
        //    //ClearPreviousImageSource(image.Source);
        //}

        //private static void OnControlRemoved(object sender, ControlRemovedEventArgs e)
        //{
        //    var image = (Image)e.Element;
        //    ClearPreviousImageSource(image.Source);
        //}

        //private static bool IsControlActive(DependencyObject loadTarget)
        //{
        //    var imageCotnrol = loadTarget as Image;
        //    bool isControlActive = true;
        //    if (imageCotnrol != null)
        //    {
        //        var state = ControlLifetimeListener.Instance.DetermineControlState(imageCotnrol);
        //        if (state == ControlLifetimeState.Unactive)
        //        {
        //            isControlActive = false;
        //        }
        //    }

        //    return isControlActive;
        //}

        private static void HandleApplicationExit(object sender, EventArgs e)
        {
            // Tell worker thread to exit
            _exiting = true;
            if (Monitor.TryEnter(_syncBlock, 100))
            {
                Monitor.Pulse(_syncBlock);
                Monitor.Exit(_syncBlock);
            }
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Relevant exceptions don't have a common base class.")]
        private static void WorkerThreadProc(object unused)
        {
            Queue<PendingRequest> pendingRequests = new Queue<PendingRequest>();
            Queue<IAsyncResult> pendingResponses = new Queue<IAsyncResult>();
            Queue<PendingCompletion> pendingCompletions = new Queue<PendingCompletion>();
            while (!_exiting)
            {
                lock (_syncBlock)
                {
                    // Wait for more work if there's nothing left to do
                    bool canEnqueuePendingRequest = (_pendingRequests.Count == 0) || (activeRequestCount == WorkItemQuantum); 
                    if ((canEnqueuePendingRequest) && (0 == _pendingResponses.Count) && (0 == pendingRequests.Count) && (0 == pendingResponses.Count))
                    {
                        Monitor.Wait(_syncBlock);
                        if (_exiting)
                        {
                            return;
                        }
                    }

                    var allowedCount = WorkItemQuantum - activeRequestCount;
                    while (_pendingRequests.Count > 0 && allowedCount > 0)
                    {
                        var request = _pendingRequests.Pop();
                        //var isControlActive = IsControlActive(request.Image);
                        //if (isControlActive)
                        //{
                            //var request = _pendingRequests.Dequeue();
                            if (request.Uri != null)
                            {
                                notSendedRequestsDictionary.Remove(request.Image);
                                pendingRequests.Enqueue(request);
                                allowedCount--;
                            }
                        //}
                    }

                    while (0 < _pendingResponses.Count)
                    {
                        pendingResponses.Enqueue(_pendingResponses.Dequeue());
                    }
                }

                while (pendingRequests.Count > 0)
                {
                    var pendingRequest = pendingRequests.Dequeue();
                    LoadImage(pendingRequest, pendingCompletions);
                    // Yield to UI thread
                    Thread.Sleep(1);
                }
                // Process pending responses
                for (var i = 0; (i < pendingResponses.Count) && (i < WorkItemQuantum); i++)
                {
                    var pendingResponse = pendingResponses.Dequeue();
                    var responseState = (ResponseState)pendingResponse.AsyncState;
                    try
                    {
                        var response = responseState.WebRequest.EndGetResponse(pendingResponse);
                        pendingCompletions.Enqueue(new PendingCompletion(responseState.Image, responseState.Uri, response.GetResponseStream()));
                    }
                    catch (WebException)
                    {
                        // Ignore web exceptions (ex: not found)
                    }
                    // Yield to UI thread
                    Thread.Sleep(1);
                }
                // Process pending completions
                if (0 < pendingCompletions.Count)
                {
                    // Get the Dispatcher and process everything that needs to happen on the UI thread in one batch
                    Deployment.Current.Dispatcher.BeginInvoke(() =>
                    {
                        while (0 < pendingCompletions.Count)
                        {
                            // Decode the image and set the source
                            var pendingCompletion = pendingCompletions.Dequeue();

                            //Somehow we get null reference exception from here. 
                            if (pendingCompletion != null)
                            {
                                if (GetSource(pendingCompletion.Image) == pendingCompletion.Uri)
                                {
                                    //var isControlActive = IsControlActive(pendingCompletion.Image);
                                    //if (isControlActive)
                                    //{
                                        BitmapSource bitmap = null;
                                        try
                                        {
                                            var imageSettings = GetImageSettings(pendingCompletion.Image);
                                            if (imageSettings != null)
                                            {
                                                bitmap = CompressImage(pendingCompletion.Stream, imageSettings);
                                            }
                                            else
                                            {
                                                bitmap = new BitmapImage();
                                                bitmap.SetSource(pendingCompletion.Stream);
                                            }
                                        }
                                        catch
                                        {
                                            // Ignore image decode exceptions (ex: invalid image)
                                        }

                                        SetContentForImage(pendingCompletion.Image, bitmap);
                                    //}
                                }
                                else
                                {
                                    // Uri mis-match; do nothing
                                }

                                // Dispose of response stream if stream exist
                                if (pendingCompletion.Stream != null)
                                {
                                    pendingCompletion.Stream.Dispose();
                                }
                            }
                        }
                    });
                }
            }
        }

        private static BitmapSource CompressImage(Stream stream, ImageSettings settings)
        {
            BitmapImage sourceBitmapImage = new BitmapImage();
            BitmapSource source = sourceBitmapImage;
            sourceBitmapImage.CreateOptions = BitmapCreateOptions.None;
            sourceBitmapImage.SetSource(stream);

            bool isBigPhoto = sourceBitmapImage.PixelHeight > 300 || sourceBitmapImage.PixelWidth > 300;
            if (isBigPhoto && sourceBitmapImage.PixelHeight > settings.Height &&
                sourceBitmapImage.PixelWidth > settings.Width)
            {
                var scaleHeight = settings.Height / sourceBitmapImage.PixelHeight;
                var scaleWidth = settings.Width / sourceBitmapImage.PixelWidth;
                var scale = Math.Max(scaleHeight, scaleWidth);

                Image rawImage = new Image();
                rawImage.Source = sourceBitmapImage;

                ScaleTransform transform = new ScaleTransform();
                transform.ScaleX = scale; 
                transform.ScaleY = scale;

                WriteableBitmap WritebleBitmapToTransform = new WriteableBitmap((int)settings.Width, (int)settings.Height);
                WritebleBitmapToTransform.Render(rawImage, transform);
                WritebleBitmapToTransform.Invalidate();
                using (var compressednStream = new MemoryStream())
                {
                    WritebleBitmapToTransform.SaveJpeg(compressednStream, WritebleBitmapToTransform.PixelWidth, WritebleBitmapToTransform.PixelHeight, 0, 100);
                    rawImage.Source = null;
                    ClearPreviousImageSource(sourceBitmapImage);
                    var compressedSource = new BitmapImage();
                    compressedSource.SetSource(compressednStream);
                    source = compressedSource;
                }
            }

            return source;
        }

        private static void LoadImage(PendingRequest pendingRequest, Queue<PendingCompletion> pendingCompletions)
        {
            var uri = new Uri(pendingRequest.Uri, UriKind.RelativeOrAbsolute);
            if (uri.IsAbsoluteUri)
            {
                switch (uri.Scheme)
                {
                    case "ms-appdata":
                        // File placed in isolated storage. Build valid path here.
                        var storagePath = pendingRequest.Uri.Replace("ms-appdata:///", String.Empty);
                        bool isLocalPath = storagePath.StartsWith("Local/", StringComparison.OrdinalIgnoreCase);
                        if (isLocalPath)
                        {
                            // remove "Local" prefix.
                            var slashIndex = storagePath.IndexOf("/", StringComparison.Ordinal);
                            storagePath = storagePath.Substring(slashIndex);
                            ReadImageFromStorage(storagePath, pendingRequest, pendingCompletions);
                        }

                        break;

                    case "ms-appx":
                        // File placed in local resources.
                        var path = pendingRequest.Uri.Replace("ms-appx:///", String.Empty);
                        ReadImageFromPackage(path, pendingRequest, pendingCompletions);
                        break;

                    default:
                        // Download from network
                        SendWebRequest(pendingRequest);
                        break;

                }
            }
            else
            {
                // Load from application (must have "Build Action"="Content")
                var originalUriString = pendingRequest.Uri;
                ReadImageFromPackage(originalUriString, pendingRequest, pendingCompletions);
                
            }
        }

        private static void SendWebRequest(PendingRequest pendingRequest)
        {
            var webRequest = HttpWebRequest.CreateHttp(pendingRequest.Uri);
            webRequest.AllowReadStreamBuffering = true; // Don't want to block this thread or the UI thread on network access
            webRequest.BeginGetResponse(HandleGetResponseResult, new ResponseState(webRequest, pendingRequest.Image, pendingRequest.Uri));
            Interlocked.Increment(ref activeRequestCount);
        }

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Relevant exceptions don't have a common base class.")]
        private static void ReadImageFromPackage(string path, PendingRequest pendingRequest, Queue<PendingCompletion> pendingCompletions)
        {
            // Trim leading '/' to avoid problems
            bool hasSlashInStart = path.StartsWith("/", StringComparison.Ordinal);
            if (hasSlashInStart)
            {
                path = path.TrimStart('/');
            }

          
            try
            {
                var resourceStreamUri = new Uri(path, UriKind.Relative);
                // Enqueue resource stream for completion
                var streamResourceInfo = Application.GetResourceStream(resourceStreamUri);
                if (null != streamResourceInfo && null != streamResourceInfo.Stream)
                {
                    pendingCompletions.Enqueue(new PendingCompletion(pendingRequest.Image, pendingRequest.Uri, streamResourceInfo.Stream));
                }
            }
            catch 
            {
                // ignore wrong uri or IO exeptions
            }
        }

        private static async void ReadImageFromStorage(string path, PendingRequest pendingRequest, Queue<PendingCompletion> pendingCompletions)
        {
            using (var fileStream = await fileSystemProvider.OpenFileAsync(path, BaseTools.Core.FileSystem.FileOpeningMode.OpenOrCreate))
            {
                if (fileStream != null)
                {
                    var memoryStream = new MemoryStream();
                    fileStream.CopyTo(memoryStream);
                    pendingCompletions.Enqueue(new PendingCompletion(pendingRequest.Image, pendingRequest.Uri, memoryStream));
                    lock (_syncBlock)
                    {
                        Monitor.Pulse(_syncBlock);
                    }
                }
            }
        }

        private static void SetContentForImage(DependencyObject imageFrameworkElement, BitmapSource source)
        {
            var image = imageFrameworkElement as Image;
            if (image != null)
            {
                var previousSource = image.Source;
                image.Source = source;
                ClearPreviousImageSource(previousSource);
            }
            else
            {
                var imageBrush = imageFrameworkElement as ImageBrush;
                if (imageBrush != null)
                {
                    var previousSource = imageBrush.ImageSource;
                    imageBrush.ImageSource = source;
                    ClearPreviousImageSource(previousSource);
                }
                else
                {
                    // What objects can include a ImageSorce or somting like that?
                }
            }
        }

        private static void ClearPreviousImageSource(ImageSource previousImageSource)
        {
            var bitmapImage = previousImageSource as BitmapImage;
            if (bitmapImage != null)
            {
                var isCreatedWithUriSource = bitmapImage.UriSource != null &&
                                             bitmapImage.UriSource.OriginalString != String.Empty;

                var isLoadingThumb = bitmapImage == LoadingImageSource;
                if (!isCreatedWithUriSource && !isLoadingThumb)
                {
                    bitmapImage.SetSource(emptyStream);
                }
            }
        }


        public static void CreateEmptyStream()
        {
            var memoryStream = new MemoryStream();
            var writableBitmap = new WriteableBitmap(1, 1);
            writableBitmap.SaveJpeg(memoryStream, 1, 1, 0, 100);
            emptyStream = memoryStream;
        }

        private static void OnUriSourceChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            var imageControl = o;
            var uri = (string)e.NewValue;
            var image = imageControl as Image;
            //if (image != null)
            //{
            //    ControlLifetimeListener.Instance.TrackControl(image);
            //}

            StartLoading(imageControl, uri);
        }

        private static void StartLoading(DependencyObject loadTarget)
        {
            var uri = GetSource(loadTarget);
            StartLoading(loadTarget, uri);
        }

        private static async void StartLoading(DependencyObject loadTarget, string uri)
        {
            var imageControl = loadTarget;
            if (uri == null)
            {
                SetContentForImage(imageControl, null);
                return;
            }

            //IsEnabled = false;
            if (!IsEnabled || DesignerProperties.IsInDesignTool)
            {
                // Avoid handing off to the worker thread (can cause problems for design tools)
                SetContentForImage(imageControl, new BitmapImage(new Uri(uri, UriKind.RelativeOrAbsolute)));
            }
            else
            {
                SetContentForImage(imageControl, LoadingImageSource);
                await Task.Delay(500);
                EnqueueRequest(imageControl, uri);
            }
        }

        public static void EnqueueRequest(DependencyObject image, string imageUri)
        {
            lock (_syncBlock)
            {
                PendingRequest request = null;
                if (notSendedRequestsDictionary.TryGetValue(image, out request))
                {
                    request.Uri = null;
                }

                request = new PendingRequest(image, imageUri);
                notSendedRequestsDictionary[image] = request;
                _pendingRequests.Push(request);
                //_pendingRequests.Enqueue(request);
                Monitor.Pulse(_syncBlock);
            }
        }

        private static void HandleGetResponseResult(IAsyncResult result)
        {
            lock (_syncBlock)
            {
                Interlocked.Decrement(ref activeRequestCount);
                // Enqueue the response
                _pendingResponses.Enqueue(result);
                Monitor.Pulse(_syncBlock);
            }
        }

        private class PendingRequest
        {
            public DependencyObject Image { get; private set; }
            public string Uri { get; set; }
            public PendingRequest(DependencyObject image, string uri)
            {
                Image = image;
                Uri = uri;
            }
        }

        private class ResponseState
        {
            public WebRequest WebRequest { get; private set; }
            public DependencyObject Image { get; private set; }
            public string Uri { get; private set; }
            public ResponseState(WebRequest webRequest, DependencyObject image, string uri)
            {
                WebRequest = webRequest;
                Image = image;
                Uri = uri;
            }
        }

        private class PendingCompletion
        {
            public DependencyObject Image { get; private set; }
            public string Uri { get; private set; }
            public Stream Stream { get; private set; }
            public PendingCompletion(DependencyObject image, string uri, Stream stream)
            {
                Image = image;
                Uri = uri;
                Stream = stream;
            }
        }
    }
}
