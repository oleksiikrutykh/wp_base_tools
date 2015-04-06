namespace BaseTools.UI
{
    using BaseTools.Core.Ioc;
    using BaseTools.UI.Dialogs;
    using BaseTools.UI.Launchers;
    using BaseTools.UI.Navigation;
    using System;
    using System.Collections.Generic;
    using System.Text;

    public class UIFactoryInitializer : FactoryInitializer
    {
        public override void SetupBindnings(Factory initializedFactory)
        {
            initializedFactory.Bind<IApplicationNavigationProvider, ApplicationNavigationProvider>();
            initializedFactory.Bind<INavigationHistoryStorageProvider, NavigationHistoryStorageProvider>();
#if SILVERLIGHT
            initializedFactory.Bind<EmailLauncher, SilverlightEmailLauncher>();
            initializedFactory.Bind<MarketplaceLauncher, SilverlightMarketplaceLauncher>();
            initializedFactory.Bind<MessageBoxProvider, SilverlightMessageBoxProvider>();
#endif

#if WINRT
            initializedFactory.Bind<MessageBoxProvider, WinrtMessageBoxProvider>();
#endif
        }
    }
}
