namespace BaseTools.UI.Launchers
{
    using BaseTools.Core.Info;
    using BaseTools.Core.Ioc;
    using Microsoft.Phone.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SilverlightMarketplaceLauncher : MarketplaceLauncher
    {
        public override void ViewCurrentApplication()
        {
            var applicationLocalInfo = Factory.Common.GetInstance<ApplicationInfo>();
            this.ViewApplication(applicationLocalInfo.ProductId);
            //var applicationLocalInfo = Factory.Common.GetInstance<ApplicationInfo>();
            //MarketplaceDetailTask task = new MarketplaceDetailTask();
            //task.ContentIdentifier = applicationLocalInfo.ProductId;
            //task.Show();
        }

        public override void ViewCurrentApplicationReviews()
        {
            var applicationLocalInfo = Factory.Common.GetInstance<ApplicationInfo>();
            var task = new MarketplaceReviewTask();
            task.Show();
        }

        public override void ViewApplication(string applicationId)
        {
            var applicationLocalInfo = Factory.Common.GetInstance<ApplicationInfo>();
            MarketplaceDetailTask task = new MarketplaceDetailTask();
            task.ContentIdentifier = applicationId;
            task.Show();
        }
    }
}
