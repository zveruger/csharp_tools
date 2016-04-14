using System;
using System.ComponentModel;
using System.Linq.Expressions;

namespace Tools.CSharp.Extensions
{
    public static class EventArgsExtensions
    {
        #region private
        private static void _Raise<T>(this Func<string, EventArgs> valueFactory, object sender, ref EventHandler eventDelegate, Expression<Func<T>> propertyExpression)
        {
            if (valueFactory == null)
            { throw new ArgumentNullException(nameof(valueFactory)); }

            eventDelegate?.Invoke(sender, valueFactory(propertyExpression != null ? _CreateMemberExpressionName(propertyExpression) : string.Empty));
        }
        private static void _Raise<TEventArgs, T>(this Func<string, TEventArgs> valueFactory, object sender, ref EventHandler<TEventArgs> eventDelegate, Expression<Func<T>> propertyExpression)
           where TEventArgs : EventArgs
        {
            if (valueFactory == null)
            { throw new ArgumentNullException(nameof(valueFactory)); }

            eventDelegate?.Invoke(sender, valueFactory(propertyExpression != null ? _CreateMemberExpressionName(propertyExpression) : string.Empty));
        }
        private static void _Raise<T>(this Func<string, PropertyChangedEventArgs> valueFactory, object sender, ref PropertyChangedEventHandler eventDelegate, Expression<Func<T>> propertyExpression)
        {
            if (valueFactory == null)
                throw new ArgumentNullException(nameof(valueFactory));

            eventDelegate?.Invoke(sender, valueFactory(propertyExpression != null ? _CreateMemberExpressionName(propertyExpression) : string.Empty));
        }

        private static string _CreateMemberExpressionName<T>(Expression<Func<T>> propertyExpression)
        {
            var unaryExpression = propertyExpression.Body as UnaryExpression;
            var memberExpression = unaryExpression != null ? unaryExpression.Operand as MemberExpression : propertyExpression.Body as MemberExpression;

            if (memberExpression == null)
            { throw new NotSupportedException("Invalid expression passed. Only property member should be selected."); }

            return memberExpression.Member.Name;
        }
        #endregion
        //---------------------------------------------------------------------
        public static void Raise(this EventArgs e, object sender, ref EventHandler eventDelegate)
        {
            _Raise<object>(argument => e, sender, ref eventDelegate, null);
        }
        public static void Raise(this Func<EventArgs> valueFactory, object sender, ref EventHandler eventDelegate)
        {
            _Raise<object>(argument => valueFactory(), sender, ref eventDelegate, null);
        }
        public static void Raise<T>(this Func<string, EventArgs> valueFactory, object sender, ref EventHandler eventDelegate, Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            { throw new ArgumentNullException(nameof(propertyExpression)); }

            _Raise(valueFactory, sender, ref eventDelegate, propertyExpression);
        }
        public static void Raise(this PropertyChangedEventArgs e, object sender, ref PropertyChangedEventHandler eventDelegate)
        {
            _Raise<object>(argument => e, sender, ref eventDelegate, null);
        }
        public static void Raise<T>(this Func<string, PropertyChangedEventArgs> valueFactory, object sender, ref PropertyChangedEventHandler eventDelegate, Expression<Func<T>> propertyExpression)
        {
            if (propertyExpression == null)
            { throw new ArgumentNullException(nameof(propertyExpression)); }

            _Raise(valueFactory, sender, ref eventDelegate, propertyExpression);
        }
        //---------------------------------------------------------------------
        public static void Raise<TEventArgs>(this TEventArgs e, object sender, ref EventHandler<TEventArgs> eventDelegate)
            where TEventArgs : EventArgs
        {
            _Raise<TEventArgs, object>(arg => e, sender, ref eventDelegate, null);
        }
        public static void Raise<TEventArgs>(this Func<TEventArgs> valueFactory, object sender, ref EventHandler<TEventArgs> eventDelegate)
           where TEventArgs : EventArgs
        {
            _Raise<TEventArgs, object>(arg => valueFactory(), sender, ref eventDelegate, null);
        }
        public static void Raise<TEventArgs, T>(this Func<string, TEventArgs> valueFactory, object sender, ref EventHandler<TEventArgs> eventDelegate, Expression<Func<T>> propertyExpression)
           where TEventArgs : EventArgs
        {
            if (propertyExpression == null)
            { throw new ArgumentNullException(nameof(propertyExpression)); }

            _Raise(valueFactory, sender, ref eventDelegate, propertyExpression);
        }
        //---------------------------------------------------------------------
    }
}