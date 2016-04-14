using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Tools.CSharp.Extensions
{
    public static class StringCollectionExtensions
    {
        //---------------------------------------------------------------------
        public static ReadOnlyCollection<string> AsReadOnly(this StringCollection collection)
        {
            if (collection == null)
            { throw new ArgumentNullException(nameof(collection)); }

            var result = new List<string>(collection.Count);
            result.AddRange(collection.Cast<string>());

            return result.AsReadOnly();
        }
        //---------------------------------------------------------------------
    }
}