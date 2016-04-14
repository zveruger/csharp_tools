using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Tools.CSharp.Collections
{
    public class MutatingObservableCollection<T> : ObservableCollection<T>
    {
        #region private
        private bool _isMutating;

        private static void _ThrowNotSupportedException()
        {
            throw new NotSupportedException();
        }
        #endregion
        #region protected
        protected override void RemoveItem(int index)
        {
            if (!_isMutating)
            { _ThrowNotSupportedException(); }
            else
            { base.RemoveItem(index); }
        }
        protected override void SetItem(int index, T item)
        {
            if (!_isMutating)
            { _ThrowNotSupportedException(); }
            else
            { base.SetItem(index, item); }
        }
        protected override void InsertItem(int index, T item)
        {
            if (!_isMutating)
            { _ThrowNotSupportedException(); }
            else
            { base.InsertItem(index, item); }
        }
        protected override void ClearItems()
        {
            if (!_isMutating)
            { _ThrowNotSupportedException(); }
            else
            { base.ClearItems(); }
        }
        #endregion
        public MutatingObservableCollection()
        {
        }
        public MutatingObservableCollection(IEnumerable<T> collection)
            : base(collection)
        {
        }
        public MutatingObservableCollection(List<T> list)
            : base(list)
        {
        }

        //---------------------------------------------------------------------
        public void Mutating(Action<MutatingObservableCollection<T>> action)
        {
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            _isMutating = true;
            try
            {
                action(this);
            }
            finally
            {
                _isMutating = false;
            }
        }
        //---------------------------------------------------------------------
    }
}