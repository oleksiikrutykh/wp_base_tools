namespace BaseTools.UI.Controls
{
    using System;
    using System.Windows.Input;
    using Microsoft.Phone.Shell;

    /// <summary>
    /// Behavior to handle connecting a <see cref="IApplicationBarMenuItem"/> to a Command.
    /// </summary>
    /// <typeparam name="T">The target object must derive from IApplicationBarMenuItem</typeparam>
    public class AppBarItemCommandBehavior<T>
            where T : IApplicationBarMenuItem
    {
        /// <summary>
        /// Target object.
        /// </summary>
        private readonly WeakReference targetObject;

        /// <summary>
        /// Can execute changed event handler for command.
        /// </summary>
        private readonly EventHandler commandCanExecuteChangedHandler;

        /// <summary>
        /// Current command.
        /// </summary>
        private ICommand command;

        /// <summary>
        /// Command parameter.
        /// </summary>
        private object commandParameter;

        /// <summary>
        /// Initializes a new instance of the AppBarItemCommandBehavior class. Constructor specifying the target object.
        /// </summary>
        /// <param name="targetObject">The target object the behavior is attached to.</param>
        public AppBarItemCommandBehavior(T targetObject)
        {
            this.targetObject = new WeakReference(targetObject);
            this.commandCanExecuteChangedHandler = new EventHandler(this.CommandCanExecuteChanged);
            ((T)this.targetObject.Target).Click += (s, e) => this.ExecuteCommand();
        }

        /// <summary>
        /// Gets or sets corresponding command to be execute and monitored for <see cref="ICommand.CanExecuteChanged"/>
        /// </summary>
        public ICommand Command
        {
            get
            {
                return this.command;
            }

            set
            {
                if (this.command != null)
                {
                    this.command.CanExecuteChanged -= this.commandCanExecuteChangedHandler;
                }

                this.command = value;
                if (this.command != null)
                {
                    this.command.CanExecuteChanged += this.commandCanExecuteChangedHandler;
                    this.UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// Gets or sets the parameter to supply the command during execution
        /// </summary>
        public object CommandParameter
        {
            get
            {
                return this.commandParameter;
            }

            set
            {
                if (this.commandParameter != value)
                {
                    this.commandParameter = value;
                    this.UpdateEnabledState();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether object to which this behavior is attached.
        /// </summary>
        protected T TargetObject
        {
            get
            {
                return (T)this.targetObject.Target;
            }
        }

        /// <summary>
        /// Updates the target object's IsEnabled property based on the commands ability to execute.
        /// </summary>
        protected virtual void UpdateEnabledState()
        {
            if (this.TargetObject == null)
            {
                this.Command = null;
                this.CommandParameter = null;
            }
            else if (this.Command != null)
            {
                this.TargetObject.IsEnabled = this.Command.CanExecute(this.CommandParameter);
            }
        }

        /// <summary>
        /// Executes the command, if it's set, providing the <see cref="CommandParameter"/>
        /// </summary>
        protected virtual void ExecuteCommand()
        {
            if (this.Command != null)
            {
                this.Command.Execute(this.CommandParameter);
            }
        }

        /// <summary>
        /// Can command execute changed.
        /// </summary>
        /// <param name="sender">Object sender</param>
        /// <param name="e">Event arguments</param>
        private void CommandCanExecuteChanged(object sender, EventArgs e)
        {
            this.UpdateEnabledState();
        }
    }
}