using BaseTools.UI.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BaseTools.UIExternal.WpSilverlight.Dialogs
{
    internal class MultiButtonsMessageBoxProvider : SilverlightMessageBoxProvider
    {
        protected override Task<DialogButton> PerformShow(string message, string title, IList<DialogButton> buttons)
        {
            DialogButton result = null;
            if (buttons.Count <= 2)
            {
                bool isStandard = false;
                if (buttons.Count == 1)
                {
                    var firstButton = buttons[0];
                    isStandard = String.Equals(firstButton.Name, "Cancel", StringComparison.InvariantCultureIgnoreCase);
                }
                else
                {
                    var firstButton = buttons[0];
                    var secondButton = buttons[1];
                    isStandard = String.Equals(firstButton.Name, "Ok", StringComparison.InvariantCultureIgnoreCase) &&
                                 String.Equals(secondButton.Name, "Cancel", StringComparison.InvariantCultureIgnoreCase);
                }

                if (isStandard)
                {
                    result = this.DisplayStandart(message, title, buttons);
                }
                else
                {
 
                }
            }
            else
            {
                //TODO: display 3 buttons control.
            }
        }

        private DialogButton DisplayStandart(string message, string title, IList<DialogButton> buttons)
        {
            return base.PerformShow(message, title, buttons);
        }

        private static bool DisplayCustomMessageBox(string originName, string expectedName)
        {
            
        }
    }
}
