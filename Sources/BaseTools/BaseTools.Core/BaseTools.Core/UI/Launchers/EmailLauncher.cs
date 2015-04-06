namespace BaseTools.UI.Launchers
{
    using BaseTools.Core.Ioc;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public abstract class EmailLauncher
    {
        private static readonly EmailLauncher instance = Factory.Common.GetInstance<EmailLauncher>();

        public static EmailLauncher Instance
        {
            get
            {
                return instance;
            }
        }

        public abstract Task CreateEmail(EmailData data);

        public abstract int DetermineRemainingLength(EmailData data);
    }
}
