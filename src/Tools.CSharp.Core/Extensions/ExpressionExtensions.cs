using System;
using System.Linq.Expressions;
using Tools.CSharp.Utils;

namespace Tools.CSharp.Extensions
{
    public static class ExpressionExtensions
    {
        //---------------------------------------------------------------------
        [Obsolete("Рекомендуется применять nameof(property).")]
        public static string NameOf<T>(this Expression<Func<T>> expression)
        {
            return GetString.Of(expression, true);
        }

        [Obsolete("Рекомендуется применять nameof(property).")]
        public static string NameOf(this Expression<Action> expression)
        {
            return GetString.Of(expression, true);
        }
        //---------------------------------------------------------------------
        public static string LastPartNameOf<T>(this Expression<Func<T>> expression)
        {
            return GetString.Of(expression, false);
        }
        public static string LastPartNameOf(this Expression<Action> expression)
        {
            return GetString.Of(expression, false);
        }
        //---------------------------------------------------------------------
    }
}