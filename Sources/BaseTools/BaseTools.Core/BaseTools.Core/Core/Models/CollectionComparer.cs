namespace BaseTools.Core.Models
{
    using BaseTools.Core.Utility;
    using System;
    using System.Collections.Generic;
    using System.Text;

    /// <summary>
    /// Performs different comparings of two collections.
    /// </summary>
    public static class CollectionComparer
    {
        /// <summary>
        /// Compares all alement of collections by equality.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="left">One collection for comparing</param>
        /// <param name="right">Another collection for comparing</param>
        /// <param name="comparer">Items comparing function</param>
        /// <returns>True, if collection are equals, otherwise false</returns>
        public static bool CompareCollections<T>(ICollection<T> left, ICollection<T> right, Func<T, T, bool> comparer)
        {
            return CompareCollections<T>(left, right, comparer, true);
        }

        /// <summary>
        /// Compares all alement of collections by equality.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="left">One collection for comparing</param>
        /// <param name="right">Another collection for comparing</param>
        /// <param name="comparer">Items comparing function</param>
        /// <param name="isEqualEmptyCollection">Flag which shows equals the empty collections or not.</param>
        /// <returns>True, if collection are equals, otherwise false</returns>
        public static bool CompareCollections<T>(ICollection<T> left, ICollection<T> right, Func<T, T, bool> comparer, bool isEqualEmptyCollection)
        {
            Guard.CheckIsNotNull(comparer, "comparer");
            bool isEquals = true;
            if (left == null && right == null)
            {
                isEquals = isEqualEmptyCollection;
            }
            else
            {
                if (left != null && right != null && left.Count == right.Count)
                {
                    foreach (var leftItem in left)
                    {
                        bool hasEqualAudioItem = false;
                        foreach (var rightItem in right)
                        {
                            if (comparer.Invoke(leftItem, rightItem))
                            {
                                hasEqualAudioItem = true;
                                break;
                            }
                        }

                        if (!hasEqualAudioItem)
                        {
                            isEquals = false;
                            break;
                        }
                    }
                }
                else
                {
                    isEquals = false;
                }
            }

            return isEquals;
        }

        /// <summary>
        /// Compare consequentially each element of collection on equality.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="left">One collection for comparing</param>
        /// <param name="right">Another collection for comparing</param>
        /// <returns>True, if collection are equals, otherwise false</returns>
        public static bool CompareCollectionsConsequentially<T>(IList<T> left, IList<T> right)
        {
            return CompareCollectionsConsequentially<T>(left, right, EqualityComparer<T>.Default.Equals, true);
        }

        /// <summary>
        /// Compare consequentially each element of collection on equality.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="left">One collection for comparing</param>
        /// <param name="right">Another collection for comparing</param>
        /// <param name="comparer">Items comparing function</param>
        /// <returns>True, if collection are equals, otherwise false</returns>
        public static bool CompareCollectionsConsequentially<T>(IList<T> left, IList<T> right, Func<T, T, bool> comparer)
        {
            return CompareCollectionsConsequentially<T>(left, right, comparer, true);
        }

        /// <summary>
        /// Compare consequentially each element of collection on equality.
        /// </summary>
        /// <typeparam name="T">Type of collection item.</typeparam>
        /// <param name="left">One collection for comparing</param>
        /// <param name="right">Second collection for comparing</param>
        /// <param name="comparer">Items comparing function</param>
        /// <param name="isEqualEmptyCollection">Flag which shows equals the empty collections or not.</param>
        /// <returns>True, if collection are equals, otherwise false</returns>
        public static bool CompareCollectionsConsequentially<T>(IList<T> left, IList<T> right, Func<T, T, bool> comparer, bool isEqualEmptyCollection)
        {
            Guard.CheckIsNotNull(comparer, "comparer");
            bool isEquals = true;
            if (left == null && right == null)
            {
                isEquals = isEqualEmptyCollection;
            }
            else
            {
                if (left != right)
                {
                    if (left != null && right != null && left.Count == right.Count)
                    {
                        for (int i = 0; i < left.Count; i++)
                        {
                            if (!comparer.Invoke(left[i], right[i]))
                            {
                                isEquals = false;
                                break;
                            }
                        }
                    }
                    else
                    {
                        isEquals = false;
                    }
                }
            }

            return isEquals;
        }
    }
}
