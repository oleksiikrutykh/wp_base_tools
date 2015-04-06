namespace BaseTools.Core.Diagnostics
{
    using BaseTools.Core.FileSystem;
    using BaseTools.Core.Ioc;
    using BaseTools.Core.Security;
    using BaseTools.Core.Storage;
    using BaseTools.Core.Threading;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Reflection;
    using BaseTools.Core.DataAccess;
    using System.Globalization;
    using BaseTools.Core.Info;

    /// <summary>
    /// Send logging info to Google analytics service.
    /// </summary>
    public class GoogleAnalyticsLogger : StorageLogger
    {
        public string ApplicationKey { get; set; }

        protected override async Task TrySendSingleMessage(LoggingMessage message, int errorsCount)
        {
            var sendedTasks = new Task<DataResponse<bool>>[errorsCount];
            for (int i = 0; i < errorsCount; i++)
            {
                var responseTask = this.SendMessageInstance(message);
                sendedTasks[i] = responseTask;
            }

            await Task.WhenAll(sendedTasks);
        }

        private async Task<DataResponse<bool>> SendMessageInstance(LoggingMessage message)
        {
            //TODO: check StoringPeriodLimit api.
            var request = new HttpRequest<bool>();
            request.Parser = new SuccessParser<bool>();
            request.FullPath = "http://www.google-analytics.com/collect";
            request.Method = Network.HttpMethod.Post;
            request.Headers["Content-Type"] = "text/plain; charset=utf-8";
            request.AddParameter("v", 1);
            request.AddParameter("tid", this.ApplicationKey);
            request.AddParameter("cid", MD5Calculator.GetMD5Hash(EnvironmentInfo.Current.DeviceUniqueId));
            request.AddParameter("an", "IconPeak");
            request.AddParameter("av", message.ApplicationVersion);
            request.AddParameter("t", "exception");
            request.AddParameter("exd", message.Message);
            request.AddParameter("exf", 0);
            request.AddParameter("ul", CultureInfo.CurrentUICulture.Name);
            var response = await request.SendAsync();
            if (response.IsSuccess)
            {
                await this.ReportMessageSended(message, 1);
            }

            return response;
        }

    }
}
