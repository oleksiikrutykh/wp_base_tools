namespace IconPeak.PlatformSpecific.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Linq;
    using IconPeak.UI.Dialogs;

    internal class SilverlightMessageBoxProvider : MessageBoxProvider
    {
        protected override Task<DialogButton> PerformShow(string message, string title, IList<DialogButton> buttons)
        {
            DialogButton resultButton = null;
            var silverlightButton = MessageBoxButton.OK;
            if (buttons.Count > 1)
            {
                silverlightButton = MessageBoxButton.OKCancel;
            }

            var realResult = MessageBox.Show(message, title, silverlightButton);
            if (realResult == MessageBoxResult.OK)
            {
                resultButton = buttons.FirstOrDefault(b => !b.IsUserDismissValue);
            }
            else
            {
                resultButton = buttons.FirstOrDefault(b => b.IsUserDismissValue);
            }

            return Task.FromResult(resultButton);
        }
    }
}
