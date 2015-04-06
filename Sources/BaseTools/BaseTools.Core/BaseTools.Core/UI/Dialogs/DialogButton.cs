namespace BaseTools.UI.Dialogs
{
    using System;

    public class DialogButton
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public Action Action { get; set; }

        public bool IsDefault { get; set; }

        public bool IsUserDismissValue { get; set; }
    }
}
