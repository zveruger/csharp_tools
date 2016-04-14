using System;
using System.Collections.Generic;

namespace Tools.CSharp.Extensions
{
    public static class EnumerableExtensions
    {
        //---------------------------------------------------------------------
        public static void ForEach<T>(this IEnumerable<T> sequence, Action<T> action)
        {
            if (sequence == null)
            { throw new ArgumentNullException(nameof(sequence)); }
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            foreach(var item in sequence)
            {
                action(item);
            }
        }
        //---------------------------------------------------------------------
    }
}