namespace BaseTools.Core.Utility
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    public static class ListHelper
    {
        public static void ReplaceOrAdd<T>(IList<T> items, T newItem, Func<T, bool> equalityComparer)
        {
            var foundedIndex = -1;
            Guard.CheckIsNotNull(items, "items");
            Guard.CheckIsNotNull(equalityComparer, "equalityComparer");
            for (int i = 0; i < items.Count; i++)
            {
                var currentItem = items[i];
                bool isSameItem = equalityComparer.Invoke(currentItem);
                if (isSameItem)
                {
                    foundedIndex = i;
                    break;
                }
            }

            if (foundedIndex > -1)
            {
                items[foundedIndex] = newItem;
            }
            else
            {
                items.Add(newItem);
            }
        }

        public static int RemoveAll<T>(IList<T> items, Func<T, bool> removingPredicate)
        {
            int removesCount = 0;
            Guard.CheckIsNotNull(items, "items");
            Guard.CheckIsNotNull(removingPredicate, "removingPredicate");
            for (int i = 0; i < items.Count; i++)
            {
                var currentItem = items[i];
                bool needRemove = removingPredicate.Invoke(currentItem);
                if (needRemove)
                {
                    items.RemoveAt(i);
                    removesCount++;
                    i--;
                }
            }

            return removesCount;
        }

        public static void ReplaceOrAddByIndex(IList items, int index, object newItem)
        {
            if (index < items.Count)
            {
                items[index] = newItem;
            }
            else
            {
                items.Add(newItem);
            }
        }

        public static void Do()
        {
 
        }
    }
}
