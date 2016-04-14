using System;
using System.Collections;
using System.Collections.Generic;

namespace Tools.CSharp.Collections
{
    public sealed class MutatingReadOnlyCollection<T> : IList<T>, IList
    {
        #region private
        private readonly List<T> _list;
        #endregion
        public MutatingReadOnlyCollection()
        {
            _list = new List<T>();
        }
        public MutatingReadOnlyCollection(int capacity)
        {
            _list = new List<T>(capacity);
        }
        public MutatingReadOnlyCollection(IEnumerable<T> collection)
        {
            _list = new List<T>(collection);
        }

        //---------------------------------------------------------------------
        public void Mutating(Action<IList<T>> action)
        {
            if (action == null)
            { throw new ArgumentNullException(nameof(action)); }

            action(_list);
        }
        //---------------------------------------------------------------------
        public T this[int index]
        {
            get { return _list[index]; }
        }
        public int Count
        {
            get { return _list.Count; }
        }
        //---------------------------------------------------------------------
        public int IndexOf(T value)
        {
            return _list.IndexOf(value);
        }
        public bool Contains(T value)
        {
            return _list.Contains(value);
        }
        public void CopyTo(T[] array, int index)
        {
            _list.CopyTo(array, index);
        }
        public IEnumerator<T> GetEnumerator()
        {
            return _list.GetEnumerator();
        }
        //---------------------------------------------------------------------
        T IList<T>.this[int index]
        {
            get { return _list[index]; }
            set { throw new NotSupportedException(); }
        }
        void IList<T>.Insert(int index, T value)
        {
            throw new NotSupportedException();
        }
        void IList<T>.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }

        bool IList.IsFixedSize
        {
            get { return true; }
        }
        bool IList.IsReadOnly
        {
            get { return true; }
        }
        object IList.this[int index]
        {
            get { return _list[index]; }
            set { throw new NotSupportedException(); }
        }
        int IList.Add(object value)
        {
            throw new NotSupportedException();
        }
        void IList.Clear()
        {
            throw new NotSupportedException();
        }
        bool IList.Contains(object value)
        {
            return ((IList)_list).Contains(value);
        }
        int IList.IndexOf(object value)
        {
            return ((IList)_list).IndexOf(value);
        }
        void IList.Insert(int index, object value)
        {
            throw new NotSupportedException();
        }
        void IList.Remove(object value)
        {
            throw new NotSupportedException();
        }
        void IList.RemoveAt(int index)
        {
            throw new NotSupportedException();
        }
        //---------------------------------------------------------------------
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        bool ICollection<T>.IsReadOnly
        {
            get { return true; }
        }
        object ICollection.SyncRoot
        {
            get { return ((ICollection)_list).SyncRoot; }
        }
        void ICollection<T>.Add(T value)
        {
            throw new NotSupportedException();
        }
        bool ICollection<T>.Remove(T value)
        {
            throw new NotSupportedException();
        }
        void ICollection<T>.Clear()
        {
            throw new NotSupportedException();
        }
        void ICollection.CopyTo(Array array, int index)
        {
            ((ICollection)_list).CopyTo(array, index);
        }
        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }
        //---------------------------------------------------------------------
    }
}
