namespace BaseTools.Core.Models
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Collections.Specialized;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using BaseTools.Core.Utility;

    public class Subcollection : BindableObject, IList, ICollection, IEnumerable, INotifyCollectionChanged
    {
        private ObservableCollection<object> internalCollection = new ObservableCollection<object>();

        private IList originCollection;

        private int startIndex;

        private int partLength;

        private int endIndex;

        public bool IsFixedSize
        {
            get
            {
                return false;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.originCollection.IsReadOnly;
            }
        }

        public object this[int index]
        {
            get
            {
                return this.internalCollection[index];
            }
            set
            {
                throw new NotSupportedException();
            }
        }

        public int Count
        {
            get
            {
                return this.internalCollection.Count;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return this.originCollection.IsSynchronized;
            }
        }

        public object SyncRoot
        {
            get
            {
                return this.originCollection.SyncRoot;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public Subcollection(IList wrappedCollection, int startIndex, int length)
        {
            this.originCollection = wrappedCollection;
            this.startIndex = startIndex;
            this.partLength = length;
            this.endIndex = this.startIndex + this.partLength - 1;
            if (wrappedCollection != null)
            {
                var originalObservableCollection = this.originCollection as INotifyCollectionChanged;
                if (originalObservableCollection != null)
                {
                    originalObservableCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnOriginCollectionChanged);
                }

                this.ResetInternalCollection();
            }
        }

        private void ResetInternalCollection()
        {
            ObservableCollection<object> observableCollection = new ObservableCollection<object>();
            var items = this.originCollection.Cast<object>().Skip(this.startIndex).Take(this.partLength);
            foreach (object obj in items)
            {
                observableCollection.Add(obj);
            }

            var iNotifyObject = this.internalCollection as INotifyPropertyChanged;

            this.internalCollection.CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnInternalCollectionChanged);
            iNotifyObject.PropertyChanged -= new PropertyChangedEventHandler(this.OnInternalPropertyChanged);

            this.internalCollection = observableCollection;
            iNotifyObject = this.internalCollection as INotifyPropertyChanged;

            this.internalCollection.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnInternalCollectionChanged);
            iNotifyObject.PropertyChanged += new PropertyChangedEventHandler(this.OnInternalPropertyChanged);

            this.RaisePropertyChanged("Count");
            this.RaiseCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        private void OnInternalPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            this.RaisePropertyChanged(e.PropertyName);
        }

        private void OnInternalCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.RaiseCollectionChanged(e);
        }

        private void OnOriginCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    int index1 = this.ConvertFromOrigin(e.NewStartingIndex);

                    if (this.originCollection.Count < this.startIndex || index1 >= this.partLength)
                        break;

                    object obj1 = e.NewItems[0];
                    if (index1 < 0)
                    {
                        index1 = 0;
                        obj1 = this.originCollection[this.startIndex];
                    }

                    this.internalCollection.Insert(index1, obj1);
                    if (this.internalCollection.Count <= this.partLength)
                        break;
                    this.internalCollection.RemoveAt(this.internalCollection.Count - 1);
                    break;

                case NotifyCollectionChangedAction.Remove:
                    int index2 = this.ConvertFromOrigin(e.OldStartingIndex);
                    if (this.originCollection.Count + 1 < this.startIndex || index2 >= this.partLength)
                        break;
                    if (index2 < 0)
                        index2 = 0;
                    this.internalCollection.RemoveAt(index2);
                    if (this.originCollection.Count <= this.endIndex)
                        break;
                    this.internalCollection.Add(this.originCollection[this.endIndex]);
                    break;

                case NotifyCollectionChangedAction.Replace:
                    int index3 = this.ConvertFromOrigin(e.NewStartingIndex);
                    if (index3 < 0 || index3 >= this.internalCollection.Count)
                        break;
                    object obj2 = e.NewItems[0];
                    this.internalCollection[index3] = obj2;
                    break;

                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    this.ResetInternalCollection();
                    break;
            }
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            NotifyCollectionChangedEventHandler changedEventHandler = this.CollectionChanged;
            if (changedEventHandler == null)
                return;
            changedEventHandler((object)this, args);
        }

        private int ConvertFromOrigin(int originIndex)
        {
            return originIndex - this.startIndex;
        }

        private int ConvertToOrigin(int currentIndex)
        {
            return this.startIndex + currentIndex;
        }

        public bool Contains(object value)
        {
            return this.internalCollection.Contains(value);
        }

        public int IndexOf(object value)
        {
            return this.internalCollection.IndexOf(value);
        }

        public int Add(object value)
        {
            throw new NotSupportedException();
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void Insert(int index, object value)
        {
            throw new NotSupportedException();
        }

        public void Remove(object value)
        {
            throw new NotSupportedException();
        }

        public void RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            var icollection = (ICollection)this.internalCollection;
            icollection.CopyTo(array, index);
            //this.internalCollection.CopyTo(array, index);
        }

        public IEnumerator GetEnumerator()
        {
            return (IEnumerator)this.internalCollection.GetEnumerator();
        }
    }
}
