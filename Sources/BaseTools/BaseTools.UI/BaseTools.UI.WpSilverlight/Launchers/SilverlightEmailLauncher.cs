namespace BaseTools.UI.Launchers
{
    using BaseTools.Core.Utility;
    using Microsoft.Phone.Tasks;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class SilverlightEmailLauncher : EmailLauncher
    {
        private const int MaxEmailMessageLength = 30000;


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification="Validate by Guard")]
        public override Task CreateEmail(EmailData data)
        {
            Guard.CheckIsNotNull(data, "data");
            int limit = MaxEmailMessageLength;

            EmailComposeTask emailComposeTask = new EmailComposeTask();
            emailComposeTask.Subject = Trim(data.Subject, limit);
            limit -= emailComposeTask.Subject.Length;

            emailComposeTask.To = Trim(data.Receiver, limit);
            limit -= emailComposeTask.To.Length;

            emailComposeTask.Body = Trim(data.Body, limit);
            limit -= emailComposeTask.Body.Length;

            emailComposeTask.Show();
            return Task.FromResult(true);
        }

        private static string Trim(string data, int limit)
        {
            if (data == null)
            {
                data = string.Empty;
            }

            if (data.Length > limit)
            {
                data = data.Substring(0, limit);
            }

            return data;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1062:Validate arguments of public methods", MessageId = "0", Justification = "Validate by Guard")]
        public override int DetermineRemainingLength(EmailData data)
        {
            Guard.CheckIsNotNull(data, "data");
            var length = SafeLength(data.Body) + SafeLength(data.Receiver) + SafeLength(data.Subject);
            return MaxEmailMessageLength - length;
        }

        private static int SafeLength(string input)
        {
            int result = 0;
            if (input != null)
            {
                result = input.Length;
            }

            return result;
        }
    }
}
