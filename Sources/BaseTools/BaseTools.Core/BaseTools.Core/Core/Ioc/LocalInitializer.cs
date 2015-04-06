namespace BaseTools.Core.Ioc
{
    using BaseTools.Core.Common;
    using BaseTools.Core.Diagnostics;
    using BaseTools.Core.Network;
    using BaseTools.Core.Utility;
    using BaseTools.UI.Dialogs;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    internal class LocalInitializer : FactoryInitializer
    {
        public override void SetupBindnings(Factory initializedFactory)
        {
            Guard.CheckIsNotNull(initializedFactory, "initializedFactory");
            initializedFactory.Bind<HttpRequestSender, WebRequestSender>();
            initializedFactory.Bind<MessageBoxProvider, MessageBoxProvider>();
            initializedFactory.Bind<RandomProvider, RandomProvider>();
            initializedFactory.Bind<AnalyticsProvider, AnalyticsProvider>();
        }
    }
}
