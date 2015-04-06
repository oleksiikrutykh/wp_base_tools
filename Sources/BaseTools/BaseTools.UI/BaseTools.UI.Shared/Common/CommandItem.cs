namespace BaseTools.UI.Common
{
    using BaseTools.Core.Common;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Input;

#if WINRT
    using Windows.UI.Xaml;
    using System.Threading.Tasks;
#endif

    /// <summary>
    /// Represents an item for <see cref="CommandCollection"/>.
    /// </summary>
    public class CommandItem : FrameworkElement, ICommand
    {
        /// <summary>
        /// Identifies Command the dependency property
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable class")]
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICommand), typeof(CommandItem), new PropertyMetadata(null, OnCommandPropertyChanged));

        /// <summary>
        /// Identifies CommandParameter the dependency property
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Justification = "DependencyProperty is immutable class")]
        public static readonly DependencyProperty CommandParameterProperty =
            DependencyProperty.Register("CommandParameter", typeof(object), typeof(CommandItem), new PropertyMetadata(null));

        private WeakEventHandler canExecuteEventWeakHandler;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommandItem"/> class.
        /// </summary>
        public CommandItem()
        {
            this.Command = null;
            this.CommandParameter = null;
            this.canExecuteEventWeakHandler = new WeakEventHandler(this.OnCommandCanExecuteChanged);
        }

        /// <summary>
        /// Gets or sets a value that indicate what command will be doing 
        /// </summary>
        /// <value>The command.</value>
        public ICommand Command
        {
            get 
            { 
                return (ICommand)GetValue(CommandProperty);
            }

            set 
            { 
                SetValue(CommandProperty, value); 
            }
        }

        /// <summary>
        /// Gets or sets a value that indicate what parameters are passed to the command
        /// </summary>
        /// <value>The command parameter.</value>
        public object CommandParameter
        {
            get 
            { 
                return GetValue(CommandParameterProperty); 
            }

            set
            { 
                SetValue(CommandParameterProperty, value); 
            }
        }

        public event EventHandler CommandInvoked;

        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter)
        {
            bool canExecute = false;
            var command = this.Command;
            if (command != null)
            {
                var commandParameter = this.DetermineParameterForCommand(parameter);
                canExecute = command.CanExecute(commandParameter);
            }

            return canExecute;
        }

        public void Execute(object parameter)
        {
            var command = this.Command;
            var commandParameter = this.DetermineParameterForCommand(parameter);
            this.ExecuteCommandIfPossible(command, commandParameter);
        }

        /// <summary>
        /// Execute command associated with command item in selected thread.   
        /// </summary>
        public void Invoke()
        {
            if (this.CanExecute(null))
            {
                this.Execute(null);
            }
        }

        private static void OnCommandPropertyChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        {
            var commandItem = (CommandItem)sender;
            commandItem.OnCommandChanged(args);
        }

        private void OnCommandChanged(DependencyPropertyChangedEventArgs args)
        {
            var previousValue = (ICommand)args.OldValue;
            if (previousValue != null)
            {
                previousValue.CanExecuteChanged -= this.canExecuteEventWeakHandler.Handler;
            }

            var newValue = (ICommand)args.NewValue;
            if (newValue != null)
            {
                newValue.CanExecuteChanged += this.canExecuteEventWeakHandler.Handler;
            }

            this.OnCommandCanExecuteChanged(this, EventArgs.Empty);
        }

        private void OnCommandCanExecuteChanged(object sender, EventArgs args)
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, args);
            }
        }

        /// <summary>
        /// Execute command if CanExecute method return true.
        /// </summary>
        /// <param name="command">command needed to execute</param>
        /// <param name="commandParameter">Parameter of command.</param>
        private void ExecuteCommandIfPossible(ICommand command, object commandParameter)
        {
            if (command != null)
            {
                command.Execute(commandParameter);
            }

            var handler = this.CommandInvoked;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }

        private object DetermineParameterForCommand(object inputParameter)
        {
            var resultParameter = inputParameter;
            if (this.CommandParameter != null)
            {
                resultParameter = this.CommandParameter;
            }

            return resultParameter;
        }
    }
}
