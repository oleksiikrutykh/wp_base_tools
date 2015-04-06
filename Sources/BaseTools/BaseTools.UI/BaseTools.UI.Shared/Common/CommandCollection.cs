namespace BaseTools.UI.Common
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.Windows.Input;

    /// <summary>
    /// This class need for create collection CommandItem
    /// </summary>
    public class CommandCollection : ObservableCollection<CommandItem>, ICommand
    {
        private List<bool> storedCanExecuteList = new List<bool>();

        public CommandCollection()
        {
            this.CollectionChanged += OnCollectionChanged;
        }

        public CommandCollectionCanExecuteMode CanExecuteMode { get; set; }

        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Call all internal commands.
        /// </summary>
        public void InvokeCommands()
        {
            var canExecute = this.CanExecute(null);
            if (canExecute)
            {
                this.Execute(null);
            }
        }

        public bool CanExecute(object parameter)
        {
            bool totalCanExecute = true;
            this.storedCanExecuteList.Clear();
            if (this.Count > 0)
            {
                if (this.CanExecuteMode == CommandCollectionCanExecuteMode.Any)
                {
                    totalCanExecute = false;
                }

                foreach (var commandItem in this)
                {
                    var currentCanExecute = commandItem.CanExecute(parameter);
                    if (this.CanExecuteMode == CommandCollectionCanExecuteMode.Any)
                    {
                        if (currentCanExecute)
                        {
                            totalCanExecute = true;
                        }
                    }
                    else if (this.CanExecuteMode == CommandCollectionCanExecuteMode.All)
                    {
                        if (!currentCanExecute)
                        {
                            totalCanExecute = false;
                        }
                    }

                    this.storedCanExecuteList.Add(currentCanExecute);
                }
            }

            return totalCanExecute;
        }

        public void Execute(object parameter)
        {
            for (int i = 0; i < this.Count; i++)
            {
                var commandItem = this[i];
                var canExecute = this.storedCanExecuteList[i];
                if (canExecute)
                {
                    commandItem.Execute(parameter);
                }
            }
        }

        private void OnCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.NewItems != null)
                {
                    foreach (CommandItem item in e.NewItems)
                    {
                        item.CanExecuteChanged += OnItemCanExecuteChanged;
                    }
                }
            }

            if (e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Replace)
            {
                if (e.OldItems != null)
                {
                    foreach (CommandItem item in e.OldItems)
                    {
                        item.CanExecuteChanged -= OnItemCanExecuteChanged;
                    }
                }
            }

            //TODO: clear can execute
            if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (CommandItem item in this)
                {
                    item.CanExecuteChanged += OnItemCanExecuteChanged;
                }
            }

            this.RaiseCanExecuteChanged();
        }

        private void OnItemCanExecuteChanged(object sender, System.EventArgs e)
        {
            this.RaiseCanExecuteChanged();
        }

        private void RaiseCanExecuteChanged()
        {
            var handler = this.CanExecuteChanged;
            if (handler != null)
            {
                handler.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
