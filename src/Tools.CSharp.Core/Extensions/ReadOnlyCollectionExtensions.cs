using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Tools.CSharp.Extensions
{
    public static class ReadOnlyCollectionExtensions
    {
        //---------------------------------------------------------------------
        public static StringCollection AsStringCollection(this ReadOnlyCollection<string> collection)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            var result = new StringCollection();

            foreach (var item in collection)
            { result.Add(item); }

            return result;
        }
        //---------------------------------------------------------------------
    }
}