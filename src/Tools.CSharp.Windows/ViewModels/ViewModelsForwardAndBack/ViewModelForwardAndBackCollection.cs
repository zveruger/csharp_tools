using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools.CSharp.ViewModels
{
    public sealed class ViewModelForwardAndBackCollection : ICollection<IBaseViewModelForwardAndBack>
    {
        #region private
        private readonly List<IBaseViewModelForwardAndBack> _collection = new List<IBaseViewModelForwardAndBack>();
        private readonly ushort _ignoredNumberViewModels;
        private int _countWithoutIgnored;

        private void _countWithoutIgnoredUpdate()
        {
            var result = _collection.Count - _ignoredNumberViewModels;
            _countWithoutIgnored = result < 0 ? _collection.Count : result;
        }
        #endregion
        public ViewModelForwardAndBackCollection()
            : this(0)
        {
        }
        public ViewModelForwardAndBackCollection(ushort ignoredNumberViewModels)
        {
            _ignoredNumberViewModels = ignoredNumberViewModels;
        }

        //---------------------------------------------------------------------
        public IBaseViewModelForwardAndBack this[int index]
        {
            get { return _collection[index]; }
        }
        public int Count
        {
            get { return _collection.Count; }
        }
        public int CountWithoutIgnored
        {
            get { return _countWithoutIgnored; }
        }
        //---------------------------------------------------------------------
        public bool IsReadOnly
        {
            get { return false; }
        }
        //---------------------------------------------------------------------
        public void Add(IBaseViewModelForwardAndBack item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            var internalViewModelForwardAndBack = item as IInternalViewModelForwardAndBack;
            if (internalViewModelForwardAndBack == null)
            {
                _collection.Add(item);
            }
            else
            {
                if (_collection.Count != 0)
                {
                    var lastViewModelForwardAndBack = _collection[_collection.Count - 1];

                    var internalLastViewModelForwardAndBack = lastViewModelForwardAndBack as IInternalViewModelForwardAndBack;
                    internalLastViewModelForwardAndBack?.SetForwardViewModel(item);

                    internalViewModelForwardAndBack.SetBackViewModel(lastViewModelForwardAndBack);
                }

                _collection.Add(item);
                internalViewModelForwardAndBack.SetCurrentViewModeIndex(_collection.Count - 1);
            }

            _countWithoutIgnoredUpdate();
        }
        public void Clear()
        {
            _collection.Clear();

            _countWithoutIgnoredUpdate();
        }
        public bool Contains(IBaseViewModelForwardAndBack item)
        {
            return _collection.Contains(item);
        }
        public void CopyTo(IBaseViewModelForwardAndBack[] array, int arrayIndex)
        {
            _collection.CopyTo(array, arrayIndex);
        }
        public bool Remove(IBaseViewModelForwardAndBack item)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        public IEnumerator<IBaseViewModelForwardAndBack> GetEnumerator()
        {
            return _collection.GetEnumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}