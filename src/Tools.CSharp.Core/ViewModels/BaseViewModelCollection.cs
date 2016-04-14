using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Tools.CSharp.Collections;

namespace Tools.CSharp.ViewModels
{
    public abstract class BaseViewModelCollection<TItem> : BaseViewModel, IList<TItem>, IList
        where TItem : class
    {
        #region private
        private readonly ImprovedObservableCollection<TItem> _items;
        //---------------------------------------------------------------------
        private Object _syncRoot;
        //---------------------------------------------------------------------
        private IList _List
        {
            get { return _items; }
        }
        private IList<TItem> _ListItems
        {
            get { return _items; }
        }
        private void _readOnlyItemsByThrowNotSupportedException()
        {
            if (_ListItems.IsReadOnly)
            { throw new NotSupportedException(); }
        }
        //---------------------------------------------------------------------
        private void _subscribeStationAllEvents(bool addEvents)
        {
            if (addEvents)
            { _items.ImprovedCollectionChanged += _itemsOnImprovedCollectionChanged; }
            else
            { _items.ImprovedCollectionChanged -= _itemsOnImprovedCollectionChanged; }
        }
        private void _itemsOnImprovedCollectionChanged(object sender, ImprovedNotifyCollectionChangedEventArgs<TItem> e)
        {
            CollectionChanged?.Invoke(this, new ViewModelCollectionChangedEventArgs<TItem>(e));
        }
        //---------------------------------------------------------------------
        private static bool _IsCompatibleObject(object value)
        {
            return value is TItem;
        }
        #endregion
        #region protected
        protected IList<TItem> Items
        {
            get { return _items; }
        }

        protected virtual void ClearItems()
        {
            _items.Clear();
        }
        protected virtual void RemoveItem(int index)
        {
            _items.RemoveAt(index);
        }
        protected virtual void InsertItem(int index, TItem item)
        {
            _items.Insert(index, item);
        }
        protected virtual void SetItem(int index, TItem item)
        {
            _items[index] = item;
        }

        protected override void Dispose(bool disposing)
        {
            try
            {
                if (!IsDisposed && disposing)
                {
                    _subscribeStationAllEvents(false);
                }
            }
            finally
            { base.Dispose(disposing); }
        }
        #endregion
        protected BaseViewModelCollection()
        {
            _items = new ImprovedObservableCollection<TItem>();
            _subscribeStationAllEvents(true);
        }
        protected BaseViewModelCollection(IList<TItem> list)
        {
            if (list == null)
            { throw new ArgumentNullException(nameof(list)); }

            _items = new ImprovedObservableCollection<TItem>(list);
            _subscribeStationAllEvents(true);
        }

        //---------------------------------------------------------------------
        public int Count
        {
            get { return _items.Count; }
        }
        public TItem this[int index]
        {
            get { return _items[index]; }
            set
            {
                _readOnlyItemsByThrowNotSupportedException();

                if (index < 0 || index >= _items.Count)
                { throw new ArgumentOutOfRangeException(nameof(index)); }

                SetItem(index, value);
            }
        }
        //---------------------------------------------------------------------
        bool ICollection<TItem>.IsReadOnly
        {
            get { return _ListItems.IsReadOnly; }
        }
        //---------------------------------------------------------------------
        bool ICollection.IsSynchronized
        {
            get { return false; }
        }
        object ICollection.SyncRoot
        {
            get
            {
                if (_syncRoot == null)
                {
                    var collection = _items as ICollection;
                    if (collection != null)
                    { _syncRoot = collection.SyncRoot; }
                    else
                    { Interlocked.CompareExchange(ref _syncRoot, new object(), null); }
                }
                return _syncRoot;
            }
        }
        //---------------------------------------------------------------------
        public void Add(TItem item)
        {
            _readOnlyItemsByThrowNotSupportedException();

            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            var index = _items.Count;
            InsertItem(index, item);
        }
        public void Clear()
        {
            _readOnlyItemsByThrowNotSupportedException();

            ClearItems();
        }
        public void CopyTo(TItem[] array, int arrayIndex)
        {
            _items.CopyTo(array, arrayIndex);
        }
        public bool Contains(TItem item)
        {
            return item != null && _items.Contains(item);
        }
        public int IndexOf(TItem item)
        {
            return item == null ? -1 : _items.IndexOf(item);
        }
        public void Insert(int index, TItem item)
        {
            if (item == null)
            { throw new ArgumentNullException(nameof(item)); }

            _readOnlyItemsByThrowNotSupportedException();

            InsertItem(index, item);
        }
        public bool Remove(TItem item)
        {
            _readOnlyItemsByThrowNotSupportedException();

            var index = _items.IndexOf(item);
            if (index == -1)
            { return false; }

            RemoveItem(index);

            return true;
        }
        public void RemoveAt(int index)
        {
            _readOnlyItemsByThrowNotSupportedException();

            if (index < 0 || index >= _items.Count)
            { throw new ArgumentOutOfRangeException(nameof(index)); }

            RemoveItem(index);
        }
        //---------------------------------------------------------------------
        void ICollection.CopyTo(Array array, int index)
        {
            if (array == null)
            { throw new ArgumentNullException(nameof(array)); }

            if (array.Rank != 1)
            { throw new ArgumentException(string.Empty, nameof(array)); }

            if (array.GetLowerBound(0) != 0)
            { throw new ArgumentException(string.Empty, nameof(array)); }

            if (index < 0)
            { throw new ArgumentOutOfRangeException(nameof(index)); }

            if (array.Length - index < Count)
            { throw new ArgumentException(); }

            var tArray = array as TItem[];
            if (tArray != null)
            { _items.CopyTo(tArray, index); }
            else
            {
                var targetType = array.GetType().GetElementType();
                var sourceType = typeof(TItem);
                if (!(targetType.IsAssignableFrom(sourceType) || sourceType.IsAssignableFrom(targetType)))
                { throw new ArgumentException(string.Empty, nameof(array)); }

                var objects = array as object[];
                if (objects == null)
                { throw new ArgumentException(string.Empty, nameof(array)); }

                var count = _items.Count;
                try
                {
                    for (var i = 0; i < count; i++)
                    { objects[index++] = _items[i]; }
                }
                catch (ArrayTypeMismatchException)
                { throw new ArgumentException(string.Empty, nameof(array)); }
            }
        }
        //---------------------------------------------------------------------
        object IList.this[int index]
        {
            get { return _items[index]; }
            set
            {
                if (value == null)
                { throw new ArgumentNullException(nameof(value)); }

                try
                {
                    this[index] = (TItem)value;
                }
                catch (InvalidCastException)
                { throw new ArgumentException(nameof(value)); }
            }
        }
        bool IList.IsReadOnly
        {
            get { return _List.IsReadOnly; }
        }
        bool IList.IsFixedSize
        {
            get { return _List.IsFixedSize; }
        }
        int IList.Add(object value)
        {
            _readOnlyItemsByThrowNotSupportedException();

            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            try
            {
                Add((TItem)value);
            }
            catch (InvalidCastException)
            { throw new ArgumentException(nameof(value)); }

            return Count - 1;
        }
        bool IList.Contains(object value)
        {
            return _IsCompatibleObject(value) && Contains((TItem)value);
        }
        int IList.IndexOf(object value)
        {
            return _IsCompatibleObject(value) ? IndexOf((TItem)value) : -1;
        }
        void IList.Insert(int index, object value)
        {
            _readOnlyItemsByThrowNotSupportedException();

            if (value == null)
            { throw new ArgumentNullException(nameof(value)); }

            try
            {
                Insert(index, (TItem)value);
            }
            catch (InvalidCastException)
            { throw new ArgumentException(nameof(value)); }
        }
        void IList.Remove(object value)
        {
            _readOnlyItemsByThrowNotSupportedException();

            if (_IsCompatibleObject(value))
            { Remove((TItem)value); }
        }
        //---------------------------------------------------------------------
        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_items).GetEnumerator();
        }
        public IEnumerator<TItem> GetEnumerator()
        {
            return _items.GetEnumerator();
        }
        //---------------------------------------------------------------------
        public event EventHandler<ViewModelCollectionChangedEventArgs<TItem>> CollectionChanged;
        //---------------------------------------------------------------------
    }
}