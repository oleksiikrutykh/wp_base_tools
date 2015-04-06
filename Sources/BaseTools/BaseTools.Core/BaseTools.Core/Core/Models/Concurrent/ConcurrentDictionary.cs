namespace BaseTools.Core.Models.Concurrent
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Generic collection of key/value pairs that blocks multithread access.
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    public sealed class ConcurrentDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IDictionary
    {
        private IDictionary<TKey, TValue> internalDictionary;

        private IDictionary objectDictionary;

        private readonly object DictionaryLock = new object();

        public ConcurrentDictionary()
        {
            var dictionary = new Dictionary<TKey, TValue>();
            this.internalDictionary = dictionary;
            this.objectDictionary = dictionary;
        }

        public TValue GetOrAdd(TKey key)
        {
            return GetOrAdd(key, this.GenerateDefaultValue);
        }

        private TValue GenerateDefaultValue()
        {
            return (TValue)Activator.CreateInstance(typeof(TValue));
        }

        public TValue GetOrAdd(TKey key, Func<TValue> valueGenerator)
        {
            lock (DictionaryLock)
            {
                TValue value = default(TValue);
                Guard.CheckIsNotNull(valueGenerator, "valueGenerator");
                var isFounded = this.internalDictionary.TryGetValue(key, out value);
                if (!isFounded)
                {
                    value = valueGenerator.Invoke();
                    this.internalDictionary.Add(key, value);
                }

                return value;
            }
        }

        public TValue GetOrAdd(TKey key, Func<TKey, TValue> valueGenerator)
        {
            return this.GetOrAdd(key, () => 
            { 
                return valueGenerator.Invoke(key); 
            });
        }

        public void Add(TKey key, TValue value)
        {
            lock (DictionaryLock)
            {
                this.internalDictionary.Add(key, value);
            }
        }

        public bool ContainsKey(TKey key)
        {
            lock (DictionaryLock)
            {
                return this.internalDictionary.ContainsKey(key);
            }
        }

        public ICollection<TKey> Keys
        {
            get
            {
                // TODO: lock working with Keys collection. 
                return this.internalDictionary.Keys;
            }
        }

        public bool Remove(TKey key)
        {
            lock (this.DictionaryLock)
            {
                return this.internalDictionary.Remove(key);
            }
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            lock (this.DictionaryLock)
            {
                return this.internalDictionary.TryGetValue(key, out value);
            }
        }

        public ICollection<TValue> Values
        {
            get
            {
                // TODO: lock working with Values collection. 
                return this.internalDictionary.Values;
            }
        }

        public TValue this[TKey key]
        {
            get
            {
                lock (this.DictionaryLock)
                {
                    return this.internalDictionary[key];
                }
            }
            set
            {
                lock (DictionaryLock)
                {
                    this.internalDictionary[key] = value;
                }
            }
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            lock (this.DictionaryLock)
            {
                this.internalDictionary.Add(item);
            }
        }

        public void Clear()
        {
            lock (this.DictionaryLock)
            {
                this.internalDictionary.Clear();
            }
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            lock (this.DictionaryLock)
            {
                return this.internalDictionary.Contains(item);
            }
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            lock (this.DictionaryLock)
            {
                this.internalDictionary.CopyTo(array, arrayIndex);
            }
        }

        public int Count
        {
            get
            {
                lock (this.DictionaryLock)
                {
                    return this.internalDictionary.Count;
                }
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return this.internalDictionary.IsReadOnly;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            lock (this.DictionaryLock)
            {
                return this.internalDictionary.Remove(item);
            }
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            var enumerator = new LockedDictionaryEnumerator<TKey, TValue>(
                                    () => this.internalDictionary.GetEnumerator(),
                                    this.DictionaryLock
                             );
            return enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        void IDictionary.Add(object key, object value)
        {
            this.Add((TKey)key, (TValue)value);
        }

        bool IDictionary.Contains(object key)
        {
            return this.ContainsKey((TKey)key);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:DisposeObjectsBeforeLosingScope", Justification = "Can't disposed")]
        IDictionaryEnumerator IDictionary.GetEnumerator()
        {
            var enumerator = new LockedDictionaryEnumerator(() =>
            {
                return this.objectDictionary.GetEnumerator();
            },
            this.DictionaryLock);

            return enumerator;
        }

        bool IDictionary.IsFixedSize
        {
            get 
            {
                return false; 
            }
        }

        ICollection IDictionary.Keys
        {
            get 
            {
                return this.objectDictionary.Keys;
            }
        }

        void IDictionary.Remove(object key)
        {
            this.Remove((TKey)key);
        }

        ICollection IDictionary.Values
        {
            get 
            {
                return this.objectDictionary.Values;
            }
        }

        object IDictionary.this[object key]
        {
            get
            {
                return this[(TKey)key];
            }
            set
            {
                this[(TKey)key] = (TValue)value;
            }
        }

        public void CopyTo(Array array, int index)
        {
            lock (this.DictionaryLock)
            {
                this.objectDictionary.CopyTo(array, index);
            }
        }

        public bool IsSynchronized
        {
            get 
            {
                return false;
            }
        }

        public object SyncRoot
        {
            get 
            { 
                throw new NotSupportedException(); 
            }
        }
    }
}
