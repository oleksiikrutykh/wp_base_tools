namespace BaseTools.UI.ControlExtensions
{
    using System;
    using System.Collections;
    using System.Collections.Specialized;
#if WINRT
    using Windows.UI.Xaml.Controls;
#endif

    public class СoupleItemsHelper
    {
        /// <summary>
        /// Gets the listview
        /// </summary>
        public WeakReference ListView { get; private set; }

        /// <summary>
        /// Gets the collection
        /// </summary>
        public IList Collection { get; private set; }

        private INotifyCollectionChanged observableCollection;

        /// <summary>
        /// Initialize instance of СoupleItemsHelper
        /// </summary>
        /// <param name="listView">ListView</param>
        /// <param name="collection">ObservableCollection</param>
        public СoupleItemsHelper(SelectorWrapper listView, IList collection)
        {
            this.ListView = new WeakReference(listView);
            this.Collection = collection;
            var observableCollection = this.Collection as INotifyCollectionChanged;
            if (observableCollection != null)
            {
                observableCollection.CollectionChanged += OnCollectionChanged;
            }
        }

        /// <summary>
        /// Clear object
        /// </summary>
        public void Clear()
        {
            this.ListView.Target = null;
            if (this.Collection != null)
            {
                if (this.observableCollection != null)
                {
                    this.observableCollection.CollectionChanged -= this.OnCollectionChanged;
                    this.observableCollection = null;
                }

                this.Collection = null;
            }
        }

        /// <summary>
        /// Occurs when items in collection changed
        /// </summary>
        /// <param name="sender">ObservableCollection</param>
        /// <param name="e">NotifyCollectionChangedEventArgs</param>
        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (ListView.IsAlive)
            {
                var listView = ListView.Target as SelectorWrapper;
                switch (e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        foreach (var item in e.NewItems)
                        {
                            if (!listView.SelectedItems.Contains(item))
                            {
                                listView.SelectedItems.Add(item);
                            }
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        foreach (var item in e.OldItems)
                        {
                            listView.SelectedItems.Remove(item);
                        }
                        break;
                    case NotifyCollectionChangedAction.Reset:
                        listView.SelectedItems.Clear();
                        break;
                }
            }
            else
            {
                if (this.observableCollection != null)
                {
                    this.observableCollection.CollectionChanged -= this.OnCollectionChanged;
                    this.observableCollection = null;
                }

                Collection = null;
            }
        }
    }
}
