using System;
using System.Collections.Generic;

namespace Tools.CSharp.Extensions
{
    public static class ListExtensions
    {
        //---------------------------------------------------------------------
        public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> collection1, IEnumerable<T> collection2)
        {
            if (list == null)
            { throw new ArgumentNullException(nameof(list)); }

            list.AddRange(collection1);
            list.AddRange(collection2);

            return list;
        }
        public static List<T> AddRange<T>(this List<T> list, params IEnumerable<T>[] collections)
        {
            if (list == null)
            { throw new ArgumentNullException(nameof(list)); }

            foreach (var collection in collections)
            { list.AddRange(collection); }

            return list;
        }
        public static List<T> AddRange<T>(this List<T> list, IEnumerable<T> collection, params T[] items)
        {
            if (list == null)
            { throw new ArgumentNullException(nameof(list)); }

            list.AddRange(collection);
            list.AddRange(items);

            return list;
        }
        //---------------------------------------------------------------------
    }
}
