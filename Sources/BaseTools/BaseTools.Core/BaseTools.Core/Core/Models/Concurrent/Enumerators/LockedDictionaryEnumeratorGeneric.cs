namespace BaseTools.Core.Models.Concurrent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Enumerator for <see cref="ConcurrentDictionary.GetEnumerator"/> method. 
    /// </summary>
    /// <typeparam name="TKey">The type of keys in the dictionary.</typeparam>
    /// <typeparam name="TValue">The type of values in the dictionary.</typeparam>
    internal class LockedDictionaryEnumerator<TKey, TValue> : LockedEnumerator<KeyValuePair<TKey, TValue>>, IDictionaryEnumerator
    {
        private IDictionaryEnumerator dictionaryEnumerator;
       
        public LockedDictionaryEnumerator(Func<IEnumerator<KeyValuePair<TKey, TValue>>> enumeratorGenerator, object syncValue)
            : base(enumeratorGenerator, syncValue)
        {
            this.dictionaryEnumerator = (IDictionaryEnumerator)this.InternalEnumerator;
        }

        public DictionaryEntry Entry
        {
            get
            {
                return this.dictionaryEnumerator.Entry;
            }
        }

        public object Key
        {
            get
            {
                return this.dictionaryEnumerator.Key;
            }
        }

        public object Value
        {
            get
            {
                return this.dictionaryEnumerator.Value;
            }
        }

        object IEnumerator.Current
        {
            get
            {
                return this.dictionaryEnumerator.Current;
            }
        }
    }
}
