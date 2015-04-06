namespace BaseTools.Core.Utility
{
    using BaseTools.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;


    public static class LinqExtesions
    {
        public static EdiatbleLookup<TKey, TValue> ToEdiatbleLookup<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> selector)
        {
            var items = new EdiatbleLookup<TKey, TValue>();
            var originGroups = collection.ToLookup<TValue, TKey>(selector);
            foreach (var group in originGroups)
            {
                var editableGroup = new EditableGroup<TKey, TValue>(group.Key, group);
                items.Add(editableGroup);
            }

            return items;
        }
    }
}
