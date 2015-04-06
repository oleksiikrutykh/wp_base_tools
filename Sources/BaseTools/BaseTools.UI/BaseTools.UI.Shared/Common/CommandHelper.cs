namespace BaseTools.UI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Windows.Input;

    public static class CommandExtensions
    {
        public static void ExecuteSafe(this ICommand command)
        {
            ExecuteSafe(command, null);
        }

        public static void ExecuteSafe(this ICommand command, object parameter)
        {
            if (command != null)
            {
                var canExecute = command.CanExecute(parameter);
                if (canExecute)
                {
                    command.Execute(parameter);
                }
            }
        }
    }
}
