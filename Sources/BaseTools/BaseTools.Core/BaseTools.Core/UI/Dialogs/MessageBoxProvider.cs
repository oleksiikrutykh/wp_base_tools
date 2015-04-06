namespace BaseTools.UI.Dialogs
{
    using BaseTools.Core.Ioc;
    using BaseTools.Resources;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    public class MessageBoxProvider
    {
        private const int CancelId = 1;

        private const int NoId = 2;

        private const int OkId = 3;

        private const int YesId = 4;

        protected static DialogButton OkButton = new DialogButton { IsDefault = true };
        
        protected static readonly IList<DialogButton> OkButtonCollection = new List<DialogButton> { new DialogButton { Id = OkId, Name = Messages.MessageBoxOkText, IsDefault = true } };

        protected static readonly IList<DialogButton> OkCancelButtonCollection = new List<DialogButton> 
        {
            new DialogButton { Id = OkId, Name = Messages.MessageBoxOkText, IsDefault = true, },
            new DialogButton { Id = CancelId, Name = Messages.MessageBoxCancelText, IsUserDismissValue = true, }
        };

        protected static readonly IList<DialogButton> YesNoButtonCollection = new List<DialogButton> 
        { 
            new DialogButton { Id = YesId, Name = Messages.MessageBoxYesText, IsDefault = true },
            new DialogButton { Id = NoId, Name = Messages.MessageBoxNoText, IsUserDismissValue = true, }
        };

        private static MessageBoxProvider instance;

        public static MessageBoxProvider Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = Factory.Common.GetInstance<MessageBoxProvider>();
                }

                return instance;
            }
        }

        public bool IsSupportCustomMessageBoxes { get; protected set; }

        public Task<MessageBoxProviderResult> ShowAsync(string message)
        {
            return this.ShowAsync(message, string.Empty, MessageBoxProviderButton.Ok);
        }

        public Task<MessageBoxProviderResult> ShowAsync(string message, string title)
        {
            return this.ShowAsync(message, title, MessageBoxProviderButton.Ok);
        }

        public async Task<MessageBoxProviderResult> ShowAsync(string message, string title, MessageBoxProviderButton button)
        {
            var customButtons = OkButtonCollection;
            if (button == MessageBoxProviderButton.OkCancel)
            {
                customButtons = OkCancelButtonCollection;
            }
            else if (button == MessageBoxProviderButton.YesNo)
            {
                customButtons = YesNoButtonCollection;
            }

            // TODO: Update button names with current resources values.
            foreach (var updatedButton in customButtons)
            {
                switch (updatedButton.Id)
                {
                    case CancelId:
                        updatedButton.Name = Messages.MessageBoxCancelText;
                        break;

                    case OkId:
                        updatedButton.Name = Messages.MessageBoxOkText;
                        break;

                    case YesId:
                        updatedButton.Name = Messages.MessageBoxYesText;
                        break;

                    case NoId:
                        updatedButton.Name = Messages.MessageBoxNoText;
                        break;
                }
            }

            MessageBoxProviderResult result = MessageBoxProviderResult.Cancel;
            var resultButton = await this.ShowAsync(message, title, customButtons);
            if (resultButton != null)
            {
                if (resultButton.Id == OkId || resultButton.Id == YesId)
                {
                    result = MessageBoxProviderResult.Ok;
                }
            }

            return result;
        }

        public async Task<DialogButton> ShowAsync(string message, string title, IList<DialogButton> buttons)
        {
            var resultButton = await this.PerformShow(message, title, buttons); 
            if (resultButton != null)
            {
                var action = resultButton.Action;
                if (action != null)
                {
                    action.Invoke();
                }
            }

            return resultButton;
        }

        protected virtual Task<DialogButton> PerformShow(string message, string title, IList<DialogButton> buttons)
        {
            var cancelButton = buttons.FirstOrDefault(b => b.IsUserDismissValue);
            return Task.FromResult(cancelButton);
        }
    }
}
