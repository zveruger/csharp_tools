using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Tools.CSharp.Extensions
{
    public static class EnumExtensions
    {
        //---------------------------------------------------------------------
        public static IEnumerable<T> GetAllItems<T>(this Enum value)
        {
            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            return (from object item in Enum.GetValues(typeof(T)) select (T)item);
        }
        public static IEnumerable<T> GetAllItems<T>() where T : struct
        {
            return Enum.GetValues(typeof(T)).Cast<T>();
        }
        //---------------------------------------------------------------------
        public static IEnumerable<T> GetAllSelectedItems<T>(this Enum value)
        {
            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            return from object item in Enum.GetValues(typeof(T))
                   let itemAsInt = Convert.ToInt32(item, CultureInfo.InvariantCulture)
                   where itemAsInt == (valueAsInt & itemAsInt) select (T)item;
        }
        //---------------------------------------------------------------------
        public static bool Contains<TRequest>(this Enum value, TRequest request)
        {
            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            var valueAsInt = Convert.ToInt32(value, CultureInfo.InvariantCulture);
            var requestAsInt = Convert.ToInt32(request, CultureInfo.InvariantCulture);

            return (requestAsInt == (requestAsInt & valueAsInt));
        }
        //---------------------------------------------------------------------
    }
}
