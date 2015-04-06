namespace BaseTools.UI.Launchers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class MarketplaceLauncher
    {
        public abstract void ViewApplication(string applicationId);

        public abstract void ViewCurrentApplication();

        public abstract void ViewCurrentApplicationReviews();
    }
}
