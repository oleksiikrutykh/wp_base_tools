namespace BaseTools.UI.Dialogs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Windows.UI.Popups;
    using BaseTools.Core.Utility;

    internal static class DialogService
    {
        private static bool isMessageBoxNowDisplayed;

        private static Queue<DialogQueueItem> dialogsForDisplaying = new Queue<DialogQueueItem>();

        private static readonly TimeSpan WaitPeriod = TimeSpan.FromSeconds(2);

        public static event EventHandler DialogOpening;

        public static event EventHandler DialogClosed;

        public static Task<IUICommand> DisplayAsync(MessageDialog dialog)
        {
            var dialogItem = new DialogQueueItem(dialog);
            dialogsForDisplaying.Enqueue(dialogItem);
            DisplayNextDialogInQueue();
            return dialogItem.DisplayingTask.Task;
        }

        private async static void DisplayNextDialogInQueue()
        {
            if (dialogsForDisplaying.Count > 0)
            {
                if (!isMessageBoxNowDisplayed)
                {
                    isMessageBoxNowDisplayed = true;
                    try
                    {
                        while (dialogsForDisplaying.Count > 0)
                        {
                            bool needRedisplay = false;
                            var dialogQueueItem = dialogsForDisplaying.Peek();
                            var dialog = dialogQueueItem.Dialog;
                            var displayingTask = dialogQueueItem.DisplayingTask;

                            // TODO: handle conflict with other message dialogs (f. e. system messages)
                            try
                            {
                                DialogOpening.CallEvent(dialog, EventArgs.Empty);
                                var result = await dialog.ShowAsync();
                                displayingTask.TrySetResult(result);
                                dialogsForDisplaying.Dequeue();
                            }
                            catch (UnauthorizedAccessException)
                            {
                                needRedisplay = true;
                            }
                            catch (Exception ex)
                            {
                                dialogsForDisplaying.Dequeue();
                                displayingTask.TrySetException(ex);
                                throw;
                            }
                            finally
                            {
                                DialogClosed.CallEvent(dialog, EventArgs.Empty);
                            }

                            if (needRedisplay)
                            {
                                dialogQueueItem.Dialog = CloneMessageDialog(dialogQueueItem.Dialog);
                                await Task.Delay(WaitPeriod);
                            }
                        }
                    }
                    finally
                    {
                        isMessageBoxNowDisplayed = false;
                    }

                    DisplayNextDialogInQueue();
                }
            }
        }

        private static MessageDialog CloneMessageDialog(MessageDialog messageDialog)
        {
            var copy = new MessageDialog(messageDialog.Content, messageDialog.Title);
            foreach (var command in messageDialog.Commands)
            {
                copy.Commands.Add(command);
            }

            return copy;
        }
    }
}
