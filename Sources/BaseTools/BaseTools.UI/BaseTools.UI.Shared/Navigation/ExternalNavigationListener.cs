namespace BaseTools.UI.Navigation
{
#if SILVERLIGHT
    using System.Windows.Navigation;
    
#endif

#if WINRT
    using Windows.UI.Xaml.Navigation;
#endif

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BaseTools.Core.Utility;



    public class ExternalNavigationListener
    {
        public ExternalNavigationState State { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public static bool IsExternalUri(Uri uri)
        {
            Guard.CheckIsNotNull(uri, "uri");
            return uri.OriginalString == "app://external/";
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Can't be static"), 
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validated by Guard")]
        public void HandleNavigatingEvent(NavigatingCancelEventArgs args)
        {
            Guard.CheckIsNotNull(args, "args");
#if SILVERLIGHT
            if (this.State == ExternalNavigationState.None)
            {
                var isExternalNavigation = IsExternalUri(args.Uri);
                if (isExternalNavigation)
                {
                    this.State = ExternalNavigationState.Starting;
                }
            }
            else
            {
                ChangeStep(true);
            }
#endif
        }

        public void HandleNavigatedEvent()
        {
            ChangeStep(true);
        }

        public void HandleNavigationNotSuccess()
        {
            ChangeStep(false);
        }

        public void HandleRestoring()
        {
            this.State = ExternalNavigationState.Completed;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification="Can't be static"), 
        System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "isSuccessStep", Justification = "Used in WP")]
        private void ChangeStep(bool isSuccessStep)
        {
#if SILVERLIGHT
            if (this.State != ExternalNavigationState.None)
            {
                var intState = (int)this.State;
                if (isSuccessStep)
                {
                    intState++;
                }
                else
                {
                    intState--;
                }

                if (intState == 4)
                {
                    intState = 0;
                }

                this.State = (ExternalNavigationState)intState;
            }
#endif
        }
    }
}
