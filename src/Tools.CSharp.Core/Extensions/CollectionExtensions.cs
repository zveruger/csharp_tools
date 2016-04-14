using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tools.CSharp.Extensions
{
    public static class CollectionExtensions
    {
        //---------------------------------------------------------------------
        public static void AddRange<T>(this Collection<T> collection, IEnumerable<T> items)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }
            if (items == null)
            { throw new ArgumentNullException(nameof(items)); }

            foreach(var item in items)
            {
                collection.Add(item);
            }
        }
        //---------------------------------------------------------------------
        public static int GetNextSelectedIndex(this ICollection collection, int currentIndex, bool isCurrentIndexRemove)
        {
            var nextIndex = GetNextSelectedIndex(collection?.Count ?? -1, currentIndex, isCurrentIndexRemove);
            return nextIndex;
        }
        public static int GetNextSelectedIndex(int collectionCount, int currentIndex, bool isCurrentIndexRemove)
        {
            var nextIndex = -1;

            if (collectionCount > 0)
            {
                if (currentIndex < 0)
                { nextIndex = 0; }
                else
                {
                    if (isCurrentIndexRemove)
                    {
                        if (currentIndex == collectionCount)
                        { nextIndex = collectionCount - 1; }
                        else if (currentIndex < collectionCount)
                        { nextIndex = currentIndex; }
                        else
                        { nextIndex = collectionCount; }
                    }
                    else
                    {
                        var lastCollectionIndex = collectionCount - 1;
                        if (currentIndex < lastCollectionIndex)
                        { nextIndex = currentIndex + 1; }
                        else
                        { nextIndex = lastCollectionIndex; }
                    }
                }
            }

            return nextIndex;
        }
        //---------------------------------------------------------------------
    }
}