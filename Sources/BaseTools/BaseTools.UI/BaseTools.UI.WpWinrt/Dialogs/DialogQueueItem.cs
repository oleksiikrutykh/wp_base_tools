namespace BaseTools.UI.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Popups;

    internal class DialogQueueItem
    {
        public DialogQueueItem(MessageDialog dialog)
        {
            this.Dialog = dialog;
            this.DisplayingTask = new TaskCompletionSource<IUICommand>();
        }

        public MessageDialog Dialog { get; set; }

        public TaskCompletionSource<IUICommand> DisplayingTask { get; private set; }
    }
}
