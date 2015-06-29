namespace BaseTools.UI.Controls
{
#if SILVERLIGHT
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Threading;
    using Microsoft.Phone.Info;
#endif

#if WINRT
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.System;
#endif

    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;
    using System.Text;
    using System.Threading.Tasks;
    
    
    public class MemoryCounter : Control
    {
        private const float ByteToMega = 1024 * 1024;

        private DispatcherTimer _timer;

        private readonly MethodInfo _deviceExtendedPropertiesMethod;

        private bool _threwException;

        private bool isLaunched;

        private bool canWorkWithoutDebugger;

        private ulong maxMeasurementsMemory;

        public MemoryCounter()
        {
            DefaultStyleKey = typeof(MemoryCounter);
            DataContext = this;
#if SILVERLIGHT
            _deviceExtendedPropertiesMethod = typeof(DeviceExtendedProperties).GetMethod("GetValue");
#endif

            if (Debugger.IsAttached)
            {
                this.Launch();
            }
            else
            {
                this.Stop();
            }

            this.AllowWorkWithoutDebugging = true;
        }

        public void Launch()
        {
            this.isLaunched = true;
            _timer = new DispatcherTimer { Interval = TimeSpan.FromMilliseconds(UpdateInterval) };
            _timer.Tick += timer_Tick;
            _timer.Start();
            this.Visibility = Visibility.Visible;
        }

        public void Stop()
        {
            this.isLaunched = false;
            if (this._timer != null)
            {
                _timer.Tick -= timer_Tick;
                _timer.Stop();
                _timer = null;
            }

            this.Visibility = Visibility.Collapsed;
        }

        public bool AllowWorkWithoutDebugging
        {
            get
            {
                return this.canWorkWithoutDebugger;
            }

            set
            {
                if (this.canWorkWithoutDebugger != value)
                {
                    this.canWorkWithoutDebugger = value;
                    if (this.canWorkWithoutDebugger)
                    {
                        if (!this.isLaunched)
                        {
                            this.Launch();
                        }
                    }
                    else
                    {
                        if (!Debugger.IsAttached && this.isLaunched)
                        {
                            this.Stop();
                        }
                    }
                }
            }
        }

        public int UpdateInterval
        {
            get { return (int)GetValue(UpdateIntervalProperty); }
            set { SetValue(UpdateIntervalProperty, value); }
        }

        public static readonly DependencyProperty UpdateIntervalProperty =
            DependencyProperty.Register("UpdateInterval", typeof(int), typeof(MemoryCounter), new PropertyMetadata(1000, OnUpdateIntervalChanged));

        private static void OnUpdateIntervalChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (Debugger.IsAttached)
            {
                var sender = ((MemoryCounter)o);

                if (sender != null && sender._timer != null)
                    sender._timer.Interval = TimeSpan.FromMilliseconds((int)e.NewValue);
            }
        }

        public string CurrentMemory
        {
            get { return (string)GetValue(CurrentMemoryProperty); }
            set { SetValue(CurrentMemoryProperty, value); }
        }

        public static readonly DependencyProperty CurrentMemoryProperty =
            DependencyProperty.Register("CurrentMemory", typeof(string), typeof(MemoryCounter), new PropertyMetadata("0"));

        public string PeakMemory
        {
            get { return (string)GetValue(PeakMemoryProperty); }
            set { SetValue(PeakMemoryProperty, value); }
        }

        public static readonly DependencyProperty PeakMemoryProperty =
            DependencyProperty.Register("PeakMemory", typeof(string), typeof(MemoryCounter), new PropertyMetadata("0"));

#if SILVERLIGHT
        void timer_Tick(object sender, EventArgs e)
#endif
#if WINRT
        void timer_Tick(object sender, object e)
#endif
        {


            try
            {

#if SILVERLIGHT
                CurrentMemory =
                    ((long)
                        _deviceExtendedPropertiesMethod.Invoke(null, new[]
                                                {
                                                    "ApplicationCurrentMemoryUsage"
                                                }) / ByteToMega).ToString(
                                                    "#.00");
                PeakMemory =
                    ((long)
                        _deviceExtendedPropertiesMethod.Invoke(null,
                                    new[]
                                        {
                                            "ApplicationPeakMemoryUsage"
                                        }) /
                        ByteToMega).ToString("#.00");


#endif

#if WINRT

                var currentMemory = MemoryManager.AppMemoryUsage;
                CurrentMemory = ((long)currentMemory / ByteToMega).ToString("#.00");

                if (maxMeasurementsMemory < currentMemory)
                {
                    maxMeasurementsMemory = currentMemory;
                    PeakMemory = ((long)currentMemory / ByteToMega).ToString("#.00");
                }
#endif
            }
            catch (Exception)
            {
                _threwException = true;
                _timer.Stop();
            }
        }
    }
}
