using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools.CSharp.Collections
{
    public abstract class BaseFastCollection<T> : IEnumerable<T>
    {
        #region private
        private readonly T _startValue;
        private readonly T _endValue;
        private readonly ulong _count;
        //---------------------------------------------------------------------
        private sealed class InternalEnumerator : IEnumerator<T>
        {
            #region private
            private readonly BaseFastCollection<T> _owner;
            private readonly ulong _count;
            //-----------------------------------------------------------------
            private ulong _index;
            private bool _isIndexInitialized;
            #endregion
            public InternalEnumerator(BaseFastCollection<T> owner)
            {
                _owner = owner;
                _count = _owner.Count;
                _index = 0;
                _isIndexInitialized = true;
            }

            //-----------------------------------------------------------------
            public T Current
            {
                get { return _owner[_index]; }
            }
            object IEnumerator.Current
            {
                get { return Current; }
            }
            //-----------------------------------------------------------------
            public bool MoveNext()
            {
                if (_isIndexInitialized)
                {
                    _isIndexInitialized = false;
                    return (_index < _count);
                }

                if (_index < _count)
                {
                    _index++;
                    return (_index < _count);
                }
                return false;
            }
            public void Reset()
            {
                _index = 0;
                _isIndexInitialized = true;
            }
            public void Dispose()
            { }
            //-----------------------------------------------------------------
        }
        #endregion
        #region protected
        protected abstract ulong CreateCount(T startValue, T endValue);
        protected abstract T GetValue(T startValue, ulong index);
        protected abstract bool IsValueRange(T value, T startValue, T endValue);
        #endregion
        protected BaseFastCollection(T startValue, T endValue)
        {
            _startValue = startValue;
            _endValue = endValue;

            _count = CreateCount(startValue, endValue);
        }

        //---------------------------------------------------------------------
        public T this[ulong index]
        {
            get
            {
                if (index > _count)
                { throw new ArgumentOutOfRangeException(nameof(index), index, string.Empty); }

                return GetValue(_startValue, index);
            }
        }
        public ulong Count
        {
            get { return _count; }
        }
        //---------------------------------------------------------------------
        public T MaxValue
        {
            get { return _endValue; }
        }
        public T MinValue
        {
            get { return _startValue; }
        }
        //---------------------------------------------------------------------
        public bool IsEmpty
        {
            get { return _count == 0; }
        }
        //---------------------------------------------------------------------
        public bool IsValueAvailable(T value)
        {
            return IsValueRange(value, _startValue, _endValue);
        }
        //---------------------------------------------------------------------
        public IEnumerator<T> GetEnumerator()
        {
            return new InternalEnumerator(this);
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}
