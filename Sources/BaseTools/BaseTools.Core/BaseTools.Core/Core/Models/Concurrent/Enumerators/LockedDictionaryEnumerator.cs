namespace BaseTools.Core.Models.Concurrent
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    /// <summary>
    /// Enumerator for <see cref="IDictionary.GetEnumerator"/> method. 
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1038:EnumeratorsShouldBeStronglyTyped", Justification="Support any type")]
    internal class LockedDictionaryEnumerator : LockedEnumerator, IDictionaryEnumerator
    {
        public LockedDictionaryEnumerator(Func<IEnumerator> enumeratorGenerator, object syncValue)
            : base(enumeratorGenerator, syncValue)
        {
        }

        public DictionaryEntry Entry
        {
            get
            {
                return (DictionaryEntry)this.Current;
            }
        }

        public object Key
        {
            get
            {
                return this.Entry.Key;
            }
        }

        public object Value
        {
            get
            {
                return this.Entry.Value;
            }
        }
    }

}
