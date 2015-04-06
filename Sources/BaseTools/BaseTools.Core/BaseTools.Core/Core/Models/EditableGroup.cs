namespace BaseTools.Core.Models
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public class EditableGroup<TKey, TValue> : List<TValue>
    {
        public EditableGroup()
        {
        }

        public EditableGroup(TKey key, IEnumerable<TValue> values)
            : base(values)
        {
            this.Key = key;
        }

        public TKey Key { get; set; }
    }
}
