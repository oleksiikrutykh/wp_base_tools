namespace BaseTools.UI.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Popups;

    internal class WinrtMessageBoxProvider : MessageBoxProvider
    {
        private const int MaxButtonsCount = 3;

        protected override async Task<DialogButton> PerformShow(string message, string title, IList<DialogButton> buttons)
        {
            var dialog = new MessageDialog(message, title);
            
            foreach (var button in buttons)
            {
                var command = new UICommand(button.Name); 
                dialog.Commands.Add(command);
                if (dialog.Commands.Count == MaxButtonsCount)
                {
                    break;
                }
            }

            uint defaultIndex = 0;
            foreach (var button in buttons)
            {
                if (button.IsDefault)
                {
                    break;
                }
                else
                {
                    defaultIndex++;
                }
            }

            dialog.DefaultCommandIndex = defaultIndex;
            var dialogResult = await DialogService.DisplayAsync(dialog);
            if (dialogResult == null)
            {
                
            }

            var selectedCommandIndex = dialog.Commands.IndexOf(dialogResult);
            DialogButton selectedButton = null;
            if (selectedCommandIndex >= 0)
            {
                selectedButton = buttons[selectedCommandIndex];
            }
            else
            {
                selectedButton = buttons.FirstOrDefault(b => b.IsUserDismissValue);
            }

            return selectedButton;
        }
    }
}
