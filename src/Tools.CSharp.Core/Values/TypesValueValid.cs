using System;
using System.Collections.Generic;

namespace Tools.CSharp.Values
{
    public class TypesValueValid
    {
        #region private
        private readonly Dictionary<Type, Delegate> _cacheValueCheckDelegates = new Dictionary<Type, Delegate>(); 
        private readonly Dictionary<Type, Delegate> _cacheValueCheckExtDelegates = new Dictionary<Type, Delegate>(); 
        private readonly Dictionary<Type, Delegate> _cacheGetValueAvailableDelages = new Dictionary<Type, Delegate>();
        //---------------------------------------------------------------------
        private static bool _AddInCacheDelegate<TValue>(IDictionary<Type, Delegate> cacheDelegates, Delegate method)
            where TValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            var typeValue = typeof(TValue);
            Delegate cacheDelegate;

            if (cacheDelegates.TryGetValue(typeValue, out cacheDelegate))
            { return false; }

            cacheDelegates.Add(typeValue, method);
            return true;
        }
        private static Delegate _GetFromCacheDelegate<TValue>(IDictionary<Type, Delegate> cacheDelegates)
        {
            Delegate cacheDelegate;

            return cacheDelegates.TryGetValue(typeof(TValue), out cacheDelegate) ? cacheDelegate : null;
        }
        #endregion
        #region protected
        protected void AddMethod<TInputValue>(ValueCheckDelegate<TInputValue> method)
           where TInputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            if (!_AddInCacheDelegate<TInputValue>(_cacheValueCheckDelegates, method))
            { throw new ArgumentException(string.Empty, nameof(method)); }
        }
        protected void AddMethod<TInputValue>(ValueCheckExtDelegate<TInputValue> method)
          where TInputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            if (!_AddInCacheDelegate<TInputValue>(_cacheValueCheckExtDelegates, method))
            { throw new ArgumentException(string.Empty, nameof(method)); }
        }
        protected void AddMethod<TInputOutputValue>(GetValueAvailableDelegate<TInputOutputValue> method)
          where TInputOutputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            if (!_AddInCacheDelegate<TInputOutputValue>(_cacheGetValueAvailableDelages, method))
            { throw new ArgumentException(string.Empty, nameof(method)); }
        }
        #endregion
        public TypesValueValid()
        {
            AddMethod<byte>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<byte>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
            //-----------------------------------------------------------------
            AddMethod<short>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<short>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
            //-----------------------------------------------------------------
            AddMethod<ushort>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<ushort>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
            //-----------------------------------------------------------------
            AddMethod<int>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<int>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
            //-----------------------------------------------------------------
            AddMethod<uint>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<uint>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
            //-----------------------------------------------------------------
            AddMethod<long>(((value, minValue, maxValue) => minValue <= value && value <= maxValue));
            AddMethod<ulong>(((value, minValue, maxValue) => value < minValue ? minValue : (value > maxValue ? maxValue : value)));
        }

        //---------------------------------------------------------------------
        public delegate bool ValueCheckDelegate<in TInputValue>(TInputValue value)
           where TInputValue : struct;
        public delegate bool ValueCheckExtDelegate<in TInputValue>(TInputValue value, TInputValue minValue, TInputValue maxValue) 
            where TInputValue : struct;
        public delegate TInputOutputValue GetValueAvailableDelegate<TInputOutputValue>(TInputOutputValue value, TInputOutputValue minValue, TInputOutputValue maxValue)
            where TInputOutputValue : struct;
        //---------------------------------------------------------------------
        public bool IsValidValue<TInputValue>(TInputValue value)
            where TInputValue : struct
        {
            var method = _GetFromCacheDelegate<TInputValue>(_cacheValueCheckDelegates) as ValueCheckDelegate<TInputValue>;

            if (method == null)
            { throw new InvalidOperationException(); }

            return method.Invoke(value);
        }
        public bool IsValidValue<TInputValue>(TInputValue value, ValueCheckDelegate<TInputValue> method)
            where TInputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            return method.Invoke(value);
        }
        public bool IsValidValue<TInputValue>(TInputValue value, TInputValue minValue, TInputValue maxValue)
             where TInputValue : struct
        {
            var method = _GetFromCacheDelegate<TInputValue>(_cacheValueCheckExtDelegates) as ValueCheckExtDelegate<TInputValue>;

            if (method == null)
            { throw new InvalidOperationException(); }

            return method.Invoke(value, minValue, maxValue);
        }
        public bool IsValidValue<TInputValue>(TInputValue value, TInputValue minValue, TInputValue maxValue, ValueCheckExtDelegate<TInputValue> method)
            where TInputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            return method.Invoke(value, minValue, maxValue);
        }
        //---------------------------------------------------------------------
        public TInputOutputValue GetValue<TInputOutputValue>(TInputOutputValue value, TInputOutputValue defaultValue)
           where TInputOutputValue : struct
        {
            var method = _GetFromCacheDelegate<TInputOutputValue>(_cacheValueCheckDelegates) as ValueCheckDelegate<TInputOutputValue>;

            if (method == null)
            { throw new InvalidOperationException(); }

            return method.Invoke(value) ? value : defaultValue;
        }
        public TInputOutputValue GetValue<TInputOutputValue>(TInputOutputValue value, TInputOutputValue minValue, TInputOutputValue maxValue)
             where TInputOutputValue : struct
        {
            var method = _GetFromCacheDelegate<TInputOutputValue>(_cacheGetValueAvailableDelages) as GetValueAvailableDelegate<TInputOutputValue>;

            if (method == null)
            { throw new InvalidOperationException(); }

            return method.Invoke(value, minValue, maxValue);
        }
        public TInputOutputValue GetValue<TInputOutputValue>(TInputOutputValue value, TInputOutputValue minValue, TInputOutputValue maxValue, GetValueAvailableDelegate<TInputOutputValue> method)
            where TInputOutputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            return method.Invoke(value, minValue, maxValue);
        }
        public TInputOutputValue GetValue<TInputOutputValue>(TInputOutputValue value, TInputOutputValue minValue, TInputOutputValue maxValue, TInputOutputValue defaultValue)
             where TInputOutputValue : struct
        {
            var method = _GetFromCacheDelegate<TInputOutputValue>(_cacheValueCheckExtDelegates) as ValueCheckExtDelegate<TInputOutputValue>;

            if (method == null)
            { throw new InvalidOperationException(); }

            return method.Invoke(value, minValue, maxValue) ? value : defaultValue;
        }
        public TInputOutputValue GetValue<TInputOutputValue>(TInputOutputValue value, TInputOutputValue minValue, TInputOutputValue maxValue, TInputOutputValue defaultValue, ValueCheckExtDelegate<TInputOutputValue> method)
            where TInputOutputValue : struct
        {
            if (method == null)
            { throw new ArgumentNullException(nameof(method)); }

            return method.Invoke(value, minValue, maxValue) ? value : defaultValue;
        }
        //---------------------------------------------------------------------
    }
}